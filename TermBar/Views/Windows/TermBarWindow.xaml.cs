#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.Configuration;
using Spakov.TermBar.Configuration.Json;
using Spakov.TermBar.Styles;
using System;
using System.Diagnostics;

namespace Spakov.TermBar.Views.Windows {
  /// <summary>
  /// The main TermBar "window".
  /// </summary>
  internal sealed partial class TermBarWindow : Window {
#if DEBUG
    internal new readonly ILogger logger;
    internal new static readonly LogLevel logLevel = App.logLevel;
#endif

    private const string taskManager = "taskmgr.exe";

    private const string viewsModulesNamespace = "TermBar.Views.Modules";

    private readonly Configuration.Json.TermBar config;

    /// <summary>
    /// Initializes a <see cref="TermBarWindow"/>.
    /// </summary>
    /// <param name="config">A <see cref="Configuration.Json.TermBar"/>.</param>
    internal TermBarWindow(Configuration.Json.TermBar config) : base(config) {
#if DEBUG
      using ILoggerFactory factory = LoggerFactory.Create(
        builder => {
          builder.AddDebug();
          builder.SetMinimumLevel(logLevel);
        }
      );

      logger = factory.CreateLogger<TermBarWindow>();
#endif

      this.config = config;

      ApplyComputedStyles();
      InitializeComponent();
      base.ApplyComputedStyles();

      Grid grid = new();
      grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

      Child = grid;

      if (config.Modules is null) return;

      config.Modules.Sort(new IModuleComparer());

      int moduleIndex = 0;

      foreach (IModule moduleConfig in config.Modules) {
        grid.ColumnDefinitions.Add(new() { Width = new(1, moduleConfig.Expand ? GridUnitType.Star : GridUnitType.Auto) });

        Type? targetType;

#if DEBUG
        logger.LogDebug("Attempting to instantiate {view}", $"{viewsModulesNamespace}.{moduleConfig.GetType().Name}.{moduleConfig.GetType().Name}View");
#endif

        try {
          targetType = Type.GetType($"{viewsModulesNamespace}.{moduleConfig.GetType().Name}.{moduleConfig.GetType().Name}View");
        } catch (Exception) {
#if DEBUG
          logger.LogDebug("Could not GetType()");
#endif

          continue;
        }

        if (targetType is null) {
#if DEBUG
          logger.LogDebug("GetType() returned null");
#endif

          continue;
        }

        ModuleView moduleView;

        try {
          moduleView = (ModuleView) Activator.CreateInstance(
            type: targetType,
            bindingAttr: System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null,
            args: [config, moduleConfig],
            culture: null,
            activationAttributes: null
          )!;
        } catch (Exception) {
#if DEBUG
          logger.LogDebug("Could not CreateInstance()");
#endif

          continue;
        }

        Grid.SetRow(moduleView, 0);
        Grid.SetColumn(moduleView, moduleIndex++);
        grid.Children.Add(moduleView);
      }
    }

    /// <summary>
    /// Applies computed styles to the window.
    /// </summary>
    private new void ApplyComputedStyles() {
      Style gridStyle = new(typeof(Grid));

      StylesHelper.MergeWithAncestor(gridStyle, (Border) Content, typeof(Grid));

      gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, $"{config.Padding},0"));
      gridStyle.Setters.Add(new Setter(Grid.ColumnSpacingProperty, config.Padding));

      Resources[typeof(Grid)] = gridStyle;
    }

    /// <summary>
    /// Starts the Task Manager.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void TaskManagerMenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
      ProcessStartInfo processStartInfo = new() {
        FileName = taskManager,
        UseShellExecute = true
      };

      Process.Start(processStartInfo);
    }

    /// <summary>
    /// Opens the TermBar settings.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void TermBarSettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
      SettingsView settingsView = new(config);

      WindowManagement.Windows.DialogWindow dialogWindow = new(
        config,
        settingsView
      );

      settingsView.Owner = dialogWindow;
      dialogWindow.Display();
    }
  }
}