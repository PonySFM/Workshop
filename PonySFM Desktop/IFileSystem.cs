using System.Collections.Generic;
using System.Xml;

namespace PonySFM_Desktop
{
    public interface IFileSystem
    {
        List<IFile> GetFiles(string dir);
        List<IFile> GetDirectories(string dir);

        void CopyFile(string src, string dest);

        bool FileExists(string path);
        bool DirectoryExists(string path);

        void CreateFile(string path, byte[] data = null);
        void CreateDirectory(string path);

        XmlDocument OpenXML(string filepath);
        void SaveXML(XmlDocument doc, string filepath);

        string GetChecksum(string filepath);

        void DeleteFile(string filepath);
        void DeleteDirectory(string filepath);
    }
}
