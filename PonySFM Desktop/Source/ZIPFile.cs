using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace PonySFM_Desktop
{
    public class ZIPFile : IZIPFile
    {
        string _path;

        public ZIPFile(string path)
        {
            _path = path;
        }

        public async Task Extract(string dir)
        {
            using (ZipFile zip1 = ZipFile.Read(_path))
            {
                await Task.Run(() => zip1.ExtractAll(dir, ExtractExistingFileAction.OverwriteSilently));
            }
        }
    }
}
