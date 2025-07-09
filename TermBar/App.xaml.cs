#if DEBUG
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using TermBar.Configuration.Json.SchemaAttributes;
using Windows.Win32;
#endif
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using TermBar.Configuration;
using TermBar.Configuration.Json;
using TermBar.Models;
using TermBar.WindowManagement;
using System.Text.Json;

namespace TermBar {
  /// <summary>
  /// Provides application-specific behavior to supplement the default
  /// Application class.
  /// </summary>
  public partial class App : Application {
#if DEBUG
    internal static readonly LogLevel logLevel = LogLevel.Debug;
#endif

    /// <summary>
    /// The window manager.
    /// </summary>
    internal static WindowManager? WindowManager { get; private set; }

    /// <summary>
    /// The TermBar configuration.
    /// </summary>
    internal static Config? Config { get; private set; }

    /// <summary>
    /// The dispatcher queue for the UI thread.
    /// </summary>
    internal static DispatcherQueue? DispatcherQueue { get; private set; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line
    /// of authored code executed, and as such is the logical equivalent of
    /// main() or WinMain().
    /// </summary>
    public App() {
      TrimRoots.PreserveTrimmableClasses();

      InitializeComponent();

      WindowManager = new();
      Config = ConfigHelper.Load(WindowManager);

      DispatcherQueue = DispatcherQueue.GetForCurrentThread();

      if (Config.StartDirectory is not null) {
        string expandedStartDirectory = Environment.ExpandEnvironmentVariables(Config.StartDirectory);

        if (Directory.Exists(expandedStartDirectory)) {
          Directory.SetCurrentDirectory(expandedStartDirectory);
        }
      }

#if DEBUG
      if (Environment.CommandLine.Contains("GenerateSchema")) {
        JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        JsonNode schema = jsonSerializerOptions.GetJsonSchemaAsNode(
          typeof(Config),
          new() {
            TreatNullObliviousAsNonNullable = true,
            TransformSchemaNode = SchemaAttributeTransformHelper.TransformSchemaNodeSchemaAttributes
          }
        );

        schema.AsObject().Insert(0, "$schema", "https://json-schema.org/draft/2020-12/schema");

        string filename = $"TermBar-{Assembly.GetExecutingAssembly().GetName().Version!.Major}.{Assembly.GetExecutingAssembly().GetName().Version!.Minor}-schema.json";

        File.WriteAllText(filename, schema.ToString());

        PInvoke.MessageBox(
          Windows.Win32.Foundation.HWND.Null,
          $"Schema has been generated at {Path.Combine(Directory.GetCurrentDirectory(), filename)}.",
          "TermBar Schema Generated",
          Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK
        );

        Current.Exit();
      }
#endif

      InitializeModels();
    }

    /// <summary>
    /// Initializes singleton intance models when they are required to use
    /// the UI thread.
    /// </summary>
    private static void InitializeModels() {
      Models.WindowList.Instance = new Models.WindowList(DispatcherQueue!);
      Volume.Instance = new Volume(DispatcherQueue!);
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and
    /// process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args) => WindowManager!.Display();
  }
}
