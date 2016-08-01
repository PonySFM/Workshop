using System;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public interface IAPIConnector
    {
        int FetchCurrentVersion();
        Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress);
        Task DownloadRevisionAdditionalInformation(Revision revision);
    }
}
