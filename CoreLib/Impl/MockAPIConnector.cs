using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class MockAPIConnector : IAPIConnector
    {
        private readonly List<Tuple<string, Revision>> _fakeRevisions = new List<Tuple<string, Revision>>();
        private readonly IFileSystem _fs;

        public MockAPIConnector(IFileSystem fs)
        {
            _fs = fs;
        }

        public void AddRevision(string dir, Revision revision)
        {
            _fakeRevisions.Add(new Tuple<string, Revision>(dir, revision));
        }

        public async Task FetchMetadata(Revision revision)
        {
            // NOP
            await Task.FromResult(false);
        }

        public async Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress)
        {
            var rev = _fakeRevisions.Find(r => r.Item2.ID == id);
            if (rev == null)
                throw new ArgumentException();

            /* In this case, we just copy the revision to filepath as a folder */
            var dirCopier = new DirectoryCopier(_fs, rev.Item1, filepath, true);

            dirCopier.OnProgress += delegate (object sender, DirectoryProgressEventArgs e)
            {
                progress?.Report(e.Progress);
            };

            await dirCopier.Execute();
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
