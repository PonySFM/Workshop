using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class PonySFMAPIConnector : IAPIConnector
    {
        static PonySFMAPIConnector singleton;
        string _baseUrl = "https://ponysfm.com";
        CookedWebClient _webClient = new CookedWebClient();

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
            _webClient.Headers.Add("user-agent", "PSFM_ModManager-" + ModManager.Version);
        }

        /// <summary>
        /// Fetch a JSON object from a given URL
        /// </summary>
        /// <typeparam name="T">JSON object class</typeparam>
        /// <param name="url">e.g. /api/thing.json</param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<T> FetchJSON<T>(string url, params object[] args)
        {
            var data = await _webClient.DownloadStringTaskAsync(new Uri(_baseUrl + string.Format(url, args)));
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// Fetches and sets metadata for the specified revision.
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        public async Task FetchMetadata(Revision revision)
        {
            int id = revision.ID;

            var revisionAPIObject = await FetchJSON<RevisionAPIObject>("/api/revision.json?id={0}", id);
            var resourceAPIObject = await FetchJSON<ResourceAPIObject>("/api/resource.json?id={0}", revisionAPIObject.resource_id);

            revision.AdditionalData["ResourceName"] = resourceAPIObject.name;

            if(resourceAPIObject.HasUser())
            {
                var userAPIObject = await FetchJSON<UserAPIObject>("/api/user.json?id={0}", resourceAPIObject.user_id);
                revision.AdditionalData["UserName"] = userAPIObject.name;
            }
            else
            {
                revision.AdditionalData["UserName"] = "None";
            }
        }

        public async Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress)
        {
            _webClient.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
            {
                progress.Report(e.ProgressPercentage);
            };

            await _webClient.DownloadFileTaskAsync(new Uri(string.Format("{0}/rev/{1}/internal_download_redirect", _baseUrl, id)),
                filepath);
        }

        public string GetRevisionURL(Revision revision)
        {
            return string.Format("{0}/rev/{1}", _baseUrl, revision.ID);
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
