using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermBar.Models;
using TermBar.Views.Modules.WindowBar;

namespace TermBar.ViewModels.Modules.WindowBar {
  /// <summary>
  /// The window bar viewmodel.
  /// </summary>
  internal partial class WindowBarViewModel : INotifyPropertyChanged, IEnumerable<WindowBarWindowView>, IList<WindowBarWindowView> {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.WindowBar moduleConfig;

    // TODO: need locks for these
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
    internal WindowBarViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      WindowBarWindowView? view;

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
        WindowBarWindowView? view;

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

    WindowBarWindowView IList<WindowBarWindowView>.this[int index] { get => Windows[index]; set => Windows[index] = value; }

    int ICollection<WindowBarWindowView>.Count => Windows.Count;

    bool ICollection<WindowBarWindowView>.IsReadOnly => true;

    void ICollection<WindowBarWindowView>.Add(WindowBarWindowView item) => Windows.Add(item);
    void ICollection<WindowBarWindowView>.Clear() => Windows.Clear();
    bool ICollection<WindowBarWindowView>.Contains(WindowBarWindowView item) => Windows.Contains(item);
    void ICollection<WindowBarWindowView>.CopyTo(WindowBarWindowView[] array, int arrayIndex) => Windows.CopyTo(array, arrayIndex);

    IEnumerator<WindowBarWindowView> IEnumerable<WindowBarWindowView>.GetEnumerator() => new WindowBarViewModelEnumerator(Windows);

    IEnumerator IEnumerable.GetEnumerator() => new WindowBarViewModelEnumerator(Windows);

    int IList<WindowBarWindowView>.IndexOf(WindowBarWindowView item) => Windows.IndexOf(item);
    void IList<WindowBarWindowView>.Insert(int index, WindowBarWindowView item) => Windows.Insert(index, item);
    bool ICollection<WindowBarWindowView>.Remove(WindowBarWindowView item) => Windows.Remove(item);
    void IList<WindowBarWindowView>.RemoveAt(int index) => Windows.RemoveAt(index);

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }

  /// <summary>
  /// Implementation of <see cref="IEnumerator{T}"/> for <see
  /// cref="WindowBarViewModel"/>.
  /// </summary>
  /// <param name="windows"><see cref="ObservableCollection{T}"/> of <see
  /// cref="WindowBarWindowView"/>.</param>
  internal partial class WindowBarViewModelEnumerator(ObservableCollection<WindowBarWindowView> windows) : IEnumerator<WindowBarWindowView> {
    private int index = -1;

    object IEnumerator.Current {
      get {
        try {
          return windows[index];
        } catch (IndexOutOfRangeException) {
          throw new InvalidOperationException();
        }
      }
    }

    WindowBarWindowView IEnumerator<WindowBarWindowView>.Current {
      get {
        try {
          return windows[index];
        } catch (IndexOutOfRangeException) {
          throw new InvalidOperationException();
        }
      }
    }

    bool IEnumerator.MoveNext() {
      index++;

      return index < windows.Count;
    }

    void IEnumerator.Reset() => index = -1;

    public void Dispose() { }
  }
}
