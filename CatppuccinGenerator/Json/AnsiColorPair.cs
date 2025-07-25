using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A Catppuccin ANSI color pair.
    /// </summary>
    internal class AnsiColorPair
    {
        /// <summary>
        /// The normal Catppuccin ANSI color.
        /// </summary>
        [JsonPropertyName("normal")]
        public required Color Normal { get; set; }

        /// <summary>
        /// The bright Catppuccin ANSI color.
        /// </summary>
        [JsonPropertyName("bright")]
        public required Color Bright { get; set; }

        /// <summary>
        /// Generates AnsiColorPair code.
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
            code.AppendLine($"{nextIndent}Normal = {Normal.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Bright = {Bright.ToCode(nextIndent)}");
            code.Append($"{indent}}}");

            return code.ToString();
        }
    }
}
