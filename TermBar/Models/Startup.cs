using Spakov.TermBar.Views;
using Spakov.TermBar.WindowManagement.Windows;
using System;
using Windows.ApplicationModel;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// Methods to manage TermBar startup-at-boot behavior.
    /// </summary>
    internal static class Startup
    {
        /// <summary>
        /// (Un)registers TermBar to start up at boot, if configured to do so.
        /// </summary>
        /// <param name="config">A TermBar configuration.</param>
        internal async static void SyncStartupTask(Configuration.Json.Config config)
        {
            bool configStartupAtBoot = false;

            if (config.StartupAtBoot is not null)
            {
                configStartupAtBoot = (bool)config.StartupAtBoot;
            }

            StartupTask startupTask = await StartupTask.GetAsync("TermBarStartup");

            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    if (configStartupAtBoot)
                    {
                        await startupTask.RequestEnableAsync();
                    }

                    break;

                case StartupTaskState.DisabledByUser:
                    StartupTaskDisabledByUserView startupTaskDisabledByUserView = new(App.Config!.Displays[0].TermBar);

                    DialogWindow startupTaskDisabledByUserDialogWindow = new(
                        App.Config!.Displays[0].TermBar,
                        startupTaskDisabledByUserView
                    );

                    startupTaskDisabledByUserView.Owner = startupTaskDisabledByUserDialogWindow;
                    startupTaskDisabledByUserDialogWindow.Display();

                    break;

                case StartupTaskState.Enabled:
                    if (!configStartupAtBoot)
                    {
                        startupTask.Disable();
                    }

                    break;
            }
        }
    }
}
