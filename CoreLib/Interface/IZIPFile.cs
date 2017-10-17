using System;
using System.Threading.Tasks;

namespace CoreLib.Interface
{
    public interface IZipFile
    {
        Task Extract(string dir, IProgress<int> progress);
    }
}
