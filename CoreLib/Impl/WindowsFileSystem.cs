using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class WindowsFile : IFile
    {
        public string Name => System.IO.Path.GetFileName(Path);
        public string Path { get; }

        public WindowsFile(string path)
        {
            Path = path;
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
        private static WindowsFileSystem _singleton;

        public static WindowsFileSystem Instance => _singleton ?? (_singleton = new WindowsFileSystem());

        private WindowsFileSystem()
        {
        }

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
            if(data != null) file.Write(data, 0, data.Length);
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
            var checksum = FileUtil.GetChecksum(stream);
            stream.Close();
            return checksum;
        }

        public List<IFile> GetFiles(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);

            return dirInfo.GetFiles().Select(file => new WindowsFile(file.FullName)).Cast<IFile>().ToList();
        }

        public List<IFile> GetDirectories(string dir, bool recursive = false)
        {
            var opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var dirInfo = new DirectoryInfo(dir);
            return dirInfo.GetDirectories("*.*", opt).Select(file => new WindowsFile(file.FullName)).Cast<IFile>().ToList();
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
            Directory.Delete(filepath, true);
        }

        public Task CopyFileAsync(string src, string dest)
        {
            return Task.Factory.StartNew(() => {
                File.Copy(src, dest, true);
            });
        }

        public IZIPFile OpenZIP(string filepath)
        {
            return new ZIPFile(filepath);
        }

        public string GetTempPath()
        {
            var path = Path.GetTempFileName();
            File.Delete(path);
            return path;
        }

        public byte[] ReadFile(string filepath)
        {
            return File.ReadAllBytes(filepath);
        }
    }
}
