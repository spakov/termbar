using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace TermBar.Views.Modules.WindowDropdown {
  /// <summary>
  /// A <see cref="MenuFlyout"/> with an <see cref="ItemsSourceProperty"/>.
  /// </summary>
  internal sealed partial class WindowDropdownBindableMenuFlyoutView : MenuFlyout {
    private static Style? fontIconStyle;

    /// <summary>
    /// The <see cref="Configuration.Json.TermBar"/>.
    /// </summary>
    public Configuration.Json.TermBar? Config {
      get => (Configuration.Json.TermBar?) GetValue(ConfigProperty);
      set => SetValue(ConfigProperty, value);
    }

    /// <summary>
    /// The <see cref="IconPath"/> property.
    /// </summary>
    public static readonly DependencyProperty ConfigProperty = DependencyProperty.Register(
      nameof(Config),
      typeof(Configuration.Json.TermBar),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(null, OnConfigChanged)
    );

    /// <summary>
    /// A <see cref="IEnumerable{T}"/> of <see
    /// cref="WindowDropdownMenuFlyoutItemView"/>s to present to the user.
    /// </summary>
    public object ItemsSource {
      get => GetValue(ItemsSourceProperty);
      set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// The <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
      nameof(ItemsSource),
      typeof(object),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(null, OnItemsSourceChanged)
    );

    /// <summary>
    /// The click <see cref="Action{T}"/>, passed <see
    /// cref="WindowDropdownMenuFlyoutItemView"/>.
    /// </summary>
    public Action<WindowDropdownMenuFlyoutItemView>? ClickAction {
      get => (Action<WindowDropdownMenuFlyoutItemView>?) GetValue(ClickActionProperty);
      set => SetValue(ClickActionProperty, value);
    }

    /// <summary>
    /// The <see cref="ClickAction"/> property.
    /// </summary>
    public static readonly DependencyProperty ClickActionProperty = DependencyProperty.Register(
      nameof(ClickAction),
      typeof(Action<WindowDropdownMenuFlyoutItemView>),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(null)
    );

    /// <summary>
    /// The name of the property to which to bind to
    /// <c>MenuFlyoutItem.Text</c>.
    /// </summary>
    public string? TextPath {
      get => (string?) GetValue(TextPathProperty);
      set => SetValue(TextPathProperty, value);
    }

    /// <summary>
    /// The <see cref="TextPath"/> property.
    /// </summary>
    public static readonly DependencyProperty TextPathProperty = DependencyProperty.Register(
      nameof(TextPath),
      typeof(string),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(string.Empty)
    );

    /// <summary>
    /// The name of the property to which to bind to
    /// <c>MenuFlyoutItem.Icon.FontIcon.Glyph</c>.
    /// </summary>
    public string? IconPath {
      get => (string?) GetValue(IconPathProperty);
      set => SetValue(IconPathProperty, value);
    }

    /// <summary>
    /// The <see cref="IconPath"/> property.
    /// </summary>
    public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
      nameof(IconPath),
      typeof(string),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(string.Empty)
    );

    /// <summary>
    /// The name of the property to which to bind to
    /// <c>MenuFlyoutItem.Click</c>.
    /// </summary>
    public Action? ClickPath {
      get => (Action?) GetValue(ClickPathProperty);
      set => SetValue(ClickPathProperty, value);
    }

    /// <summary>
    /// The <see cref="ClickPath"/> property.
    /// </summary>
    public static readonly DependencyProperty ClickPathProperty = DependencyProperty.Register(
      nameof(ClickPath),
      typeof(Action),
      typeof(WindowDropdownBindableMenuFlyoutView),
      new PropertyMetadata(null)
    );

    /// <summary>
    /// Invoked when <see cref="Config"/> changes.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='e']"/></param>
    private static void OnConfigChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      if (e.NewValue is Configuration.Json.TermBar) {
        Configuration.Json.TermBar? config = e.NewValue as Configuration.Json.TermBar;

        if (config is not null) {
          WindowDropdownBindableMenuFlyoutView flyout = (WindowDropdownBindableMenuFlyoutView) sender;

          FontFamily fontIconFontFamily = new(config.FontFamily);

          fontIconStyle = new(typeof(FontIcon));
          fontIconStyle.Setters.Add(new Setter(FontIcon.FontFamilyProperty, fontIconFontFamily));

          flyout.RebuildItems();
        }
      }
    }

    /// <summary>
    /// Invoked when <see cref="ItemsSource"/> changes.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='e']"/></param>
    private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      WindowDropdownBindableMenuFlyoutView flyout = (WindowDropdownBindableMenuFlyoutView) sender;

      if (e.OldValue is INotifyCollectionChanged oldObservable) {
        oldObservable.CollectionChanged -= flyout.OnCollectionChanged;
      }

      if (e.NewValue is INotifyCollectionChanged newObservable) {
        newObservable.CollectionChanged += flyout.OnCollectionChanged;
      }

      flyout.RebuildItems();
    }

    /// <summary>
    /// Invoked when <see cref="ItemsSource"/> is a collection and changes.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="DependencyPropertyChangedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RebuildItems();

    /// <summary>
    /// Rebuilds the contained <see cref="WindowDropdownMenuFlyoutItemView"/>s.
    /// </summary>
    private void RebuildItems() {
      if (Config is null) return;

      Items.Clear();

      if (ItemsSource is IEnumerable windows) {
        foreach (WindowDropdownMenuFlyoutItemView window in windows) {
          window.Click += (sender, e) => ClickAction?.Invoke(window);

          if (!string.IsNullOrEmpty(TextPath)) {
            BindingOperations.SetBinding(
              window,
              WindowDropdownMenuFlyoutItemView.TextProperty,
              new Binding() {
                Path = new PropertyPath(TextPath),
                Mode = BindingMode.OneWay
              }
            );
          }

          if (!string.IsNullOrEmpty(IconPath)) {
            FontIcon icon = new();

            BindingOperations.SetBinding(
              icon,
              FontIcon.GlyphProperty,
              new Binding() {
                Path = new PropertyPath(IconPath),
                Mode = BindingMode.OneWay
              }
            );

            icon.Resources[typeof(FontIcon)] = fontIconStyle;
            window.Icon = icon;
          }

          Items.Add(window);
        }
      }
    }
  }
}
