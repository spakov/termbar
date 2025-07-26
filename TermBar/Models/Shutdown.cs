using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.Shutdown;

namespace Spakov.TermBar.Models
{
    /// <summary>
    /// Facilitates shutdowns, reboots, and the like.
    /// </summary>
    internal class Shutdown
    {
        private static readonly Shutdown s_instance = new();

        /// <summary>
        /// Initializes a <see cref="Shutdown"/>.
        /// </summary>
        /// <remarks>This constructor sets appropriate privileges to support
        /// <see cref="PInvoke.ExitWindowsEx"/>.</remarks>
        private Shutdown()
        {
            if (!PInvoke.OpenProcessToken(
                PInvoke.GetCurrentProcess_SafeHandle(),
                TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES | TOKEN_ACCESS_MASK.TOKEN_QUERY,
                out SafeFileHandle processToken)
            )
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!PInvoke.LookupPrivilegeValue(null, PInvoke.SE_SHUTDOWN_NAME, out LUID lpLuid))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            TOKEN_PRIVILEGES tkp = new()
            {
                PrivilegeCount = 1,
                Privileges = new VariableLengthInlineArray<LUID_AND_ATTRIBUTES>()
            };

            tkp.Privileges[0] = new()
            {
                Luid = lpLuid,
                Attributes = TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED
            };

            unsafe
            {
                if (!PInvoke.AdjustTokenPrivileges(
                    processToken,
                    false,
                    &tkp,
                    0,
                    null,
                    null)
                )
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            int err = Marshal.GetLastWin32Error();

            if (err != 0)
            {
                throw new Win32Exception(err);
            }
        }

        /// <summary>
        /// Initiates a <paramref name="shutdownType"/>.
        /// </summary>
        /// <remarks>We use <see
        /// cref="SHUTDOWN_REASON.SHTDN_REASON_MINOR_OTHER"/> so this works on
        /// Server editions of Windows as well.</remarks>
        /// <param name="shutdownType">The type of shutdown to
        /// initiate.</param>
        /// <returns><see langword="true"/> if the shutdown is proceeding or
        /// <see langword="false"/> if it failed.</returns>
        internal static bool Initiate(ShutdownType shutdownType)
        {
            Shutdown shutdown = s_instance;

            return shutdownType switch
            {
                ShutdownType.Logoff => (bool)PInvoke.ExitWindowsEx(EXIT_WINDOWS_FLAGS.EWX_LOGOFF, SHUTDOWN_REASON.SHTDN_REASON_MINOR_OTHER),
                ShutdownType.Reboot => (bool)PInvoke.ExitWindowsEx(EXIT_WINDOWS_FLAGS.EWX_REBOOT, SHUTDOWN_REASON.SHTDN_REASON_MINOR_OTHER),
                ShutdownType.PowerOff => (bool)PInvoke.ExitWindowsEx(EXIT_WINDOWS_FLAGS.EWX_POWEROFF, SHUTDOWN_REASON.SHTDN_REASON_MINOR_OTHER),
                ShutdownType.Suspend => (bool)PInvoke.SetSuspendState(false, false, false),
                ShutdownType.Lock => (bool)PInvoke.LockWorkStation(),
                _ => true,
            };
        }

        /// <summary>
        /// Supported shutdown types.
        /// </summary>
        internal enum ShutdownType
        {
            Logoff,
            Reboot,
            PowerOff,
            Suspend,
            Lock
        }
    }
}
