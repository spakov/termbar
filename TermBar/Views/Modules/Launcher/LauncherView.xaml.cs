using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Spakov.Catppuccin;
using Spakov.TermBar.ViewModels.Modules.Launcher;
using System;
using System.Diagnostics;

namespace Spakov.TermBar.Views.Modules.Launcher {
  /// <summary>
  /// The TermBar launcher.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
  internal sealed partial class LauncherView : ModuleView {
    private LauncherViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private LauncherViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// Initializes a <see cref="LauncherView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.Launcher"/> for this <see
    /// cref="LauncherView"/>.</param>
    internal LauncherView(Configuration.Json.TermBar config, Configuration.Json.Modules.Launcher moduleConfig) : base(config, moduleConfig) {
      ViewModel = new LauncherViewModel(config, moduleConfig);
      DataContext = ViewModel;

      InitializeComponent();
      ApplyComputedStyles(config);
    }

    /// <summary>
    /// Applies computed styles to the <see cref="ListView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc
    /// cref="Configuration.Json.TermBar"
    /// path="/param[@name='config']"/></param>
    private void ApplyComputedStyles(Configuration.Json.TermBar config) {
      Style textBlockStyle = new(typeof(TextBlock));

      textBlockStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(config.FontFamily)));
      textBlockStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, config.FontSize));

      textBlockStyle.Setters.Add(
        new Setter(
          TextBlock.ForegroundProperty,
          Palette.Instance[config.Flavor].Colors[config.TextColor].SolidColorBrush
        )
      );

      ListView.Resources[typeof(TextBlock)] = textBlockStyle;
    }

    /// <summary>
    /// Invoked when a launcher button is clicked.
    /// </summary>
    /// <param name="sender"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='sender']"/></param>
    /// <param name="e"><inheritdoc cref="RoutedEventHandler"
    /// path="/param[@name='e']"/></param>
    private void Button_Click(object sender, RoutedEventArgs e) {
      Button button = (Button) sender;
      (string? command, string[]? commandArguments) = (ValueTuple<string?, string[]?>) button.Tag;

      if (command is null) return;

      command = Environment.ExpandEnvironmentVariables(command);
      commandArguments ??= [];

      ProcessStartInfo processStartInfo = new() {
        FileName = command,
        UseShellExecute = true
      };

      foreach (string commandArgument in commandArguments) {
        processStartInfo.ArgumentList.Add(commandArgument);
      }

      Process.Start(processStartInfo);
    }
  }
}