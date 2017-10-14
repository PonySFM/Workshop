using System;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class MockZIPFile : IZIPFile
    {
        private readonly string _path;
        private readonly IFileSystem _fs;

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
