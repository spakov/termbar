using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// The Catppuccin palette.
    /// </summary>
    internal class Palette
    {
        /// <summary>
        /// The Catppuccin Latte flavor.
        /// </summary>
        [JsonPropertyName("latte")]
        public required Flavor Latte { get; set; }

        /// <summary>
        /// The Catppuccin Frappé flavor.
        /// </summary>
        [JsonPropertyName("frappe")]
        public required Flavor Frappe { get; set; }

        /// <summary>
        /// The Catppuccin Macchiato flavor.
        /// </summary>
        [JsonPropertyName("macchiato")]
        public required Flavor Macchiato { get; set; }

        /// <summary>
        /// The Catppuccin Mocha flavor.
        /// </summary>
        [JsonPropertyName("mocha")]
        public required Flavor Mocha { get; set; }

        /// <summary>
        /// Generates code.
        /// </summary>
        /// <param name="flavor">A <see cref="Flavor"/>.</param>
        /// <param name="indent">Indent with this value after the first
        /// line.</param>
        /// <returns>Generated code.</returns>
        internal static string ToCode(Flavor flavor, string indent)
        {
            StringBuilder code = new();

            code.AppendLine($"Colors = {flavor.Colors.ToCode(indent)},");
            code.Append($"{indent}AnsiColors = {flavor.AnsiColors.ToCode(indent)}");

            return code.ToString();
        }
    }
}
