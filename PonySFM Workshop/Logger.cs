using System;
using System.Diagnostics;
using System.IO;

namespace PonySFM_Workshop
{
    public static class Logger
    {
        public static void Open()
        {
            Trace.Listeners.Add(
                new TextWriterTraceListener(
                    File.AppendText(
                        Path.Combine(
                            Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location),
                            "ponysfm_mod_manager.log"))));
            Log("-- Logger started -- \n");
            Log("\n");
            Trace.AutoFlush = true;
        }

        public static void Log(string log)
        {
            Trace.WriteLine("["+DateTime.Now.ToString()+"] " + log);
        }
    }
}

