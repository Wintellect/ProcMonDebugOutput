using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Diagnostics;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Sysinternals.log4net;

namespace ManagedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(new ColoredConsoleAppender());
            hierarchy.Root.AddAppender(new ProcMonAppender());
            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;

            ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            Debug.Listeners.Remove("Default");

            StringBuilder sb = new StringBuilder(100);
            for (int i = 0; i < 20; i++)
            {
                sb.Length = 0;
                sb.AppendFormat("ProcMon Debug Out Test # {0}", i);
                Trace.Write(sb.ToString());

                if (i%2 == 0)
                {
                    _logger.DebugFormat("ProcMon log4net Out Test # {0}", i);
                }
                else if (i%3 == 0)
                {
                    _logger.InfoFormat("ProcMon log4net Out Test # {0}", i);
                }
                else if (i%5 == 0)
                {
                    _logger.WarnFormat("ProcMon log4net Out Test # {0}", i);
                }
                else
                {
                    _logger.ErrorFormat("ProcMon log4net Out Test # {0}", i);
                }
            }
        }
    }
}
