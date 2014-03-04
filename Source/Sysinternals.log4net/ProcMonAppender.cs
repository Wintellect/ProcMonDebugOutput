using System;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using Sysinternals.Debug;

namespace Sysinternals.log4net
{
    /// <summary>
    /// A <c>log4net</c> appender for ProcMonDebugOutput.
    /// </summary>
    public class ProcMonAppender : AppenderSkeleton
    {
        /// <summary>
        ///  Default constructor.
        /// </summary>
        /// <remarks>
        /// Sets the default layout.
        /// </remarks>
        public ProcMonAppender()
        {
            // Although it breaks convention set by the built-in appenders, this is more forgiving.
            Layout = new PatternLayout("%-5p %m");
        }
        /// <summary>
        /// This appender requires a <see cref="AppenderSkeleton.Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        override protected bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            NativeMethods.ProcMonDebugOutput(RenderLoggingEvent(loggingEvent));
        }
    }
}
