using PonySFM_Workshop.Source;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public class ModManager
    {
        ///TODO: Use <see cref="System.Version"/> instead, if possible.
        public static int Version = 100;
        public static string PonySFMURL = "https://ponysfm.com";
        public static string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        public static string ConfigFileLocation = ConfigLocation + "\\config.xml";

        public static void CreateFolders()
        {
            if(!Directory.Exists(ConfigLocation))
                Directory.CreateDirectory(ConfigLocation);
        }
    }
}
