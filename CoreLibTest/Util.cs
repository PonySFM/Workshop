using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    public static class Util
    {
        public static void CreateSFMDirectory(MockFileSystem fs)
        {
            fs.CreateDirectory("C:\\");
            fs.CreateDirectory("C:\\SFM");
        }

        public static void CreateFakeTempRevisionStructure(MockFileSystem fs)
        {
            fs.CreateDirectory("C:\\tmp");
            fs.CreateDirectory("C:\\tmp\\models");
            fs.CreateFile("C:\\tmp\\models\\pony.vtf");
            fs.CreateDirectory("C:\\tmp\\materials");
            fs.CreateFile("C:\\tmp\\materials\\pony.vmt");
        }

        public static Revision CreateFakeTempRevision(MockFileSystem fs)
        {
            CreateFakeTempRevisionStructure(fs);

            var files = new List<RevisionFileEntry>
            {
                RevisionFileEntry.FromFile("C:\\tmp\\models\\pony.vtf", fs),
                RevisionFileEntry.FromFile("C:\\tmp\\materials\\pony.vmt", fs)
            };

            return new Revision(1, files);
        }
    }
}
