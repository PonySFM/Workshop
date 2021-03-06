﻿using System;
using System.Threading.Tasks;
using Ionic.Zip;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class ZipFile : IZipFile
    {
        private readonly string _path;

        public ZipFile(string path)
        {
            _path = path;
        }

        public async Task Extract(string dir, IProgress<int> progress)
        {
            using (var zip = Ionic.Zip.ZipFile.Read(_path))
            {
                var total = zip.Entries.Count;
                var i = 0;
                await Task.Run(() =>
                {
                    foreach (var e in zip)
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
