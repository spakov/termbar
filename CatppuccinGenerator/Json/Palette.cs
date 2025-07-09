using System.Text;
using System.Text.Json.Serialization;

namespace CatppuccinGenerator.Json {
  internal class Palette {
    [JsonPropertyName("latte")]
    public required Flavor Latte { get; set; }

    [JsonPropertyName("frappe")]
    public required Flavor Frappe { get; set; }

    [JsonPropertyName("macchiato")]
    public required Flavor Macchiato { get; set; }

    [JsonPropertyName("mocha")]
    public required Flavor Mocha { get; set; }

    /// <summary>
    /// Generates code.
    /// </summary>
    /// <param name="flavor">A <see cref="Flavor"/>.</param>
    /// <param name="indent">Indent with this value after the first
    /// line.</param>
    /// <returns>Generated code.</returns>
    internal static string ToCode(Flavor flavor, string indent) {
      StringBuilder code = new();

      code.AppendLine($"Colors = {flavor.Colors.ToCode(indent)},");
      code.Append($"{indent}AnsiColors = {flavor.AnsiColors.ToCode(indent)}");

      return code.ToString();
    }
  }
}
