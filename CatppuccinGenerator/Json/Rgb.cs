using System.Text;
using System.Text.Json.Serialization;

namespace CatppuccinGenerator.Json {
  internal class Rgb {
    [JsonPropertyName("r")]
    public required int Red { get; set; }

    [JsonPropertyName("g")]
    public required int Green { get; set; }

    [JsonPropertyName("b")]
    public required int Blue { get; set; }

    /// <summary>
    /// Generates Rgb code.
    /// </summary>
    /// <param name="indent">Indent with this value after the first
    /// line.</param>
    /// <returns>Generated code.</returns>
    internal string ToCode(string indent) {
      StringBuilder code = new();

      string nextIndent = $"{indent}  ";

      code.AppendLine("new() {");
      code.AppendLine($"{nextIndent}Red = {Red},");
      code.AppendLine($"{nextIndent}Green = {Green},");
      code.AppendLine($"{nextIndent}Blue = {Blue}");
      code.Append($"{indent}}}");

      return code.ToString();
    }
  }
}
