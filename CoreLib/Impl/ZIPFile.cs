using System;
using System.Threading.Tasks;
using Ionic.Zip;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class ZIPFile : IZIPFile
    {
        string _path;

        public ZIPFile(string path)
        {
            _path = path;
        }

        public async Task Extract(string dir, IProgress<int> progress)
        {
            using (ZipFile zip1 = ZipFile.Read(_path))
            {
                int total = zip1.Entries.Count;
                int i = 0;
                await Task.Run(() =>
                {
                    foreach (ZipEntry e in zip1)
                    {
                        /* Skip annoying READMEs etc. */
                        if (e.FileName.ToLower().EndsWith(".txt"))
                            continue;
                        e.Extract(dir, ExtractExistingFileAction.OverwriteSilently);

                        progress?.Report((int)(i / (double)total * 100.0));
                        i++;
                    }
                });
            }
        }
    }
}
