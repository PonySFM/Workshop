using Microsoft.Win32;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostInstall
{
    internal class Program
    {
        private static void RegisterUrlProtocol(string protocolName, string applicationPath, string description)
        {
            // Create new key for desired URL protocol
            var key = Registry.ClassesRoot.CreateSubKey(protocolName);

            // Assign protocol
            if (key != null)
            {
                key.SetValue(null, description);
                key.SetValue("URL Protocol", string.Empty);

                // Register Shell values
                Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell");
                Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open");
            }

            key = Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open\\command");

            // Specify application handling the URL protocol
            key?.SetValue(null, "\"" + applicationPath + "\" %1");
        }


        private static void Main(string[] args)
        {
            RegisterUrlProtocol("ponysfm", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PonySFM Workshop.exe"), "PonySFM Installer Client");
        }
    }
}
