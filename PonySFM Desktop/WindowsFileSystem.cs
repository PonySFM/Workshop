using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public class WindowsFile : IFile
    {
        string _path;

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string Name
        {
            get
            {
                return global::System.IO.Path.GetFileName(_path);
            }
        }

        public WindowsFile(string path)
        {
            _path = path;
        }

        public bool IsDirectory()
        {
            throw new NotImplementedException();
        }

        public bool IsFile()
        {
            throw new NotImplementedException();
        }
    }

    public class WindowsFileSystem : IFileSystem
    {
        public void CopyFile(string src, string dest)
        {
            File.Copy(src, dest);
        }

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

        public List<IFile> GetFiles(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            List<IFile> ret = new List<IFile>();

            foreach (var file in dirInfo.GetFiles())
            {
                ret.Add(new WindowsFile(file.FullName));
            }

            return ret;
        }

        public List<IFile> GetDirectories(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            List<IFile> ret = new List<IFile>();

            foreach (var file in dirInfo.GetDirectories())
            {
                ret.Add(new WindowsFile(file.FullName));
            }

            return ret;
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

        public void DeleteFile(string filepath)
        {
            File.Delete(filepath);
        }

        public void DeleteDirectory(string filepath)
        {
            Directory.Delete(filepath);
        }
    }
}
