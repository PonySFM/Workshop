using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public class MockZIPFile : IZIPFile
    {
        string _path;
        IFileSystem _fs;

        public MockZIPFile(string path, IFileSystem fs)
        {
            _path = path;
            _fs = fs;
        }

        /* In MockZIPFile the archive itself is not a file but just the folder with the zip contents.
         * This simplifies the process. */
        public async Task Extract(string dir, IProgress<int> progress)
        {
            var dirCopier = new DirectoryCopier(_fs, _path, dir, true);
            await dirCopier.Execute();
        }
    }
}
