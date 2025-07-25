using Spakov.Catppuccin;
using Spakov.TermBar.Configuration.Json;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using Spakov.TermBar.WindowManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.RegularExpressions;
using Windows.Win32;

namespace Spakov.TermBar.Configuration
{
    /// <summary>
    /// Configuration helper methods.
    /// </summary>
    internal static partial class ConfigHelper
    {
        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        private const string ConfigFile = "config.json";

        /// <summary>
        /// The JSON Schema metaschema to reference in the configuration file.
        /// </summary>
        private const string Metaschema = "https://json-schema.org/draft/2020-12/schema";

        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new(ConfigContext.Default.Options);

        private static JsonArray? s_colorEnum;
        private static JsonArray? s_nullableColorEnum;
        private static JsonArray? s_ansiColorEnum;
        private static JsonArray? s_nullableAnsiColorEnum;

        /// <summary>
        /// The path to the configuration file.
        /// </summary>
        internal static string ConfigPath => $@"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\{ConfigFile}";

        /// <summary>
        /// The <see cref="System.Text.Json.JsonSerializerOptions"/>.
        /// </summary>
        internal static JsonSerializerOptions JsonSerializerOptions => s_jsonSerializerOptions;

        /// <summary>
        /// Loads an existing configuration file or creates a default one.
        /// </summary>
        /// <param name="windowManager"><inheritdoc cref="WindowManager"
        /// path="/summary"/></param>
        /// <returns>A <see cref="Config"/>.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Types are preserved, reflection required")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        internal static Config Load(WindowManager windowManager)
        {
            Config config;

            if (!File.Exists(ConfigPath))
            {
                config = new()
                {
                    Displays =
                    [
                        new()
                        {
                            Id = windowManager.Displays.Keys.ToList()[0],
                            TermBar = new()
                        }
                    ]
                };

                using StreamWriter streamWriter = new(ConfigPath);

                JsonSerializer.Serialize(
                    streamWriter.BaseStream,
                    config,
                    JsonSerializerOptions
                );
            }
            else
            {
                using StreamReader streamReader = new(ConfigPath);

                config = JsonSerializer.Deserialize(
                    streamReader.BaseStream,
                    ConfigContext.Default.Config
                )!;
            }

            foreach (Json.Display display in config.Displays)
            {
                display.TermBar.Display = windowManager.Displays[display.Id];
            }

            return config;
        }

        /// <summary>
        /// Generates the TermBar schema and writes it to the current working
        /// directory.
        /// </summary>
        internal static void GenerateSchema()
        {
            JsonNode schema = JsonSerializerOptions.GetJsonSchemaAsNode(
              typeof(Config),
              new()
              {
                  TreatNullObliviousAsNonNullable = true,
                  TransformSchemaNode = SchemaAttributeTransformHelper.TransformSchemaNodeSchemaAttributes
              }
            );

            EnhanceSchema(schema);

            string filename = $"TermBar-{Assembly.GetExecutingAssembly().GetName().Version!.Major}.{Assembly.GetExecutingAssembly().GetName().Version!.Minor}-schema.json";
            File.WriteAllText(filename, schema.ToJsonString(JsonSerializerOptions));

            PInvoke.MessageBox(
                Windows.Win32.Foundation.HWND.Null,
                string.Format(
                    App.ResourceLoader.GetString("SchemaHasBeenGenerated"),
                    Path.Combine(Directory.GetCurrentDirectory(), filename)
                ),
                App.ResourceLoader.GetString("TermBarSchemaGenerated"),
                Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK
            );
        }

