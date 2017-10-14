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
        private static PonySFMAPIConnector _singleton;
        private const string BaseUrl = "https://ponysfm.com";
        private readonly CookedWebClient _webClient = new CookedWebClient();

        public static PonySFMAPIConnector Instance => _singleton ?? (_singleton = new PonySFMAPIConnector());

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
        private async Task<T> FetchJson<T>(string url, params object[] args)
        {
            var data = await _webClient.DownloadStringTaskAsync(new Uri(BaseUrl + string.Format(url, args)));
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// Fetches and sets metadata for the specified revision.
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        public async Task FetchMetadata(Revision revision)
        {
            var id = revision.ID;

            var revisionApiObject = await FetchJson<RevisionAPIObject>("/api/revision.json?id={0}", id);
            var resourceApiObject = await FetchJson<ResourceAPIObject>("/api/resource.json?id={0}", revisionApiObject.resource_id);

            revision.Metadata["ResourceName"] = resourceApiObject.name;

            if(resourceApiObject.HasUser())
            {
                var userApiObject = await FetchJson<UserAPIObject>("/api/user.json?id={0}", resourceApiObject.user_id);
                revision.Metadata["UserName"] = userApiObject.name;
            }
            else
            {
                revision.Metadata["UserName"] = "None";
            }
        }

        public async Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress)
        {
            _webClient.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
            {
                progress.Report(e.ProgressPercentage);
            };

            await _webClient.DownloadFileTaskAsync(new Uri($"{BaseUrl}/rev/{id}/internal_download_redirect"),
                filepath);
        }

        public string GetRevisionUrl(Revision revision)
        {
            return $"{BaseUrl}/rev/{revision.ID}";
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
