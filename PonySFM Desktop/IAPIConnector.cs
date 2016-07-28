using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    interface IAPIConnector
    {
        int FetchCurrentVersion();
        Task DownloadRevisionZIP(int id, string filepath);
    }
}
