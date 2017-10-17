using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class PonySfmapiConnector : IApiConnector
    {
        private static PonySfmapiConnector _singleton;
        private const string BaseUrl = "https://ponysfm.com";
        private readonly CookedWebClient _webClient = new CookedWebClient();

        public static PonySfmapiConnector Instance => _singleton ?? (_singleton = new PonySfmapiConnector());

        private PonySfmapiConnector()
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
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new SnakeCaseContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(data, settings);
        }

        /// <summary>
        /// Fetches and sets metadata for the specified revision.
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        public async Task FetchMetadata(Revision revision)
        {
            var id = revision.ID;

            var revisionApiObject = await FetchJson<RevisionApiObject>("/api/revision.json?id={0}", id);
            var resourceApiObject = await FetchJson<ResourceApiObject>("/api/resource.json?id={0}", revisionApiObject.ResourceID);

            revision.Metadata["ResourceName"] = resourceApiObject.Name;

            if(resourceApiObject.HasUser())
            {
                var userApiObject = await FetchJson<UserApiObject>("/api/user.json?id={0}", resourceApiObject.UserId);
                revision.Metadata["UserName"] = userApiObject.Name;
            }
            else
            {
                revision.Metadata["UserName"] = "None";
            }
        }

        public async Task DownloadRevisionZip(int id, string filepath, IProgress<int> progress)
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
