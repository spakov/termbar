using Spakov.TermBar.Models;
using Spakov.TermBar.Views.Modules.WindowBar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.WindowBar {
  /// <summary>
  /// The window bar viewmodel.
  /// </summary>
  internal partial class WindowBarViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly WindowBarView windowBarView;
    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.WindowBar moduleConfig;

    private readonly ObservableCollection<Window> models = WindowList.Windows;
    private readonly ObservableCollection<WindowBarWindowView> views = [];

    /// <summary>
    /// The list of <see cref="WindowBarWindowView"/>s to be presented to the
    /// user.
    /// </summary>
    internal ObservableCollection<WindowBarWindowView> Windows => views;

    /// <summary>
    /// The currently foregrounded window.
    /// </summary>
    internal WindowBarWindowView? ForegroundedWindow {
      get => WindowList.ForegroundedWindow is null ? null : FindView(WindowList.ForegroundedWindow)!;

      set {
        WindowList.ForegroundedWindow = value is null ? null : FindModel(value);
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Iconifies the foregrounded window.
    /// </summary>
    internal static void Iconify() => WindowList.Iconify();

    /// <summary>
    /// Initializes a <see cref="WindowBarViewModel"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="WindowBarView.WindowBarView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig"><inheritdoc cref="WindowBarView.WindowBarView"
    /// path="/param[@name='moduleConfig']"/></param>
    internal WindowBarViewModel(WindowBarView windowBarView, Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig) {
      this.windowBarView = windowBarView;
      this.config = config;
      this.moduleConfig = moduleConfig;

      WindowBarWindowView? view;

      foreach (Window model in models) {
        do {
          view = FindView(model);
          if (view is not null) views.Remove(view);
        } while (view is not null);

        view = new(config, moduleConfig, model.HWnd, model.ProcessId, model.Name);
        WindowListHelper.OrderAndInsert(config.WindowList, view, views, view.WindowProcessId, view.WindowName!);

        model.PropertyChanged += (sender, e) => FindView((Window) sender!)!.WindowName = ((Window) sender!).Name;
      }

      models.CollectionChanged += Models_CollectionChanged;

      WindowList.Instance!.PropertyChanged += (sender, e) => ForegroundedWindow = WindowList.ForegroundedWindow is null ? null : FindView(WindowList.ForegroundedWindow);
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
    private void Models_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
      if (e.Action.Equals(NotifyCollectionChangedAction.Add)) {
        WindowBarWindowView? view;

        // Remove any duplicates that were added prior to event handler registration
        foreach (Window model in e.NewItems!) {
          do {
            view = FindView(model);
            if (view is not null) views.Remove(view);
          } while (view is not null);
        }

        foreach (Window model in e.NewItems!) {
          view = new(config, moduleConfig, model.HWnd, model.ProcessId, model.Name);

          windowBarView.SetSelectedWindowIndex(
            WindowListHelper.OrderAndInsert(
              config.WindowList,
              view,
              views,
              view.WindowProcessId,
              view.WindowName!
            )
          );

          model.PropertyChanged += (sender, e) => FindView((Window) sender!)!.WindowName = ((Window) sender!).Name;
        }
      } else if (e.Action.Equals(NotifyCollectionChangedAction.Remove)) {
        List<WindowBarWindowView> toRemove = [];

        foreach (Window model in e.OldItems!) {
          toRemove.Add(FindView(model)!);
        }

        foreach (WindowBarWindowView view in toRemove) {
          views.Remove(view);
        }
      }
    }

    /// <summary>
    /// Gets the view corresponding to <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The <see cref="Window"/> to look up.</param>
    /// <returns>The view corresponding to <paramref name="model"/>, or
    /// <c>null</c> if there isn't one.</returns>
    private WindowBarWindowView? FindView(Window model) {
      foreach (WindowBarWindowView view in views) {
        if (view.HWnd == model.HWnd) return view;
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
    private Window? FindModel(WindowBarWindowView view) {
      foreach (Window model in models) {
        if (model.HWnd == view.HWnd) return model;
      }

      return null;
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}