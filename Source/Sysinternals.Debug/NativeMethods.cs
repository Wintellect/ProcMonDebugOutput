/*//////////////////////////////////////////////////////////////////////////////
// ProcessMonitorTraceListener
// 
// History:
// - April 1, 2010 - Version 1.0 - John Robbins/Wintellect
//      - Initial release.
// - March 1, 2014 - Version 1.1 - John Robbins/Wintellect
//      - Moved to VS 2013 and .NET 4.5.1
//////////////////////////////////////////////////////////////////////////////*/

namespace Sysinternals.Debug
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A class to wrap all the native code needed by this assembly.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Flag to check if we've already done the process lookup.
        /// </summary>
        private static bool lookedUpProcessType;

        /// <summary>
        /// True if the process is 64-bit.
        /// </summary>
        private static bool is64BitProcess;

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
            if (false == lookedUpProcessType)
            {
                lookedUpProcessType = true;

                Assembly mscorlibAssem = Assembly.GetAssembly(typeof(object));
                Module[] mods = mscorlibAssem.GetModules();
                PortableExecutableKinds peK;
                ImageFileMachine ifn;
                mods[0].GetPEKind(out peK, out ifn);
                is64BitProcess = ImageFileMachine.I386 != ifn;
            }

            bool returnValue = false;
            try
            {
                string renderedMessage = string.Format(message, args);
                if (true == is64BitProcess)
                {
                    returnValue = ProcMonDebugOutputx64(renderedMessage);
                }
                else
                {
                    returnValue = ProcMonDebugOutputWin32(renderedMessage);
                }
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

        [DllImport("ProcMonDebugOutputx64.dll",
                   CharSet = CharSet.Unicode,
                   EntryPoint = "ProcMonDebugOutput",
                   SetLastError = true,
                   CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ProcMonDebugOutputx64(string lpOutput);

        [DllImport("ProcMonDebugOutputWin32.dll",
                   CharSet = CharSet.Unicode,
                   EntryPoint = "ProcMonDebugOutput",
                   SetLastError = true,
                   CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ProcMonDebugOutputWin32(string lpOutput);
    }
}
