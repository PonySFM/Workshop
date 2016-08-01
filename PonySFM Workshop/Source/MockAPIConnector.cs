using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public class MockAPIConnector : IAPIConnector
    {
        private List<Tuple<string, Revision>> _fakeRevisions = new List<Tuple<string, Revision>>();
        private IFileSystem _fs;

        public List<Tuple<string ,Revision>> FakeRevisions
        {
            get
            {
                return _fakeRevisions;
            }

            set
            {
                _fakeRevisions = value;
            }
        }

        public MockAPIConnector(IFileSystem fs)
        {
            _fs = fs;
        }

        public void AddRevision(string dir, Revision revision)
        {
            _fakeRevisions.Add(new Tuple<string, Revision>(dir, revision));
        }


        public async Task DownloadRevisionAdditionalInformation(Revision revision)
        {
            // NOP
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
