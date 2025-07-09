using System.Text;
using System.Text.Json.Serialization;

namespace CatppuccinGenerator.Json {
  internal class Colors {
    [JsonPropertyName("rosewater")]
    public required Color Rosewater { get; set; }

    [JsonPropertyName("flamingo")]
    public required Color Flamingo { get; set; }

    [JsonPropertyName("pink")]
    public required Color Pink { get; set; }

    [JsonPropertyName("mauve")]
    public required Color Mauve { get; set; }

    [JsonPropertyName("red")]
    public required Color Red { get; set; }

    [JsonPropertyName("maroon")]
    public required Color Maroon { get; set; }

    [JsonPropertyName("peach")]
    public required Color Peach { get; set; }

    [JsonPropertyName("yellow")]
    public required Color Yellow { get; set; }

    [JsonPropertyName("green")]
    public required Color Green { get; set; }

    [JsonPropertyName("teal")]
    public required Color Teal { get; set; }

    [JsonPropertyName("sky")]
    public required Color Sky { get; set; }

    [JsonPropertyName("sapphire")]
    public required Color Sapphire { get; set; }

    [JsonPropertyName("blue")]
    public required Color Blue { get; set; }

    [JsonPropertyName("lavender")]
    public required Color Lavender { get; set; }

    [JsonPropertyName("text")]
    public required Color Text { get; set; }

    [JsonPropertyName("subtext1")]
    public required Color Subtext1 { get; set; }

    [JsonPropertyName("subtext0")]
    public required Color Subtext0 { get; set; }

    [JsonPropertyName("overlay2")]
    public required Color Overlay2 { get; set; }

    [JsonPropertyName("overlay1")]
    public required Color Overlay1 { get; set; }

    [JsonPropertyName("overlay0")]
    public required Color Overlay0 { get; set; }

    [JsonPropertyName("surface2")]
    public required Color Surface2 { get; set; }

    [JsonPropertyName("surface1")]
    public required Color Surface1 { get; set; }

    [JsonPropertyName("surface0")]
    public required Color Surface0 { get; set; }

    [JsonPropertyName("base")]
    public required Color Base { get; set; }

    [JsonPropertyName("mantle")]
    public required Color Mantle { get; set; }

    [JsonPropertyName("crust")]
    public required Color Crust { get; set; }

    /// <summary>
    /// Generates Colors code.
    /// </summary>
    /// <param name="indent">Indent with this value after the first
    /// line.</param>
    /// <returns>Generated code.</returns>
    internal string ToCode(string indent) {
      StringBuilder code = new();

      string nextIndent = $"{indent}  ";

      code.AppendLine("new() {");
      code.AppendLine($"{nextIndent}Rosewater = {Rosewater.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Flamingo = {Flamingo.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Pink = {Pink.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Mauve = {Mauve.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Red = {Red.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Maroon = {Maroon.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Peach = {Peach.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Yellow = {Yellow.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Green = {Green.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Teal = {Teal.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Sky = {Sky.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Sapphire = {Sapphire.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Blue = {Blue.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Lavender = {Lavender.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Text = {Text.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Subtext1 = {Subtext1.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Subtext0 = {Subtext0.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Overlay2 = {Overlay2.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Overlay1 = {Overlay1.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Overlay0 = {Overlay0.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Surface2 = {Surface2.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Surface1 = {Surface1.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Surface0 = {Surface0.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Base = {Base.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Mantle = {Mantle.ToCode(nextIndent)},");
      code.AppendLine($"{nextIndent}Crust = {Crust.ToCode(nextIndent)}");
      code.Append($"{indent}}}");

      return code.ToString();
    }
  }
}
