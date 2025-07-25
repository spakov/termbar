using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.Configuration;
using Spakov.TermBar.Configuration.Json;
using Spakov.TermBar.Styles;
using System;
using System.Diagnostics;

namespace Spakov.TermBar.Views.Windows
{
    /// <summary>
    /// The main TermBar "window".
    /// </summary>
    internal sealed partial class TermBarWindow : Window
    {
        private readonly ILogger? _logger;

        private const string TaskManager = "taskmgr";

        private const string ViewsModulesNamespace = "Spakov.TermBar.Views.Modules";

        private readonly Configuration.Json.TermBar _config;

        private SettingsView? _settingsView;
        private WindowManagement.Windows.DialogWindow? _settingsDialogWindow;

        /// <summary>
        /// Initializes a <see cref="TermBarWindow"/>.
        /// </summary>
        /// <param name="config">A <see
        /// cref="Configuration.Json.TermBar"/>.</param>
        internal TermBarWindow(Configuration.Json.TermBar config) : base(config)
        {
            _logger = LoggerHelper.CreateLogger<TermBarWindow>();

            _config = config;

            ApplyComputedStyles();
            InitializeComponent();
            base.ApplyComputedStyles();

            Grid grid = new();
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

            Child = grid;

            if (config.Modules is null)
            {
                return;
            }

            config.Modules.Sort(new IModuleComparer());

            int moduleIndex = 0;

            foreach (IModule moduleConfig in config.Modules)
            {
                grid.ColumnDefinitions.Add(new() { Width = new(1, moduleConfig.Expand ? GridUnitType.Star : GridUnitType.Auto) });

                _logger?.LogTrace("Attempting to instantiate {view}", $"{ViewsModulesNamespace}.{moduleConfig.GetType().Name}.{moduleConfig.GetType().Name}View");
                (bool exceptionThrown, Type? targetType) = LoggerHelper.LogTry(
                    () => Type.GetType($"{ViewsModulesNamespace}.{moduleConfig.GetType().Name}.{moduleConfig.GetType().Name}View"),
                    "Could not GetType()",
                    _logger
                );

                if (exceptionThrown)
                {
                    continue;
                }

                if (targetType is null)
                {
                    _logger?.LogError("GetType() returned null");

                    throw new InvalidOperationException(string.Format(App.ResourceLoader.GetString("UnableToCreateInstance"), moduleConfig.GetType().Name));
                }

                (exceptionThrown, ModuleView? moduleView) = LoggerHelper.LogTry(
                    () => (ModuleView)Activator.CreateInstance(
                        type: targetType!,
                        bindingAttr: System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                        binder: null,
                        args: [config, moduleConfig],
                        culture: null,
                        activationAttributes: null
                    )!,
                    "Could not CreateInstance()",
                    _logger
                );

                if (exceptionThrown)
                {
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
        private new void ApplyComputedStyles()
        {
            Style gridStyle = new(typeof(Grid));

            StylesHelper.MergeWithAncestor(gridStyle, (Border)Content, typeof(Grid));

            gridStyle.Setters.Add(new Setter(Grid.PaddingProperty, $"{_config.Padding},0"));
            gridStyle.Setters.Add(new Setter(Grid.ColumnSpacingProperty, _config.Padding));

            Resources[typeof(Grid)] = gridStyle;
        }

        /// <summary>
        /// Starts the Task Manager.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void TaskManagerMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = TaskManager,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }

        /// <summary>
        /// Opens the TermBar settings window.
        /// </summary>
        /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc cref="RoutedEventHandler"
        /// path="/param[@name='e']"/></param>
        private void TermBarSettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            _settingsView ??= new(_config);

            if (_settingsDialogWindow is null)
            {
                _settingsDialogWindow = new(
                  _config,
                  _settingsView
                );

                _settingsView.Owner = _settingsDialogWindow;
                _settingsDialogWindow.Closing += SettingsDialogWindow_Closing;
                _settingsDialogWindow.Display();
            }
        }

        /// <summary>
        /// Prepares to display the settings window again.
        /// </summary>
        private void SettingsDialogWindow_Closing()
        {
            if (_settingsDialogWindow is not null)
            {
                _settingsDialogWindow.Closing -= SettingsDialogWindow_Closing;
            }

            _settingsDialogWindow = null;
            _settingsView = null;
        }
    }
}