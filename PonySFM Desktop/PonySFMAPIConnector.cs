using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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
