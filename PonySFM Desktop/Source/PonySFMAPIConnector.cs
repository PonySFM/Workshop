using System;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class PonySFMAPIConnector : IAPIConnector
    {
        string _baseUrl = "https://ponysfm.com/";

        public async Task DownloadRevisionZIP(int id, string filepath)
        {
            var webClient = new CookedWebClient();
            webClient.Headers.Add("user-agent", "PSFM_ModManager-" + ModManager.Version);
            await webClient.DownloadFileTaskAsync(new Uri(string.Format("{0}/rev/{1}/internal_download_redirect", _baseUrl, id)),
                filepath);
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
