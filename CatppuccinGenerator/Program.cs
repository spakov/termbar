using CatppuccinGenerator;
using System.CommandLine;

internal class Program {
  private static int Main(string[] args) {
    Option<string?> outputPathOption = new("--outputPath", ["-o"]) {
      Description = "The generated code output path."
    };

    Option<string> namespaceOption = new("--namespace", ["-n"]) {
      Description = "The namespace to use for generated code.",
      Required = true
    };

    Option<bool> generateSDColorOption = new("--generateSDColor", ["-s"]) {
      Description = "Whether the generated Color should include SDColor."
    };

    Option<bool> generateWUIColorOption = new("--generateWUIColor", ["-w"]) {
      Description = "Whether the generated Color should include WUIColor."
    };

    Option<bool> generateSolidColorBrushOption = new("--generateSolidColorBrush", ["-b"]) {
      Description = "Whether the generated Color should include SolidColorBrush. Implies --generateWUIColor."
    };

    RootCommand rootCommand = new() {
      Description = "A Catppuccin palette.json code generator."
    };

    rootCommand.Options.Add(outputPathOption);
    rootCommand.Options.Add(namespaceOption);
    rootCommand.Options.Add(generateSDColorOption);
    rootCommand.Options.Add(generateWUIColorOption);
    rootCommand.Options.Add(generateSolidColorBrushOption);

    rootCommand.SetAction(parseResult => {
      return new Generator(
        parseResult.GetValue(outputPathOption),
        parseResult.GetValue(namespaceOption)!,
        parseResult.GetValue(generateSDColorOption),
        parseResult.GetValue(generateWUIColorOption) || parseResult.GetValue(generateSolidColorBrushOption),
        parseResult.GetValue(generateSolidColorBrushOption)
      ).Generate();
    });

    return rootCommand.Parse(args).Invoke();
  }
}