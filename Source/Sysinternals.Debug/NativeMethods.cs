/*//////////////////////////////////////////////////////////////////////////////
// ProcessMonitorTraceListener
// 
// History:
// - April 1, 2010 - Version 1.0 - John Robbins/Wintellect
//      - Initial release.
// - March 1, 2014 - Version 1.1 - John Robbins/Wintellect
//      - Moved to VS 2013 and .NET 4.5.1
//////////////////////////////////////////////////////////////////////////////*/

using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Sysinternals.Debug
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A class to wrap all the native code needed by this assembly.
    /// </summary>
    internal static class NativeMethods
    {
        // Constants to represent C preprocessor macros for PInvokes
        const uint GENERIC_WRITE = 0x40000000;
        const uint OPEN_EXISTING = 3;
        const uint FILE_WRITE_ACCESS = 0x0002;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        const uint METHOD_BUFFERED = 0;

        // Procmon Constants 
        const uint FILE_DEVICE_PROCMON_LOG = 0x00009535;
        const string PROCMON_DEBUGGER_HANDLER = "\\\\.\\Global\\ProcmonDebugLogger";

        /// <summary>
        /// The handle to the procmon log device.
        /// </summary>
        private static SafeFileHandle hProcMon;

        /// <summary>
        /// Get the IO Control code for the ProcMon log.
        /// </summary>
        private static uint IOCTL_EXTERNAL_LOG_DEBUGOUT { get { return CTL_CODE(); } }

        /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/hardware/ff543023(v=vs.85).aspx"/>
        private static uint CTL_CODE(
            uint DeviceType = FILE_DEVICE_PROCMON_LOG,
            uint Function = 0x81,
            uint Method = METHOD_BUFFERED,
            uint Access = FILE_WRITE_ACCESS)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        /// <remarks>This is only used for opening the procmon log handle, hence the default parameters.</remarks>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx"/>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName = PROCMON_DEBUGGER_HANDLER,
            uint dwDesiredAccess = GENERIC_WRITE,
            uint dwShareMode = FILE_SHARE_WRITE,
            IntPtr lpSecurityAttributes = default(IntPtr),
            uint dwCreationDisposition = OPEN_EXISTING,
            uint dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL,
            IntPtr hTemplateFile = default(IntPtr));

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice, uint dwIoControlCode,
            StringBuilder lpInBuffer, uint nInBufferSize,
            IntPtr lpOutBuffer, uint nOutBufferSize,
            out uint lpBytesReturned, IntPtr lpOverlapped);

        static NativeMethods()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                if (!hProcMon.IsInvalid) hProcMon.Close();
            };
        }

        /// <summary>
        /// Does the actual tracing to Process Monitor.
        /// </summary>
        /// <param name="message">
        /// The message to display.
        /// </param>
        /// <param name="args">
        /// The formatting arguments for the message
        /// </param>
        /// <returns>
        /// True if the trace succeeded, false otherwise.
        /// </returns>
        public static bool ProcMonDebugOutput(string message, params object[] args)
        {
            bool returnValue = false;
            try
            {
                StringBuilder renderedMessage = new StringBuilder(); 
                renderedMessage.AppendFormat(message, args);
                uint outLen;
                if (hProcMon == null || hProcMon.IsInvalid)
                {
                    hProcMon = CreateFile();
                }
                DeviceIoControl(
                    hProcMon, IOCTL_EXTERNAL_LOG_DEBUGOUT,
                    renderedMessage, (uint)(message.Length * Marshal.SizeOf(typeof(char))),
                    IntPtr.Zero, 0, out outLen, IntPtr.Zero);
            }
            catch (EntryPointNotFoundException notFoundException)
            {
                // This means the appropriate ProcMonDebugOutput[Win32|x64].DLL
                // file could not be found. I'll eat this exception so it does
                // not take down the application.
                Debug.WriteLine(notFoundException.Message);
            }

            return returnValue;
        }
    }
}
