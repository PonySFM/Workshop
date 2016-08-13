using System;
using System.Threading.Tasks;

namespace CoreLib.Interface
{
    public interface IZIPFile
    {
        Task Extract(string dir, IProgress<int> progress);
    }
}
