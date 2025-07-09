using System.Text;
using System.Text.Json.Serialization;

namespace CatppuccinGenerator.Json {
  internal class Flavor {
    [JsonPropertyName("colors")]
    public required Colors Colors { get; set; }

    [JsonPropertyName("ansiColors")]
    public required AnsiColors AnsiColors { get; set; }
  }
}
