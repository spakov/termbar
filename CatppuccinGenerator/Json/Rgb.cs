using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A red-green-blue triplet.
    /// </summary>
    internal class Rgb
    {
        /// <summary>
        /// The red value.
        /// </summary>
        [JsonPropertyName("r")]
        public required int Red { get; set; }

        /// <summary>
        /// The green value.
        /// </summary>
        [JsonPropertyName("g")]
        public required int Green { get; set; }

        /// <summary>
        /// The blue value.
        /// </summary>
        [JsonPropertyName("b")]
        public required int Blue { get; set; }

        /// <summary>
        /// Generates Rgb code.
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
            code.AppendLine($"{nextIndent}Red = {Red},");
            code.AppendLine($"{nextIndent}Green = {Green},");
            code.AppendLine($"{nextIndent}Blue = {Blue}");
            code.Append($"{indent}}}");

            return code.ToString();
        }
    }
}
