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
                zip1.ExtractProgress += delegate (object sender, ExtractProgressEventArgs e)
                {
                    if (e.EntriesExtracted != 0)
                        progress.Report(e.EntriesExtracted / e.EntriesTotal * 100);
                };

                await Task.Run(() => zip1.ExtractAll(dir, ExtractExistingFileAction.DoNotOverwrite));
            }
        }
    }
}
