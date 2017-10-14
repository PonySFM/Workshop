using System.IO;
using CoreLib.Interface;

namespace CoreLib
{
    public class TempRevisionParser
    {
        private readonly IFileSystem _fs;
        private readonly string _path;

        public TempRevisionParser(string path, IFileSystem fs)
        {
            _path = path;
            _fs = fs;
        }

        public string FindModFolder()
        {
            var ret = string.Empty;
            var folders = _fs.GetDirectories(_path, true);
            foreach (var folder in folders)
            {
                if (!IsModFolder(folder.Path)) continue;

                ret = Directory.GetParent(folder.Path).FullName;
                break;
            }

            return ret; 
        }

        private static bool IsModFolder(string folder)
        {
            var onlyName = Path.GetFileName(folder)?.ToLower();
            return onlyName == "materials" || onlyName == "models" || onlyName == "maps" || onlyName == "sounds" || onlyName == "particles" || onlyName == "scripts";
        }
    }
}
