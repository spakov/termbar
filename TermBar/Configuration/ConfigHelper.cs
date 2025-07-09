using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TermBar.Configuration.Json;
using TermBar.WindowManagement;

namespace TermBar.Configuration {
  /// <summary>
  /// Configuration helper methods.
  /// </summary>
  internal static class ConfigHelper {
    private const string configFile = "config.json";

    /*private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
      WriteIndented = true,
      PropertyNameCaseInsensitive = true,
      Converters = { new JsonStringEnumConverter() }
    };*/

    /// <summary>
    /// The path to the configuration file.
    /// </summary>
    internal static string ConfigPath => $@"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\{configFile}";

    /// <summary>
    /// JSON serializer options.
    /// </summary>
    //internal static JsonSerializerOptions JsonSerializerOptions => jsonSerializerOptions;

    /// <summary>
    /// Loads an existing configuration file or creates a default one.
    /// </summary>
    /// <param name="windowManager"><inheritdoc cref="WindowManager"
    /// path="/summary"/></param>
    /// <returns>A <see cref="Config"/>.</returns>
    internal static Config Load(WindowManager windowManager) {
      Config config;

      if (!File.Exists(ConfigPath)) {
        config = new() {
          Displays = [
            new() {
              Id = windowManager.Displays.Keys.ToList()[0],
              TermBar = new()
            }
          ]
        };

        using StreamWriter streamWriter = new(ConfigPath);

        JsonSerializer.Serialize(
          streamWriter.BaseStream,
          config,
          ConfigContext.Default.Config
        );
      } else {
        using StreamReader streamReader = new(ConfigPath);

        config = JsonSerializer.Deserialize(
          streamReader.BaseStream,
          ConfigContext.Default.Config
        )!;
      }

      foreach (Json.Display display in config.Displays) {
        display.TermBar.Display = windowManager.Displays[display.Id];
      }

      return config;
    }
  }
}
