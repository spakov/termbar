using Microsoft.UI.Xaml;
using Spakov.Catppuccin;

namespace Spakov.TermBar.Themes {
  /// <summary>
  /// Application theme helper methods.
  /// </summary>
  internal static class ThemeHelper {
    private const string lightKey = "Light";
    private const string darkKey = "Dark";

    private static ResourceDictionary? lightResourceDictionary;
    private static ResourceDictionary? darkResourceDictionary;

    /// <summary>
    /// Builds the application theme.
    /// </summary>
    /// <param name="appResources"><see cref="Application.Resources"/>.</param>
    /// <param name="config"><inheritdoc cref="Configuration.Json.TermBar"
    /// path="/summary"/></param>
    internal static void BuildTheme(ResourceDictionary appResources, Configuration.Json.TermBar config) {
      if (!appResources.ThemeDictionaries.ContainsKey(lightKey)) {
        appResources.ThemeDictionaries[lightKey] = new ResourceDictionary();
      }

      if (!appResources.ThemeDictionaries.ContainsKey(darkKey)) {
        appResources.ThemeDictionaries[darkKey] = new ResourceDictionary();
      }

      lightResourceDictionary = (ResourceDictionary) appResources.ThemeDictionaries[lightKey];
      darkResourceDictionary = (ResourceDictionary) appResources.ThemeDictionaries[darkKey];

      foreach (string key in new string[] {
        "ButtonForeground",
        "ButtonForegroundPointerOver",
        "ListViewItemForeground",
        "ListViewItemForegroundPointerOver"
      }) {
        SetResource(
          key,
          Palette.Instance[config.Flavor].Colors[config.TextColor].SolidColorBrush
        );
      }

      if (config.AccentBackground is not null) {
        foreach (string key in new string[] {
          "ButtonBackground",
          "ButtonBorderBrush",
          "ButtonBackgroundPointerOver",
          "ButtonBorderBrushPointerOver",
          "ListViewItemBackground",
          "ListViewItemBackgroundPointerOver",
          "TextControlBackground",
          "TextControlBorderBrush",
          "TextControlBackgroundPointerOver",
          "TextControlBorderBrushPointerOver",
          "TextControlBackgroundFocused",
          "TextControlBorderBrushFocused",
          "TextControlBackgroundDisabled",
          "TextControlBorderBrushDisabled"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.AccentBackground].SolidColorBrush
          );
        }
      }

      if (config.AccentColor is not null) {
        foreach (string key in new string[] {
          "DropDownButtonForegroundSecondary",
          "DropDownButtonForegroundSecondaryPointerOver",
          "ListViewItemSelectionIndicatorBrush",
          "ListViewItemSelectionIndicatorPointerOverBrush",
          "ListViewItemSelectionIndicatorPressedBrush"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.AccentColor].SolidColorBrush
          );
        }
      }

      if (config.SelectedBackground is not null) {
        foreach (string key in new string[] {
          "ListViewItemBackgroundSelected",
          "ListViewItemBackgroundSelectedPointerOver"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.SelectedBackground].SolidColorBrush
          );
        }
      }

      if (config.SelectedColor is not null) {
        foreach (string key in new string[] {
          "ListViewItemForegroundSelected"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.SelectedColor].SolidColorBrush
          );
        }
      }

      if (config.ClickedBackground is not null) {
        foreach (string key in new string[] {
          "ButtonBackgroundPressed",
          "ButtonBorderBrushPressed",
          "ListViewItemBackgroundPressed"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.ClickedBackground].SolidColorBrush
          );
        }
      }

      if (config.ClickedColor is not null) {
        foreach (string key in new string[] {
          "ButtonForegroundPressed",
          "DropDownButtonForegroundSecondaryPressed",
          "ListViewItemForegroundPressed"
        }) {
          SetResource(
            key,
            Palette.Instance[config.Flavor].Colors[(ColorEnum) config.ClickedColor].SolidColorBrush
          );
        }
      }
    }

    /// <summary>
    /// Sets a resource in the light and dark resource dictionaries.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set.</param>
    private static void SetResource(object key, object value) {
      lightResourceDictionary![key] = value;
      darkResourceDictionary![key] = value;
    }
  }
}