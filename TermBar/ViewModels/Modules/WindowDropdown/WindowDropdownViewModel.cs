using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.Models;
using Spakov.TermBar.Views.Modules.WindowDropdown;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Spakov.TermBar.ViewModels.Modules.WindowDropdown
{
    /// <summary>
    /// The window dropdown viewmodel.
    /// </summary>
    internal class WindowDropdownViewModel
    {
        private readonly Configuration.Json.TermBar _config;
        private readonly Configuration.Json.Modules.WindowDropdown _moduleConfig;

        private readonly ObservableCollection<Window> _models = WindowList.Windows;
        private readonly ObservableCollection<WindowDropdownMenuFlyoutItemView> _views;

        /// <summary>
        /// The dropdown icon.
        /// </summary>
        internal string? Icon => _moduleConfig.DropdownIcon;

        /// <summary>
        /// The dropdown icon color.
        /// </summary>
        internal SolidColorBrush? IconColor => Palette.Instance[_config.Flavor].Colors[_moduleConfig.AccentColor].SolidColorBrush;

        /// <summary>
        /// The list of <see cref="WindowDropdownMenuFlyoutItemView"/>s to be
        /// presented to the user.
        /// </summary>
        internal ObservableCollection<WindowDropdownMenuFlyoutItemView> Windows => _views;

        /// <summary>
        /// Initializes a <see cref="WindowDropdownViewModel"/>.
        /// </summary>
        /// <param name="config"><inheritdoc
        /// cref="WindowDropdownView.WindowDropdownView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig"><inheritdoc
        /// cref="WindowDropdownView.WindowDropdownView"
        /// path="/param[@name='moduleConfig']"/></param>
        internal WindowDropdownViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig)
        {
            _config = config;
            _moduleConfig = moduleConfig;
            _views = [];

            WindowDropdownMenuFlyoutItemView? view;

            foreach (Window model in _models)
            {
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
            }

            _models.CollectionChanged += Models_CollectionChanged;
        }

        /// <summary>
        /// Foregrounds the window represented by <paramref name="view"/>.
        /// </summary>
        /// <param name="view">A <see
        /// cref="WindowDropdownMenuFlyoutItemView"/>.</param>
        internal void Foreground(WindowDropdownMenuFlyoutItemView view) => WindowList.ForegroundWindow = FindModel(view);

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

                WindowDropdownMenuFlyoutItemView? view;

                foreach (Window model in e.NewItems)
                {
                    model.PropertyChanged += WindowChanged;

                    if (!model.IsInteresting)
                    {
                        continue;
                    }

                    view = new(_config, _moduleConfig, model.HWnd, model.ProcessId, model.Name);

                    WindowListHelper.OrderAndInsert(
                        _config.WindowList,
                        view,
                        _views,
                        view.WindowProcessId,
                        view.WindowName
                    );
                }
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                if (e.OldItems is null)
                {
                    return;
                }

                List<WindowDropdownMenuFlyoutItemView> toRemove = [];

                foreach (Window model in e.OldItems)
                {
                    WindowDropdownMenuFlyoutItemView? view = FindView(model);

                    if (view is not null)
                    {
                        toRemove.Add(view);
                    }
                }

                foreach (WindowDropdownMenuFlyoutItemView view in toRemove)
                {
                    _views.Remove(view);
                }
            }
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
            WindowDropdownMenuFlyoutItemView? view = FindView(model);

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
                        view = new(_config, _moduleConfig, model.HWnd, model.ProcessId, model.Name);

                        WindowListHelper.OrderAndInsert(
                            _config.WindowList,
                            view,
                            _views,
                            view.WindowProcessId,
                            view.WindowName
                        );
                    }
                }
                else
                {
                    if (!model.IsInteresting)
                    {
                        _views.Remove(view);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the view corresponding to <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="Window"/> to look up.</param>
        /// <returns>The view corresponding to <paramref name="model"/>, or
        /// <c>null</c> if there isn't one.</returns>
        private WindowDropdownMenuFlyoutItemView? FindView(Window? model)
        {
            if (model is null)
            {
                return null;
            }

            foreach (WindowDropdownMenuFlyoutItemView view in _views)
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
        /// <param name="view">The <see
        /// cref="WindowDropdownMenuFlyoutItemView"/> to look up.</param>
        /// <returns>The model corresponding to <paramref name="view"/>, or
        /// <c>null</c> if there isn't one.</returns>
        private Window? FindModel(WindowDropdownMenuFlyoutItemView? view)
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
    }
}