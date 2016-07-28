using System;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class PonySFMAPIConnector : IAPIConnector
    {
        string _baseUrl = "https://ponysfm.com/";

        public Task DownloadRevisionZIP(int id, string filepath)
        {
            throw new NotImplementedException();
        }

        public int FetchCurrentVersion()
        {
            throw new NotImplementedException();
        }
    }
}
