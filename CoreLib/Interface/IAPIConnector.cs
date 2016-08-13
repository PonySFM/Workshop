using System;
using System.Threading.Tasks;
using CoreLib;

namespace CoreLib.Interface
{
    public interface IAPIConnector
    {
        int FetchCurrentVersion();
        Task DownloadRevisionZIP(int id, string filepath, IProgress<int> progress);
        Task DownloadRevisionAdditionalInformation(Revision revision);
    }
}