        /// <summary>
        /// Enhances the schema by making several tweaks.
        /// </summary>
        /// <remarks>
        /// <para>Performs the following operations, intended to make the
        /// json-schema-for-humans-generated documentation prettier:</para>
        /// <list type="bullet">
        /// <item>Recursively inserts JSON Schema descriptions for
        /// <c>$module</c> objects.</item>
        /// <item>Marks <c>$module</c> as required.</item>
        /// <item>Adds a <c>$defs</c> object to the root object.</item>
        /// <item>References the <c>$defs</c> in module objects.</item>
        /// <item>Flattens all color enums and ANSI color enums into a single
        /// <c>$defs</c> object.</item>
        /// </list>
        /// </remarks>
        /// <param name="jsonNode">The <see cref="JsonNode"/> at which to begin
        /// replacing. Invoke with the schema root.</param>
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Primitive types only")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        private static void EnhanceSchema(JsonNode? jsonNode)
        {
            // Build enums
            if (s_colorEnum is null && s_nullableColorEnum is null)
            {
                s_colorEnum = [];
                s_nullableColorEnum = [];

                foreach (string color in Enum.GetNames(typeof(ColorEnum)))
                {
                    s_colorEnum.Add(color);
                    s_nullableColorEnum.Add(color);
                }

                s_nullableColorEnum.Add(null);
            }

            if (s_ansiColorEnum is null && s_nullableAnsiColorEnum is null)
            {
                s_ansiColorEnum = [];
                s_nullableAnsiColorEnum = [];

                foreach (string color in Enum.GetNames(typeof(AnsiColorEnum)))
                {
                    s_ansiColorEnum.Add(color);
                    s_nullableAnsiColorEnum.Add(color);
                }

                s_nullableAnsiColorEnum.Add(null);
            }

            List<JsonNode> toReplace = [];

            // Check objects
            if (jsonNode is JsonObject jsonObject)
            {
                if (jsonObject.Parent is null)
                {
                    // Add title and metaschema to root object
                    jsonObject.Insert(0, "title", ConfigFile);
                    jsonObject.Insert(0, "$schema", Metaschema);

                    // Add $defs to root object
                    jsonObject.Add(
                        "$defs",
                        new JsonObject()
                        {
                            {
                                nameof(s_colorEnum),
                                new JsonObject()
                                {
                                    ["enum"] = s_colorEnum
                                }
                            },
                            {
                                nameof(s_nullableColorEnum),
                                new JsonObject()
                                {
                                    ["enum"] = s_nullableColorEnum
                                }
                            },
                            {
                                nameof(s_ansiColorEnum),
                                new JsonObject()
                                {
                                    ["enum"] = s_ansiColorEnum
                                }
                            },
                            {
                                nameof(s_nullableAnsiColorEnum),
                                new JsonObject()
                                {
                                    ["enum"] = s_nullableAnsiColorEnum
                                }
                            },
                            { "Clock module", new JsonObject() },
                            { "Cpu module", new JsonObject() },
                            { "Gpu module", new JsonObject() },
                            { "Launcher module", new JsonObject() },
                            { "Memory module", new JsonObject() },
                            { "StaticText module", new JsonObject() },
                            { "SystemDropdown module", new JsonObject() },
                            { "Terminal module", new JsonObject() },
                            { "Volume module", new JsonObject() },
                            { "WindowBar module", new JsonObject() },
                            { "WindowDropdown module", new JsonObject() }
                        }
                    );
                }

                // Recursively visit object children
                foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
                {
                    EnhanceSchema(property.Value);
                }

                // Note enums to be flattened
                if (jsonObject.TryGetPropertyValue("enum", out JsonNode? enumNode) && enumNode is JsonArray enumArray)
                {
                    // Do not flatten our $defs enums
                    if (enumArray.Parent?.Parent?.GetPropertyName() != "$defs")
                    {
                        if (
                            JsonNode.DeepEquals(enumArray, s_colorEnum)
                            || JsonNode.DeepEquals(enumArray, s_nullableColorEnum)
                            || JsonNode.DeepEquals(enumArray, s_ansiColorEnum)
                            || JsonNode.DeepEquals(enumArray, s_nullableAnsiColorEnum)
                        )
                        {
                            toReplace.Add(enumArray);
                        }
                    }
                }

                string jsonObjectPath = jsonObject.GetPath();

                // Set description for $module objects
                if (jsonObjectPath.EndsWith(".$module"))
                {
                    jsonObject.Insert(0, "description", "The module type.");
                } // Add definition references in modules
                else if (ModuleObjectRegex().IsMatch(jsonObjectPath))
                {
                    if (jsonObject.TryGetPropertyValue("properties", out JsonNode? propertiesNode))
                    {
                        if (propertiesNode is JsonObject propertiesObject && propertiesObject.TryGetPropertyValue("$module", out JsonNode? moduleNode))
                        {
                            if (moduleNode is JsonObject moduleObject && moduleObject.TryGetPropertyValue("const", out JsonNode? constNode))
                            {
                                if (constNode is JsonValue constValue)
                                {
                                    // Make $module required
                                    if (!jsonObject.TryGetPropertyValue("required", out JsonNode? requiredNode))
                                    {
                                        requiredNode = new JsonArray();
                                        jsonObject.Add("required", requiredNode);
                                    }

                                    if (requiredNode is JsonArray requiredArray)
                                    {
                                        requiredArray.Add("$module");
                                    }

                                    // Add definition reference
                                    jsonObject.Add("$ref", $"#/$defs/{constValue} module");
                                }
                            }
                        }
                    }
                }

                // Check arrays
            }
            else if (jsonNode is JsonArray jsonArray)
            {
                // Recursively visit array children
                foreach (JsonNode? property in jsonArray)
                {
                    EnhanceSchema(property);
                }
            }

            // Flatten enums
            foreach (JsonNode replaceJsonNode in toReplace)
            {
                if (replaceJsonNode.Parent is JsonObject replaceParentJsonObject)
                {
                    string? replacementRef = null;

                    if (JsonNode.DeepEquals(replaceJsonNode, s_colorEnum))
                    {
                        replacementRef = nameof(s_colorEnum);
                    }
                    else if (JsonNode.DeepEquals(replaceJsonNode, s_nullableColorEnum))
                    {
                        replacementRef = nameof(s_nullableColorEnum);
                    }
                    else if (JsonNode.DeepEquals(replaceJsonNode, s_ansiColorEnum))
                    {
                        replacementRef = nameof(s_ansiColorEnum);
                    }
                    else if (JsonNode.DeepEquals(replaceJsonNode, s_nullableAnsiColorEnum))
                    {
                        replacementRef = nameof(s_nullableAnsiColorEnum);
                    }

                    if (replacementRef is null)
                    {
                        continue;
                    }

                    int index = replaceParentJsonObject.IndexOf("enum");
                    replaceParentJsonObject.Remove("enum");
                    replaceParentJsonObject.Insert(index, "$ref", $"#/$defs/{replacementRef}");
                }
            }
        }

        /// <summary>
        /// A regular expression that matches a module schema.
        /// </summary>
        /// <returns>A <see cref="Regex"/>.</returns>
        [GeneratedRegex(@"\.Modules\.items\.anyOf\[[0-9]+\]$")]
        private static partial Regex ModuleObjectRegex();
    }
}