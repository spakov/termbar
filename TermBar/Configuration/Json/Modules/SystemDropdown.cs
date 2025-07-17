using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using System.Collections.Generic;

namespace Spakov.TermBar.Configuration.Json.Modules {
  [Description("A TermBar system dropdown configuration.")]
  internal class SystemDropdown : IModule {
    private const int orderDefault = int.MinValue;
    private const bool expandDefault = false;
    private const ColorEnum accentColorDefault = ColorEnum.Overlay1;
    private const string accentColorDefaultAsString = "Overlay1";
    private const string iconDefault = "";

    [Description("The order in which the module should be displayed on the TermBar.")]
    [DefaultIntNumber(orderDefault)]
    [MinimumInt(int.MinValue)]
    [MaximumInt(int.MaxValue)]
    public int Order { get; set; } = orderDefault;

    [Description("Whether the module should expand to take up as much space as possible.")]
    [DefaultBoolean(expandDefault)]
    public bool Expand { get; set; } = expandDefault;

    [Description("The Catppuccin color to use for the dropdown icon color.")]
    [DefaultString(accentColorDefaultAsString)]
    public ColorEnum AccentColor { get; set; } = accentColorDefault;

    [Description("The text to use as the dropdown icon.")]
    [DefaultString(iconDefault)]
    public string Icon { get; set; } = iconDefault;

    [Description("System menu features to display in the dropdown.")]
    public List<SystemDropdownFeatures> Features { get; set; } = [
      SystemDropdownFeatures.SystemSettings,
      SystemDropdownFeatures.SignOut,
      SystemDropdownFeatures.Lock,
      SystemDropdownFeatures.ShutDown,
      SystemDropdownFeatures.Restart
    ];

    /// <summary>
    /// Available system dropdown features.
    /// </summary>
    internal enum SystemDropdownFeatures {
      SystemSettings,
      SignOut,
      Lock,
      Sleep,
      ShutDown,
      Restart
    }
  }
}