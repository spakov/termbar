using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermBar.Catppuccin;
using TermBar.Models;
using TermBar.Views.Modules.WindowDropdown;

namespace TermBar.ViewModels.Modules.WindowDropdown {
  internal partial class WindowDropdownViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.WindowDropdown moduleConfig;

    private readonly ObservableCollection<Window> models = WindowList.Windows;
    private readonly ObservableCollection<WindowDropdownMenuFlyoutItemView> views = [];

    /// <summary>
    /// The dropdown icon.
    /// </summary>
    internal string? Icon => moduleConfig.DropdownIcon;

    /// <summary>
    /// The dropdown icon color.
    /// </summary>
    internal SolidColorBrush? IconColor => PaletteHelper.Palette[config.Flavor].Colors[moduleConfig.AccentColor].SolidColorBrush;

    /// <summary>
    /// The list of <see cref="WindowDropdownMenuFlyoutItemView"/>s to be
    /// presented to the user.
    /// </summary>
    internal ObservableCollection<WindowDropdownMenuFlyoutItemView> Windows => views;

    /// <summary>
    /// The currently foregrounded window.
    /// </summary>
    internal WindowDropdownMenuFlyoutItemView? ForegroundedWindow {
      get => WindowList.ForegroundedWindow is null ? null : FindView(WindowList.ForegroundedWindow)!;
      set {
        WindowList.ForegroundedWindow = value is null ? null : FindModel(value);
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Initializes a <see cref="WindowDropdownViewModel"/>.
    /// </summary>
    /// <param name="config"><inheritdoc
    /// cref="WindowDropdownView.WindowDropdownView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig"><inheritdoc
    /// cref="WindowDropdownView.WindowDropdownView"
    /// path="/param[@name='moduleConfig']"/></param>
    internal WindowDropdownViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowDropdown moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      WindowDropdownMenuFlyoutItemView? view;

      foreach (Window model in models) {
        do {
          view = FindView(model);
          if (view is not null) views.Remove(view);
        } while (view is not null);

        views.Add(new(config, moduleConfig, model.HWnd, model.ProcessId, model.Name));
        model.PropertyChanged += (sender, e) => FindView((Window) sender!)!.WindowName = ((Window) sender!).Name;
      }

      models.CollectionChanged += Models_CollectionChanged;

      WindowList.Instance.PropertyChanged += (sender, e) => ForegroundedWindow = WindowList.ForegroundedWindow is null ? null : FindView(WindowList.ForegroundedWindow);
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
        WindowDropdownMenuFlyoutItemView? view;

        // Remove any duplicates that were added prior to event handler registration
        foreach (Window model in e.NewItems!) {
          do {
            view = FindView(model);
            if (view is not null) views.Remove(view);
          } while (view is not null);
        }

        foreach (Window model in e.NewItems!) {
          views.Add(new(config, moduleConfig, model.HWnd, model.ProcessId, model.Name));
          model.PropertyChanged += (sender, e) => FindView((Window) sender!)!.WindowName = ((Window) sender!).Name;
        }
      } else if (e.Action.Equals(NotifyCollectionChangedAction.Remove)) {
        List<WindowDropdownMenuFlyoutItemView> toRemove = [];

        foreach (Window model in e.OldItems!) {
          toRemove.Add(FindView(model)!);
        }

        foreach (WindowDropdownMenuFlyoutItemView view in toRemove) {
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
    private WindowDropdownMenuFlyoutItemView? FindView(Window model) {
      foreach (WindowDropdownMenuFlyoutItemView view in views) {
        if (view.HWnd == model.HWnd) return view;
      }

      return null;
    }

    /// <summary>
    /// Gets the model corresponding to <paramref name="view"/>.
    /// </summary>
    /// <param name="view">The <see cref="WindowDropdownMenuFlyoutItemView"/>
    /// to look up.</param>
    /// <returns>The model corresponding to <paramref name="view"/>, or
    /// <c>null</c> if there isn't one.</returns>
    private Window? FindModel(WindowDropdownMenuFlyoutItemView view) {
      foreach (Window model in models) {
        if (model.HWnd == view.HWnd) return model;
      }

      return null;
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}
