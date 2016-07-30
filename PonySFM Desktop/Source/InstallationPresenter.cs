using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    public class InstallationPresenter : IPresenter
    {
        public Control View
        {
            get
            {
                return View;
            }
            set
            {
                View = value;
                View.DataContext = this;
            }
        }

        public int Progress { get; private set; }
        public string Status { get; private set; }

        IAPIConnector _api;
        IFileSystem _fs;
        RevisionManager _revisionMgr;

        public InstallationPresenter(IAPIConnector api, IFileSystem fs, RevisionManager revisionMgr)
        {
            _api = api;
            _fs = fs;
            _revisionMgr = revisionMgr;
        }

        public async Task ExecuteInstallation(int id)
        {
            var zipTmp = Path.GetTempFileName();
            var tempDir = Path.GetTempFileName();

            await _api.DownloadRevisionZIP(id, tempDir);
            var zip = _fs.OpenZIP(tempDir);
            await zip.Extract(zipTmp);

            var tmpRev = Revision.CreateTemporaryRevisionFromFolder(id, zipTmp, _fs);
            await _revisionMgr.InstallRevision(tmpRev, tempDir);

            _fs.DeleteDirectory(zipTmp);
            _fs.DeleteDirectory(tempDir);
        }
    }
}
