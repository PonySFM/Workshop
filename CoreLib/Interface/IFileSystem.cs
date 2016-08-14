﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace CoreLib.Interface
{
    public interface IFileSystem
    {
        List<IFile> GetFiles(string dir);
        List<IFile> GetDirectories(string dir, bool recursive = false);

        void CopyFile(string src, string dest);
        Task CopyFileAsync(string src, string dest);

        bool FileExists(string path);
        bool DirectoryExists(string path);

        void CreateFile(string path, byte[] data = null);
        void CreateDirectory(string path);

        XmlDocument OpenXML(string filepath);
        void SaveXML(XmlDocument doc, string filepath);

        string GetChecksum(string filepath);

        void DeleteFile(string filepath);
        void DeleteDirectory(string filepath);

        IZIPFile OpenZIP(string filepath);

        string GetTempPath();

        byte[] ReadFile(string filepath);
    }
}