using System;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using Sysinternals.Debug;
using System.Diagnostics.CodeAnalysis;

namespace Sysinternals.log4net
{
    /// <summary>
    /// A <c>log4net</c> appender for ProcMonDebugOutput.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", 
                     "CA1704:IdentifiersShouldBeSpelledCorrectly", 
                     MessageId = "Proc",
                     Justification="Naming conforms to the rest of the project")]
    [SuppressMessage("Microsoft.Naming",
                     "CA1704:IdentifiersShouldBeSpelledCorrectly",
                     MessageId = "Appender",
                     Justification = "Naming conforms the log4net project")]
    public class ProcMonAppender : AppenderSkeleton
    {
        /// <summary>
        ///  Default constructor.
        /// </summary>
        /// <remarks>
        /// Sets the default layout.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", 
                         "CA2214:DoNotCallOverridableMethodsInConstructors",
                         Justification="Justin put this in and as I don't use log4net, I'm afraid to touch it.")]
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
