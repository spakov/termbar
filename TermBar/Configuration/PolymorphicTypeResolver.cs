using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using TermBar.Configuration.Json;
using TermBar.Configuration.Json.Modules;

namespace TermBar.Configuration {
  /// <summary>
  /// Used to resolve module configuration to appropriate configuration types.
  /// </summary>
  internal class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver {
    private const string module = "$module";

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options) {
      JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

      Type baseClassType = typeof(IModule);

      if (jsonTypeInfo.Type == baseClassType) {
        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
          TypeDiscriminatorPropertyName = module,
          IgnoreUnrecognizedTypeDiscriminators = false,
          UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
          DerivedTypes = {
            new JsonDerivedType(typeof(Clock), typeof(Clock).Name),
            new JsonDerivedType(typeof(Cpu), typeof(Cpu).Name),
            new JsonDerivedType(typeof(Launcher), typeof(Launcher).Name),
            new JsonDerivedType(typeof(Memory), typeof(Memory).Name),
            new JsonDerivedType(typeof(StaticText), typeof(StaticText).Name),
            new JsonDerivedType(typeof(Json.Modules.Terminal), typeof(Json.Modules.Terminal).Name),
            new JsonDerivedType(typeof(Volume), typeof(Volume).Name),
            new JsonDerivedType(typeof(WindowBar), typeof(WindowBar).Name),
            new JsonDerivedType(typeof(WindowDropdown), typeof(WindowDropdown).Name)
          }
        };
      }

      return jsonTypeInfo;
    }
  }
}
