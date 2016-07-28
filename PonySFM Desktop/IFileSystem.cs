﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        bool DirectoryExists(string path);

        void CreateFile(string path, byte[] data = null);
        void CreateDirectory(string path);

        XmlDocument OpenXML(string filepath);
        void SaveXML(XmlDocument doc, string filepath);

        string GetChecksum(string filepath);
    }
}