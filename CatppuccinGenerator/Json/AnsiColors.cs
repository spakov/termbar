using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    internal class AnsiColors
    {
        [JsonPropertyName("black")]
        public required AnsiColorPair Black { get; set; }

        [JsonPropertyName("red")]
        public required AnsiColorPair Red { get; set; }

        [JsonPropertyName("green")]
        public required AnsiColorPair Green { get; set; }

        [JsonPropertyName("yellow")]
        public required AnsiColorPair Yellow { get; set; }

        [JsonPropertyName("blue")]
        public required AnsiColorPair Blue { get; set; }

        [JsonPropertyName("magenta")]
        public required AnsiColorPair Magenta { get; set; }

        [JsonPropertyName("cyan")]
        public required AnsiColorPair Cyan { get; set; }

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
