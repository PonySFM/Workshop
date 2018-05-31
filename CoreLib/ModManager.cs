using System;
using System.IO;

namespace CoreLib
{
    public class ModManager
    {
        ///TODO: Use <see cref="System.Version"/> instead, if possible.
        public static readonly int Version = 101;
        public static readonly string PonySfmurl = "https://ponysfm.com";
        public static readonly string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        public static readonly string ConfigFileLocation = ConfigLocation + "\\config.xml";

        public static void CreateFolders()
        {
            if(!Directory.Exists(ConfigLocation))
                Directory.CreateDirectory(ConfigLocation);
        }
    }
}
