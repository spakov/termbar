using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A Catppuccin flavor.
    /// </summary>
    internal class Flavor
    {
        /// <summary>
        /// The Catppuccin colors for this flavor.
        /// </summary>
        [JsonPropertyName("colors")]
        public required Colors Colors { get; set; }

        /// <summary>
        /// The Catppuccin ANSI colors for this flavor.
        /// </summary>
        [JsonPropertyName("ansiColors")]
        public required AnsiColors AnsiColors { get; set; }
    }
}
