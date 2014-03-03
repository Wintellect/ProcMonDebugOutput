using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ManagedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Remove("Default");

            StringBuilder sb = new StringBuilder(100);
            for (int i = 0; i < 20; i++)
            {
                sb.Length = 0;
                sb.AppendFormat("ProcMon Debug Out Test # {0}",
                                i.ToString());
                Trace.Write(sb.ToString());
            }
        }
    }
}
