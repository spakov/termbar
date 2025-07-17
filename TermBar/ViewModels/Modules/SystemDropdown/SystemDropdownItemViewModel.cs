using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spakov.TermBar.ViewModels.Modules.SystemDropdown {
  /// <summary>
  /// The system dropdown item viewmodel.
  /// </summary>
  internal partial class SystemDropdownItemViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Configuration.Json.TermBar config;
    private readonly Configuration.Json.Modules.SystemDropdown moduleConfig;

    private string? _icon;
    private string? _name;

    /// <summary>
    /// The item icon.
    /// </summary>
    public string? Icon {
      get => _icon;

      set {
        if (_icon != value) {
          _icon = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// The item name.
    /// </summary>
    public string? Name {
      get => _name;

      set {
        if (_name != value) {
          _name = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a <see cref="SystemDropdownItemViewModel"/>.
    /// </summary>
    /// <param name="config"><inheritdoc
    /// cref="Views.Modules.SystemDropdown.SystemDropdownMenuFlyoutItemView.SystemDropdownMenuFlyoutItemView"
    /// path="/param[@name='config']"/></param>
    /// <param name="moduleConfig"><inheritdoc
    /// cref="Views.Modules.SystemDropdown.SystemDropdownMenuFlyoutItemView.SystemDropdownMenuFlyoutItemView"
    /// path="/param[@name='moduleConfig']"/></param>
    /// <param name="feature"><inheritdoc
    /// cref="Views.Modules.SystemDropdown.SystemDropdownMenuFlyoutItemView.SystemDropdownMenuFlyoutItemView"
    /// path="/param[@name='processId']"/></param>
    public SystemDropdownItemViewModel(Configuration.Json.TermBar config, Configuration.Json.Modules.SystemDropdown moduleConfig, Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures feature) {
      this.config = config;
      this.moduleConfig = moduleConfig;

      switch (feature) {
        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.SystemSettings:
          Icon = "\ue713";
          Name = App.ResourceLoader.GetString("SystemSettings");

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.SignOut:
          Icon = "\uf3b1";
          Name = App.ResourceLoader.GetString("SignOut");

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Lock:
          Icon = "\ue72e";
          Name = App.ResourceLoader.GetString("Lock");

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Sleep:
          Icon = "\ue708";
          Name = App.ResourceLoader.GetString("Sleep");

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.ShutDown:
          Icon = "\ue7e8";
          Name = App.ResourceLoader.GetString("ShutDown");

          break;

        case Configuration.Json.Modules.SystemDropdown.SystemDropdownFeatures.Restart:
          Icon = "\ue777";
          Name = App.ResourceLoader.GetString("Restart");

          break;
      }
    }

    private void OnPropertyChanged([CallerMemberName] string? callerMemberName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
  }
}