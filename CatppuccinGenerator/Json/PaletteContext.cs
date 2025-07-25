using System.Text.Json.Serialization;

namespace Spakov.CatppuccinGenerator.Json
{
    /// <summary>
    /// A <see cref="JsonSerializerContext"/> for reading <c>palette.json</c>
    /// via generated code.
    /// </summary>
    [JsonSerializable(typeof(Palette))]
    internal partial class PaletteContext : JsonSerializerContext
    {
    }
}
