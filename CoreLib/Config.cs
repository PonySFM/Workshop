using System;
using System.IO;

namespace CoreLib
{
    public class Config
    {
        private static readonly string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        private static readonly string ConfigFileLocation = ConfigLocation + "\\config.xml";

        static bool CheckConfigExists()
        {
            return Directory.Exists(ConfigLocation) && File.Exists(ConfigFileLocation);
        }
    }
}
