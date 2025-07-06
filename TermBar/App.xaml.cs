using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using TermBar.Configuration;
using TermBar.Configuration.Json;
using TermBar.Models;
using TermBar.WindowManagement;

namespace TermBar {
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : Application {
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
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
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

      InitializeModels();
    }

    /// <summary>
    /// Initializes singleton intance models when they are required to use
    /// the UI thread.
    /// </summary>
    private static void InitializeModels() => Volume.Instance = new Volume(DispatcherQueue!);

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args) => WindowManager!.Display();
  }
}
