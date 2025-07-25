using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A Catppuccin color.
    /// </summary>
    internal class Color
    {
        /// <summary>
        /// The color name.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// The color RGB value.
        /// </summary>
        [JsonPropertyName("rgb")]
        public required Rgb Rgb { get; set; }

        /// <summary>
        /// Generates Color code.
        /// </summary>
        /// <param name="indent">Indent with this value after the first
        /// line.</param>
        /// <returns>Generated code.</returns>
        internal string ToCode(string indent)
        {
            StringBuilder code = new();

            string nextIndent = $"{indent}    ";

            code.AppendLine("new()");
            code.AppendLine($"{indent}{{");
            code.AppendLine($"{nextIndent}Name = \"{Name}\",");
            code.AppendLine($"{nextIndent}Rgb = {Rgb.ToCode(nextIndent)}");
            code.Append($"{indent}}}");

            return code.ToString();
        }
    }
}
