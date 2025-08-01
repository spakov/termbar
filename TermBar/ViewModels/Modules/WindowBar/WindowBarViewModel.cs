﻿using Microsoft.Extensions.Logging;
using Spakov.TermBar.Models;
using Spakov.TermBar.Views.Modules.WindowBar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.ViewModels.Modules.WindowBar
{
    /// <summary>
    /// The window bar viewmodel.
    /// </summary>
    internal partial class WindowBarViewModel : INotifyPropertyChanged
    {
        private readonly ILogger? _logger;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly WindowBarView _windowBarView;
        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.WindowBar _moduleConfig;

        private readonly ObservableCollection<Window> _models = WindowList.Windows;
        private readonly ObservableCollection<WindowBarWindowView> _views;
        private readonly Dictionary<HWND, CancellationTokenSource> _pendingRemovals;

        private WindowBarWindowView? _foregroundWindow;
        private WindowBarWindowView? _lastForegroundWindow;

        /// <summary>
        /// The list of <see cref="WindowBarWindowView"/>s to be presented to
        /// the user.
        /// </summary>
        internal ObservableCollection<WindowBarWindowView> Windows => _views;

        /// <summary>
        /// The currently foregrounded window.
        /// </summary>
        internal WindowBarWindowView? ForegroundWindow
        {
            get => _foregroundWindow;

            set
            {
                if (ForegroundWindow?.HWnd != value?.HWnd)
                {
                    _logger?.LogDebug(
                        "WindowList foreground window: {hWnd} \"{name}\"",
                        WindowList.ForegroundWindow?.HWnd,
                        WindowList.ForegroundWindow?.Name
                    );

                    _logger?.LogDebug(
                        "ForegroundWindow: {oldHWnd} \"{oldName}\" -> {newHWnd} \"{newName}\"",
                        ForegroundWindow?.HWnd,
                        ForegroundWindow?.WindowName,
                        value?.HWnd,
                        value?.WindowName
                    );

                    _lastForegroundWindow = _foregroundWindow;
                    _foregroundWindow = value;

                    // This bears some explanation: normally we'd just invoke
                    // OnPropertyChanged() here, but we can end up in a
                    // scenario in which we're trying to update the selected
                    // item (this property is bound to SelectedItem in
                    // WindowBarView.xaml) but it hasn't actually been added to
                    // the list yet, due to the "lazy" way the ListView's
                    // ItemsSource works. So, instead, we use the ListView's
                    // LayoutUpdated event to generate a PropertyChanged on
                    // WindowBarViewModel's behalf. This allows things to
                    // settle before trying to update the selected item. For
                    // the sake of completeness, it's worth mentioning that it
                    // is not always necessary to invoke UpdateLayout()--a
                    // layout update is automatically invoked after adding or
                    // removing a ListView item. However, it doesn't hurt to
                    // have it be invoked multiple times.
                    App.DispatcherQueue!.TryEnqueue(_windowBarView.UpdateLayout);

                    _logger?.LogInformation(
                        "WindowBarView foreground window: {hWnd} \"{name}\"",
                        ((WindowBarWindowView?)((Microsoft.UI.Xaml.Controls.ListView?)_windowBarView.Content)?.SelectedItem)?.HWnd,
                        ((WindowBarWindowView?)((Microsoft.UI.Xaml.Controls.ListView?)_windowBarView.Content)?.SelectedItem)?.WindowName
                    );
                }
            }
        }

        /// <summary>
        /// The last foregrounded window.
        /// </summary>
        internal WindowBarWindowView? LastForegroundWindow => _lastForegroundWindow;

        /// <summary>
        /// Initializes a <see cref="WindowBarViewModel"/>.
        /// </summary>
        /// <param name="windowBarView">The <see cref="WindowBarView"/>
        /// associated with this <see cref="WindowBarViewModel"/>.</param>
        /// <param name="config"><inheritdoc cref="WindowBarView.WindowBarView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig"><inheritdoc
        /// cref="WindowBarView.WindowBarView"
        /// path="/param[@name='moduleConfig']"/></param>
        internal WindowBarViewModel(WindowBarView windowBarView, Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig)
        {
            _logger = LoggerHelper.CreateLogger<WindowBarViewModel>();

            _windowBarView = windowBarView;
            _config = config;
            _moduleConfig = moduleConfig;
            _views = [];
            _pendingRemovals = [];

            WindowBarWindowView? view;

            foreach (Window model in _models)
            {
                model.PropertyChanged += WindowChanged;

                if (!model.IsInteresting)
                {
                    continue;
                }

                view = new(config, moduleConfig, model.HWnd, model.ProcessId, model.Name);

                WindowListHelper.OrderAndInsert(
                    config.WindowList,
                    view,
                    _views,
                    view.WindowProcessId,
                    view.WindowName
                );

                _logger?.LogInformation("Visually tracking window {hWnd} \"{name}\"", view.HWnd, view.WindowName);
            }

            _models.CollectionChanged += Models_CollectionChanged;

            ForegroundWindowChanged(this, new(nameof(WindowBarViewModel)));
            WindowList.Instance!.PropertyChanged += ForegroundWindowChanged;
        }

        /// <summary>
        /// Foregrounds the window represented by <paramref name="view"/>.
        /// </summary>
        /// <param name="view">A <see cref="WindowBarWindowView"/>.</param>
        internal void Foreground(WindowBarWindowView view) => WindowList.ForegroundWindow = FindModel(view);

        /// <summary>
        /// Iconifies the last foregrounded window.
        /// </summary>
        /// <remarks>We iconify the last foregrounded window because the
        /// currently foregrounded window is always TermBar.</remarks>
        internal void Iconify()
        {
            WindowList.ForegroundWindow = FindModel(_lastForegroundWindow);
            WindowList.Iconify();
        }

        /// <summary>
        /// Handles changes to the window list model.
        /// </summary>
        /// <param name="sender"><inheritdoc
        /// cref="NotifyCollectionChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc
        /// cref="NotifyCollectionChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void Models_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                if (e.NewItems is null)
                {
                    return;
                }

                WindowBarWindowView? view;

                foreach (Window model in e.NewItems)
                {
                    model.PropertyChanged += WindowChanged;

                    if (!model.IsInteresting)
                    {
                        continue;
                    }

                    if (_pendingRemovals.TryGetValue(model.HWnd, out CancellationTokenSource? cts))
                    {
                        if (cts is not null)
                        {
                            cts.Cancel();
                            _pendingRemovals.Remove(model.HWnd);
                        }
                    }
                    else
                    {
                        view = new(_config, _moduleConfig, model.HWnd, model.ProcessId, model.Name);

                        WindowListHelper.OrderAndInsert(
                            _config.WindowList,
                            view,
                            _views,
                            view.WindowProcessId,
                            view.WindowName
                        );

                        _logger?.LogInformation("Visually tracking window {hWnd} \"{name}\"", view.HWnd, view.WindowName);
                    }
                }
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                if (e.OldItems is null)
                {
                    return;
                }

                List<WindowBarWindowView> toRemove = [];

                foreach (Window model in e.OldItems)
                {
                    WindowBarWindowView? view = FindView(model);

                    if (view is not null)
                    {
                        toRemove.Add(view);
                    }
                }

                foreach (WindowBarWindowView view in toRemove)
                {
                    ScheduleRemoval(view);
                }
            }

            ForegroundWindowChanged(this, new(nameof(WindowBarViewModel)));
        }

        /// <summary>
        /// Invoked when a <see cref="Window"/> changes.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="PropertyChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="PropertyChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void WindowChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            Window model = (Window)sender;
            WindowBarWindowView? view = FindView(model);

            if (view is not null && e.PropertyName == nameof(Window.Name))
            {
                view.WindowName = model.Name;
            }
            else if (e.PropertyName == nameof(Window.IsInteresting))
            {
                if (view is null)
                {
                    if (model.IsInteresting)
                    {
                        if (_pendingRemovals.TryGetValue(model.HWnd, out CancellationTokenSource? cts))
                        {
                            if (cts is not null)
                            {
                                cts.Cancel();
                                _pendingRemovals.Remove(model.HWnd);
                            }
                        }
                        else
                        {
                            view = new(_config, _moduleConfig, model.HWnd, model.ProcessId, model.Name);

                            WindowListHelper.OrderAndInsert(
                                _config.WindowList,
                                view,
                                _views,
                                view.WindowProcessId,
                                view.WindowName
                            );

                            _logger?.LogInformation("Visually tracking window {hWnd} \"{name}\"", view.HWnd, view.WindowName);
                        }
                    }
                }
                else
                {
                    if (!model.IsInteresting)
                    {
                        ScheduleRemoval(view);
                    }
                }
            }

            ForegroundWindowChanged(this, new(nameof(WindowBarViewModel)));
        }

        /// <summary>
        /// Sets <see cref="ForegroundWindow"/> to <see
        /// cref="WindowList.ForegroundWindow"/>.
        /// </summary>
        /// <remarks>Effectively does nothing if the new window is the same as
        /// the old window.</remarks>
        /// <param name="sender"><inheritdoc cref="PropertyChangedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="PropertyChangedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void ForegroundWindowChanged(object? sender, PropertyChangedEventArgs e) => ForegroundWindow = WindowList.ForegroundWindow is null ? null : FindView(WindowList.ForegroundWindow);

        /// <summary>
        /// Schedules removal of <paramref name="view"/> from the window bar.
        /// </summary>
        /// <param name="view">A <see cref="WindowBarWindowView"/>.</param>
        private async void ScheduleRemoval(WindowBarWindowView view)
        {
            CancellationTokenSource cts = new();

            _pendingRemovals[view.HWnd] = cts;

            try
            {
                await Task.Delay(100, cts.Token);
                _views.Remove(view);
                _pendingRemovals.Remove(view.HWnd);
                _logger?.LogInformation("No longer visually tracking window {hWnd} \"{name}\"", view.HWnd, view.WindowName);
            }
            catch (TaskCanceledException) {
            }
        }

        /// <summary>
        /// Gets the view corresponding to <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="Window"/> to look up.</param>
        /// <returns>The view corresponding to <paramref name="model"/>, or
        /// <c>null</c> if there isn't one.</returns>
        private WindowBarWindowView? FindView(Window? model)
        {
            if (model is null)
            {
                return null;
            }

            foreach (WindowBarWindowView view in _views)
            {
                if (view.HWnd == model.HWnd)
                {
                    return view;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the model corresponding to <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The <see cref="WindowBarWindowView"/> to look
        /// up.</param>
        /// <returns>The model corresponding to <paramref name="view"/>, or
        /// <c>null</c> if there isn't one.</returns>
        private Window? FindModel(WindowBarWindowView? view)
        {
            if (view is null)
            {
                return null;
            }

            foreach (Window model in _models)
            {
                if (model.HWnd == view.HWnd)
                {
                    return model;
                }
            }

            return null;
        }

        internal void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}