using System.Text;
using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// The Catppuccin flavor colors.
    /// </summary>
    internal class Colors
    {
        /// <summary>
        /// The Catppuccin rosewater color.
        /// </summary>
        [JsonPropertyName("rosewater")]
        public required Color Rosewater { get; set; }

        /// <summary>
        /// The Catppuccin flamingo color.
        /// </summary>
        [JsonPropertyName("flamingo")]
        public required Color Flamingo { get; set; }

        /// <summary>
        /// The Catppuccin pink color.
        /// </summary>
        [JsonPropertyName("pink")]
        public required Color Pink { get; set; }

        /// <summary>
        /// The Catppuccin mauve color.
        /// </summary>
        [JsonPropertyName("mauve")]
        public required Color Mauve { get; set; }

        /// <summary>
        /// The Catppuccin red color.
        /// </summary>
        [JsonPropertyName("red")]
        public required Color Red { get; set; }

        /// <summary>
        /// The Catppuccin maroon color.
        /// </summary>
        [JsonPropertyName("maroon")]
        public required Color Maroon { get; set; }

        /// <summary>
        /// The Catppuccin peach color.
        /// </summary>
        [JsonPropertyName("peach")]
        public required Color Peach { get; set; }

        /// <summary>
        /// The Catppuccin yellow color.
        /// </summary>
        [JsonPropertyName("yellow")]
        public required Color Yellow { get; set; }

        /// <summary>
        /// The Catppuccin green color.
        /// </summary>
        [JsonPropertyName("green")]
        public required Color Green { get; set; }

        /// <summary>
        /// The Catppuccin teal color.
        /// </summary>
        [JsonPropertyName("teal")]
        public required Color Teal { get; set; }

        /// <summary>
        /// The Catppuccin sky color.
        /// </summary>
        [JsonPropertyName("sky")]
        public required Color Sky { get; set; }

        /// <summary>
        /// The Catppuccin sapphire color.
        /// </summary>
        [JsonPropertyName("sapphire")]
        public required Color Sapphire { get; set; }

        /// <summary>
        /// The Catppuccin blue color.
        /// </summary>
        [JsonPropertyName("blue")]
        public required Color Blue { get; set; }

        /// <summary>
        /// The Catppuccin lavender color.
        /// </summary>
        [JsonPropertyName("lavender")]
        public required Color Lavender { get; set; }

        /// <summary>
        /// The Catppuccin text color.
        /// </summary>
        [JsonPropertyName("text")]
        public required Color Text { get; set; }

        /// <summary>
        /// The Catppuccin subtext color 1.
        /// </summary>
        [JsonPropertyName("subtext1")]
        public required Color Subtext1 { get; set; }

        /// <summary>
        /// The Catppuccin subtext color 0.
        /// </summary>
        [JsonPropertyName("subtext0")]
        public required Color Subtext0 { get; set; }

        /// <summary>
        /// The Catppuccin overlay color 2.
        /// </summary>
        [JsonPropertyName("overlay2")]
        public required Color Overlay2 { get; set; }

        /// <summary>
        /// The Catppuccin overlay color 1.
        /// </summary>
        [JsonPropertyName("overlay1")]
        public required Color Overlay1 { get; set; }

        /// <summary>
        /// The Catppuccin overlay color 0.
        /// </summary>
        [JsonPropertyName("overlay0")]
        public required Color Overlay0 { get; set; }

        /// <summary>
        /// The Catppuccin surface color 2.
        /// </summary>
        [JsonPropertyName("surface2")]
        public required Color Surface2 { get; set; }

        /// <summary>
        /// The Catppuccin surface color 1.
        /// </summary>
        [JsonPropertyName("surface1")]
        public required Color Surface1 { get; set; }

        /// <summary>
        /// The Catppuccin surface color 0.
        /// </summary>
        [JsonPropertyName("surface0")]
        public required Color Surface0 { get; set; }

        /// <summary>
        /// The Catppuccin base color.
        /// </summary>
        [JsonPropertyName("base")]
        public required Color Base { get; set; }

        /// <summary>
        /// The Catppuccin mantle color.
        /// </summary>
        [JsonPropertyName("mantle")]
        public required Color Mantle { get; set; }

        /// <summary>
        /// The Catppuccin crust color.
        /// </summary>
        [JsonPropertyName("crust")]
        public required Color Crust { get; set; }

        /// <summary>
        /// Generates Colors code.
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
