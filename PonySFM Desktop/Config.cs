using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PonySFM_Desktop
{
    public class Config
    {
        static string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        static string ConfigFileLocation = ConfigLocation + "\\config.xml";

        static bool CheckConfigExists()
        {
            return Directory.Exists(ConfigLocation) && File.Exists(ConfigFileLocation);
        }
    }
}
