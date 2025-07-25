using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A set of Catppuccin ANSI colors.
    /// </summary>
    internal class AnsiColors
    {
        /// <summary>
        /// The Catppuccin black ANSI color.
        /// </summary>
        [JsonPropertyName("black")]
        public required AnsiColorPair Black { get; set; }

        /// <summary>
        /// The Catppuccin red ANSI color.
        /// </summary>
        [JsonPropertyName("red")]
        public required AnsiColorPair Red { get; set; }

        /// <summary>
        /// The Catppuccin green ANSI color.
        /// </summary>
        [JsonPropertyName("green")]
        public required AnsiColorPair Green { get; set; }

        /// <summary>
        /// The Catppuccin yellow ANSI color.
        /// </summary>
        [JsonPropertyName("yellow")]
        public required AnsiColorPair Yellow { get; set; }

        /// <summary>
        /// The Catppuccin blue ANSI color.
        /// </summary>
        [JsonPropertyName("blue")]
        public required AnsiColorPair Blue { get; set; }

        /// <summary>
        /// The Catppuccin magenta ANSI color.
        /// </summary>
        [JsonPropertyName("magenta")]
        public required AnsiColorPair Magenta { get; set; }

        /// <summary>
        /// The Catppuccin cyan ANSI color.
        /// </summary>
        [JsonPropertyName("cyan")]
        public required AnsiColorPair Cyan { get; set; }

        /// <summary>
        /// The Catppuccin white ANSI color.
        /// </summary>
        [JsonPropertyName("white")]
        public required AnsiColorPair White { get; set; }

        /// <summary>
        /// Generates AnsiColors code.
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
            code.AppendLine($"{nextIndent}Black = {Black.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Red = {Red.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Green = {Green.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Yellow = {Yellow.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Blue = {Blue.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Magenta = {Magenta.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}Cyan = {Cyan.ToCode(nextIndent)},");
            code.AppendLine($"{nextIndent}White = {White.ToCode(nextIndent)}");
            code.Append($"{indent}}}");

            return code.ToString();
        }
    }
}
