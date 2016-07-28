using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public class WindowsFileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void CreateFile(string path, byte[] data = null)
        {
            var file = File.Create(path);
            file.Write(data, 0, data.Length);
            file.Close();
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetChecksum(string filepath)
        {
            var stream = File.OpenRead(filepath);
            return FileUtil.GetChecksum(stream);
        }

        public XmlDocument OpenXML(string filepath)
        {
            var doc = new XmlDocument();
            doc.Load(filepath);

            return doc;
        }

        public void SaveXML(XmlDocument doc, string filepath)
        {
            doc.Save(filepath);
        }
    }
}
