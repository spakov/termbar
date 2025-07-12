#if DEBUG
using Microsoft.Extensions.Logging;
using Spakov.TermBar.Configuration.Json.SchemaAttributes;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
#endif
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Spakov.TermBar.Configuration;
using Spakov.TermBar.Configuration.Json;
using Spakov.TermBar.Models;
using Spakov.TermBar.Views;
using Spakov.TermBar.WindowManagement;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.Win32;

namespace Spakov.TermBar {
  /// <summary>
  /// Provides application-specific behavior to supplement the default
  /// Application class.
  /// </summary>
  public partial class App : Application {
#if DEBUG
    internal static readonly LogLevel logLevel = LogLevel.Debug;
#endif

    private Exception? criticalFailure;

    private ExceptionView? exceptionView;
    private WindowManagement.Windows.DialogWindow? exceptionDialogWindow;

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

      criticalFailure = null;
      InitializeComponent();
      WindowManager = new();

      ConfigHelper.JsonSerializerOptions.Encoder = JavaScriptEncoderJsonTartare.Instance;

      try {
        Config = ConfigHelper.Load(WindowManager);
      } catch (JsonException e) {
        criticalFailure = e;
        return;
      }

      DispatcherQueue = DispatcherQueue.GetForCurrentThread();

      if (Config.StartDirectory is not null) {
        string expandedStartDirectory = Environment.ExpandEnvironmentVariables(Config.StartDirectory);

        if (Directory.Exists(expandedStartDirectory)) {
          Directory.SetCurrentDirectory(expandedStartDirectory);
        }
      }

#if DEBUG
      if (Environment.CommandLine.Contains("GenerateSchema")) {
        JsonNode schema = ConfigHelper.JsonSerializerOptions.GetJsonSchemaAsNode(
          typeof(Config),
          new() {
            TreatNullObliviousAsNonNullable = true,
            TransformSchemaNode = SchemaAttributeTransformHelper.TransformSchemaNodeSchemaAttributes
          }
        );

        schema.AsObject().Insert(0, "$schema", "https://json-schema.org/draft/2020-12/schema");

        string filename = $"TermBar-{Assembly.GetExecutingAssembly().GetName().Version!.Major}.{Assembly.GetExecutingAssembly().GetName().Version!.Minor}-schema.json";

        File.WriteAllText(filename, schema.ToJsonString(ConfigHelper.JsonSerializerOptions));

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
    protected override void OnLaunched(LaunchActivatedEventArgs args) {
      if (criticalFailure is null) {
        WindowManager!.Display();
        UnhandledException += App_UnhandledException;
      } else {
        UnhandledExceptionHandler(criticalFailure);
      }
    }

    /// <summary>
    /// Invoked when an unhandled exception occurs.
    /// </summary>
    /// <param name="sender"><inheritdoc
    /// cref="UnhandledExceptionEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc
    /// cref="UnhandledExceptionEventHandler"
    /// path="/param[@name='e']"/></param>
    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) {
      criticalFailure = e.Exception;
      e.Handled = true;

      UnhandledExceptionHandler(criticalFailure);
    }

    /// <summary>
    /// A very nice method of displaying <paramref name="e"/>.
    /// </summary>
    /// <param name="e">An <see cref="Exception"/>.</param>
    private void UnhandledExceptionHandler(Exception e) {
      System.Diagnostics.Debug.WriteLine("Exception handler");

      if (WindowManager is null) {
        FallbackUnhandledExceptionHandler(e);

        return;
      }

      try {
        Config ??= new() {
          Displays = [
            new() {
            Id = WindowManager.Displays.Keys.ToList()[0],
            TermBar = new()
          }
          ]
        };

        if (Config.Displays[0].TermBar.Display is null) {
          Config.Displays[0].TermBar.Display = WindowManager.Displays[Config.Displays[0].Id];
        }

        exceptionView ??= new(Config.Displays[0].TermBar, e);

        if (exceptionDialogWindow is null) {
          exceptionDialogWindow = new(
            Config.Displays[0].TermBar,
            exceptionView
          );

          exceptionView.Owner = exceptionDialogWindow;
          exceptionDialogWindow.Closing += ExceptionDialogWindow_Closing;
          exceptionDialogWindow.Display();
        }
      } catch (Exception) {
        FallbackUnhandledExceptionHandler(e);
      }
    }

    /// <summary>
    /// A fallback method of displaying <paramref name="e"/>.
    /// </summary>
    /// <param name="e">An <see cref="Exception"/>.</param>
    private static void FallbackUnhandledExceptionHandler(Exception e) {
      PInvoke.MessageBox(
        Windows.Win32.Foundation.HWND.Null,
        $"Unhandled exception: {e}",
        "Unhandled Exception",
        Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK
      );
    }

    /// <summary>
    /// Invoked when the exception window is closing.
    /// </summary>
    private void ExceptionDialogWindow_Closing() {
      if (exceptionDialogWindow is not null) {
        exceptionDialogWindow.Closing -= ExceptionDialogWindow_Closing;
      }

      exceptionDialogWindow = null;
      exceptionView = null;

      Current.Exit();
    }
  }
}