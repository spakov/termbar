using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    internal class AnsiColorPair
    {
        [JsonPropertyName("normal")]
        public required Color Normal { get; set; }

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
