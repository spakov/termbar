using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json.Modules;
using System.Text.Json.Serialization;

namespace Spakov.TermBar.Configuration.Json {
  /// <summary>
  /// The interface that all modules implement.
  /// </summary>
  [JsonPolymorphic(TypeDiscriminatorPropertyName = "$module")]
  [JsonDerivedType(typeof(Clock), "Clock")]
  [JsonDerivedType(typeof(Cpu), "Cpu")]
  [JsonDerivedType(typeof(Gpu), "Gpu")]
  [JsonDerivedType(typeof(Launcher), "Launcher")]
  [JsonDerivedType(typeof(Memory), "Memory")]
  [JsonDerivedType(typeof(StaticText), "StaticText")]
  [JsonDerivedType(typeof(SystemDropdown), "SystemDropdown")]
  [JsonDerivedType(typeof(Modules.Terminal), "Terminal")]
  [JsonDerivedType(typeof(Volume), "Volume")]
  [JsonDerivedType(typeof(WindowBar), "WindowBar")]
  [JsonDerivedType(typeof(WindowDropdown), "WindowDropdown")]
  internal interface IModule {
    /// <summary>
    /// The order in which the module should be displayed.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Whether the module should expand to take up as much space as possible.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// The Catppuccin color to use as an accent color.
    /// </summary>
    public ColorEnum AccentColor { get; set; }
  }
}