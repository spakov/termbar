using Spakov.TermBar.Models;
using Spakov.TermBar.ViewModels.Modules.SystemDropdown;
using System;
using System.Diagnostics;

namespace Spakov.TermBar.Views.Modules.SystemDropdown {
  /// <summary>
  /// The TermBar window dropdown.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812:Class is apparently never instantiated", Justification = "Instantiated with Activator.CreateInstance()")]
  internal sealed partial class SystemDropdownView : ModuleView {
    private const string explorer = "explorer";
    private const string msSettings = "ms-settings:";

    private SystemDropdownViewModel? viewModel;

    /// <summary>
    /// The viewmodel.
    /// </summary>
    private SystemDropdownViewModel? ViewModel {
      get => viewModel;
      set {
        viewModel = value;
        DataContext = viewModel;
      }
    }

    /// <summary>
    /// <inheritdoc cref="ModuleView.ModuleView"
    /// path="/param[@name='config']"/>
    /// </summary>
    private Configuration.Json.TermBar? Config { get; init; }

    /// <summary>
    /// The <see cref="Action{T}"/> to invoke when the user clicks a menu item.
    /// </summary>
    private Action<SystemDropdownMenuFlyoutItemView>? ClickAction { get; init; }

    /// <summary>
    /// Initializes a <see cref="SystemDropdownView"/>.
    /// </summary>
    /// <param name="config"><inheritdoc cref="Config"
    /// path="/summary"/></param>
    /// <param name="moduleConfig">The <see
    /// cref="Configuration.Json.Modules.WindowBar"/> for this <see
    /// cref="WindowBarView"/>.</param>
    internal SystemDropdownView(Configuration.Json.TermBar config, Configuration.Json.Modules.SystemDropdown moduleConfig) : base(config, moduleConfig, skipColor: true) {
      Config = config;
      ClickAction = ItemClicked;
      ViewModel = new SystemDropdownViewModel(config, moduleConfig);

      InitializeComponent();
    }

    /// <summary>
    /// Invoked when the user clicks a system dropdown menu item.
    /// </summary>
    /// <param name="item">The menu item that was clicked.</param>
    private void ItemClicked(SystemDropdownMenuFlyoutItemView item) {
      switch (item.Feature) {
        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.SystemSettings:
          Process.Start(
            new ProcessStartInfo() {
              FileName = explorer,
              Arguments = msSettings,
              UseShellExecute = true
            }
          );

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.SignOut:
          if (Shutdown.Initiate(Shutdown.ShutdownTypes.Logoff)) {
            App.Current.Exit();
          }

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Lock:
          Shutdown.Initiate(Shutdown.ShutdownTypes.Lock);

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Sleep:
          Shutdown.Initiate(Shutdown.ShutdownTypes.Suspend);

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.ShutDown:
          if (Shutdown.Initiate(Shutdown.ShutdownTypes.PowerOff)) {
            App.Current.Exit();
          } else {
            throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
          }

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Restart:
          if (Shutdown.Initiate(Shutdown.ShutdownTypes.Reboot)) {
            App.Current.Exit();
          }

          break;
      }
    }
  }
}