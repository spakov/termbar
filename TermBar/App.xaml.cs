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
using Windows.ApplicationModel.Resources;
using Windows.Win32;

namespace Spakov.TermBar
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default
    /// Application class.
    /// </summary>
    public partial class App : Application
    {
        private static ResourceLoader? s_resourceLoader;

        private Exception? _criticalFailure;
        private ExceptionView? _exceptionView;
        private WindowManagement.Windows.DialogWindow? _exceptionDialogWindow;

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
        /// App resources.
        /// </summary>
        internal static ResourceLoader ResourceLoader => s_resourceLoader!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first
        /// line of authored code executed, and as such is the logical
        /// equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            TrimRoots.PreserveTrimmableClasses();

            s_resourceLoader = ResourceLoader.GetForViewIndependentUse();

            _criticalFailure = null;
            InitializeComponent();
            WindowManager = new();

            ConfigHelper.JsonSerializerOptions.Encoder = JavaScriptEncoderJsonTartare.Instance;

            try
            {
                Config = ConfigHelper.Load(WindowManager);
            }
            catch (JsonException e)
            {
                _criticalFailure = e;
                return;
            }

            DispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (Config.StartDirectory is not null)
            {
                string expandedStartDirectory = Environment.ExpandEnvironmentVariables(Config.StartDirectory);

                if (Directory.Exists(expandedStartDirectory))
                {
                    Directory.SetCurrentDirectory(expandedStartDirectory);
                }
            }

#if DEBUG
      if (Environment.CommandLine.Contains("GenerateSchema")) {
        ConfigHelper.GenerateSchema();
        Current.Exit();
      }
#endif

            InitializeModels();
        }

        /// <summary>
        /// Initializes singleton intance models when they are required to use
        /// the UI thread.
        /// </summary>
        private static void InitializeModels()
        {
            Models.WindowList.Instance = new Models.WindowList(DispatcherQueue!);
            Volume.Instance = new Volume(DispatcherQueue!);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and
        /// process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (_criticalFailure is null)
            {
                WindowManager!.Display();
                UnhandledException += App_UnhandledException;

                Startup.SyncStartupTask(Config!);
            }
            else
            {
                UnhandledExceptionHandler(_criticalFailure);
            }
        }

        /// <summary>
        /// Displays the cause of unhandled exceptions to the user.
        /// </summary>
        /// <param name="sender"><inheritdoc
        /// cref="UnhandledExceptionEventHandler"
        /// path="/param[@name='sender']"/></param>
        /// <param name="e"><inheritdoc
        /// cref="UnhandledExceptionEventHandler"
        /// path="/param[@name='e']"/></param>
        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _criticalFailure = e.Exception;
            e.Handled = true;

            UnhandledExceptionHandler(_criticalFailure);
        }

        /// <summary>
        /// A very nice method of displaying <paramref name="e"/>.
        /// </summary>
        /// <param name="e">An <see cref="Exception"/>.</param>
        private void UnhandledExceptionHandler(Exception e)
        {
            if (WindowManager is null)
            {
                FallbackUnhandledExceptionHandler(e);

                return;
            }

            try
            {
                Config ??= new()
                {
                    Displays =
                    [
                        new()
                        {
                            Id = WindowManager.Displays.Keys.ToList()[0],
                            TermBar = new()
                        }
                    ]
                };

                if (Config.Displays[0].TermBar.Display is null)
                {
                    Config.Displays[0].TermBar.Display = WindowManager.Displays[Config.Displays[0].Id];
                }

                _exceptionView ??= new(Config.Displays[0].TermBar, e);

                if (_exceptionDialogWindow is null)
                {
                    _exceptionDialogWindow = new(
                        Config.Displays[0].TermBar,
                        _exceptionView
                    );

                    _exceptionView.Owner = _exceptionDialogWindow;
                    _exceptionDialogWindow.Closing += ExceptionDialogWindow_Closing;
                    _exceptionDialogWindow.Display();
                }
            }
            catch (Exception)
            {
                FallbackUnhandledExceptionHandler(e);
            }
        }

        /// <summary>
        /// A fallback method of displaying <paramref name="e"/>.
        /// </summary>
        /// <param name="e">An <see cref="Exception"/>.</param>
        private static void FallbackUnhandledExceptionHandler(Exception e)
        {
            PInvoke.MessageBox(
                Windows.Win32.Foundation.HWND.Null,
                string.Format(App.ResourceLoader.GetString("UnhandledExceptionMessage"), e),
                App.ResourceLoader.GetString("UnhandledException"),
                Windows.Win32.UI.WindowsAndMessaging.MESSAGEBOX_STYLE.MB_OK
            );
        }

        /// <summary>
        /// Prepares for displaying the exception dialog again and handles
        /// fatal exceptions by exiting.
        /// </summary>
        private void ExceptionDialogWindow_Closing()
        {
            bool isFatal = true;

            if (_exceptionView is not null)
            {
                isFatal = _exceptionView.IsFatal;
            }

            if (_exceptionDialogWindow is not null)
            {
                _exceptionDialogWindow.Closing -= ExceptionDialogWindow_Closing;
            }

            _exceptionDialogWindow = null;
            _exceptionView = null;

            if (isFatal)
            {
                Current.Exit();
            }
        }
    }
}