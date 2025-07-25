using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Spakov.TermBar.Models;
using Spakov.TermBar.Styles;
using Spakov.TermBar.ViewModels.Modules.WindowBar;
using Windows.Win32.Foundation;

namespace Spakov.TermBar.Views.Modules.WindowBar
{
    /// <summary>
    /// A TermBar window bar window view.
    /// </summary>
    internal sealed partial class WindowBarWindowView : ModuleView, IWindowListWindow
    {
        private WindowBarWindowViewModel? _viewModel;

        private readonly uint _windowProcessId;

        /// <summary>
        /// The viewmodel.
        /// </summary>
        private WindowBarWindowViewModel? ViewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        public uint WindowProcessId => _windowProcessId;

        public string? WindowName
        {
            get => _viewModel?.Name;

            set
            {
                if (_viewModel is not null)
                {
                    _viewModel.Name = value;
                }
            }
        }

        /// <summary>
        /// The window's <see cref="HWND"/>.
        /// </summary>
        internal HWND HWnd { get; private set; }

        /// <summary>
        /// Initializes a <see cref="WindowBarWindowView"/>.
        /// </summary>
        /// <param name="config"><inheritdoc cref="ModuleView.ModuleView"
        /// path="/param[@name='config']"/></param>
        /// <param name="moduleConfig">The <see
        /// cref="Configuration.Json.Modules.WindowBar"/> for this <see
        /// cref="WindowBarWindowView"/>.</param>
        /// <param name="hWnd"><inheritdoc cref="HWnd"
        /// path="/summary"/></param>
        /// <param name="processId">The window's owning process ID.</param>
        /// <param name="name">The window's name.</param>
        internal WindowBarWindowView(Configuration.Json.TermBar config, Configuration.Json.Modules.WindowBar moduleConfig, HWND hWnd, uint processId, string name) : base(config, moduleConfig, skipColor: true)
        {
            _windowProcessId = processId;
            HWnd = hWnd;

            ViewModel = new WindowBarWindowViewModel(config, moduleConfig, processId, name);

            InitializeComponent();
            ApplyComputedStyles(moduleConfig);
        }

        /// <summary>
        /// Applies computed styles to the <see cref="Grid"/>.
        /// </summary>
        /// <param name="moduleConfig"><inheritdoc
        /// cref="WindowBarView.WindowBarView"
        /// path="/param[@name='config']"/></param>
        private void ApplyComputedStyles(Configuration.Json.Modules.WindowBar moduleConfig)
        {
            if (moduleConfig.Windows is null)
            {
                return;
            }

            Style gridStyle = new(typeof(Grid));

            if (moduleConfig.Windows.FixedLength is not null)
            {
                gridStyle.Setters.Add(new Setter(Grid.WidthProperty, moduleConfig.Windows.FixedLength));
            }

            if (moduleConfig.Windows.MaxLength is not null)
            {
                gridStyle.Setters.Add(new Setter(Grid.MaxWidthProperty, moduleConfig.Windows.MaxLength));
            }

          ((Grid)Content).Style = gridStyle;

            if (moduleConfig.Windows.NiceTruncation)
            {
                Style textBlockStyle = new(typeof(TextBlock));

                StylesHelper.MergeWithAncestor(textBlockStyle, this, typeof(TextBlock));

                textBlockStyle.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
                textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.NoWrap));

                Resources[typeof(TextBlock)] = textBlockStyle;
            }
        }
    }
}