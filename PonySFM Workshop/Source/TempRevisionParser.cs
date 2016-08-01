using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public class TempRevisionParser
    {
        IFileSystem _fs;
        string _path;

        public TempRevisionParser(string path, IFileSystem fs)
        {
            _path = path;
            _fs = fs;
        }

        public string FindModFolder()
        {
            string ret = string.Empty;
            var folders = _fs.GetDirectories(_path, true);
            foreach (var folder in folders)
            {
                if (IsModFolder(folder.Path))
                {
                    ret = Directory.GetParent(folder.Path).FullName;
                    break;
                }
            }

            return ret; 
        }

        private bool IsModFolder(string folder)
        {
            string onlyName = Path.GetFileName(folder).ToLower();
            return onlyName == "materials" || onlyName == "models" || onlyName == "maps" || onlyName == "sounds" || onlyName == "particles" || onlyName == "scripts";
        }
    }
}
