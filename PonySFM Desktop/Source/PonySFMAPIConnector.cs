using System;
using System.Net;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class PonySFMAPIConnector : IAPIConnector
    {
        string _baseUrl = "https://ponysfm.com/";
        static PonySFMAPIConnector singleton;

        public static PonySFMAPIConnector Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new PonySFMAPIConnector();
                return singleton;
            }
        }

        private PonySFMAPIConnector()
        {
        }

        public async Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress)
        {
            var webClient = new CookedWebClient();
            webClient.Headers.Add("user-agent", "PSFM_ModManager-" + ModManager.Version);

            webClient.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
            {
                progress.Report(e.ProgressPercentage);
            };

            await webClient.DownloadFileTaskAsync(new Uri(string.Format("{0}/rev/{1}/internal_download_redirect", _baseUrl, id)),
                filepath);
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
