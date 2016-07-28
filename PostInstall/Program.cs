using Microsoft.Win32;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostInstall
{
    class Program
    {
        static void RegisterURLProtocol(string protocolName, string applicationPath, string description)
        {
            // Create new key for desired URL protocol
            RegistryKey myKey=Registry.ClassesRoot.CreateSubKey(protocolName);

            // Assign protocol
            myKey.SetValue(null, description);
            myKey.SetValue("URL Protocol", string.Empty);

            // Register Shell values
            Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell");
            Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open");
            myKey = Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open\\command");

            // Specify application handling the URL protocol
            myKey.SetValue(null, "\"" + applicationPath + "\" %1");
        }


        static void Main(string[] args)
        {
            RegisterURLProtocol("ponysfm", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PonySFM Desktop.exe"), "PonySFM Installer Client");
        }
    }
}
