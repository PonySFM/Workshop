using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace PonySFM_Desktop
{
    public static class Logger
    {
        public static void Open()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(File.AppendText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ponysfm_mod_manager.log"))));
            Log("-- Logger started -- \n");
            Log("\n");
        }

        public static void Log(string log)
        {
            Trace.WriteLine("["+DateTime.Now.ToString()+"] " + log);
            Trace.Flush();
        }
    }
}

