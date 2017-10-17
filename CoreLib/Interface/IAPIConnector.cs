using System;
using System.Threading.Tasks;
using CoreLib;

namespace CoreLib.Interface
{
    public interface IApiConnector
    {
        int FetchCurrentVersion();
        Task DownloadRevisionZip(int id, string filepath, IProgress<int> progress);
        Task FetchMetadata(Revision revision);
    }
}
