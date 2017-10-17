using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using CoreLib;
using CoreLib.Impl;
using CoreLib.Interface;

namespace CoreLibTest
{
    [TestClass]
    public class RevisionDatabaseTest
    {
        private static string _filepath;
        private static string _stubfile;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _filepath = Path.Combine(Path.GetTempPath(), "ponysfmtest.xml");
            _stubfile = Path.Combine(Path.GetTempPath(), "stubtest.xml");
        }

        [TestMethod]
        [TestCategory("RevisionDatabase")]
        public void CreateDefaultDb()
        {
            var fs = new MockFileSystem();
            var db = new RevisionDatabase(_filepath, fs);
            Assert.IsTrue(fs.FileExists(_filepath));

            var doc = fs.OpenXml(_filepath);

            Assert.IsTrue(doc.HasChildNodes);
            Assert.IsTrue(doc.FirstChild.Name == "PonySFM");
            Assert.IsTrue(!doc.FirstChild.HasChildNodes);
        }

        [TestMethod]
        [TestCategory("RevisionDatabase")]
        public void PopulateData()
        {
            var fs = new MockFileSystem();
            fs.CreateFile(_stubfile);
            var db = new RevisionDatabase(_filepath, fs);
            Assert.IsTrue(fs.FileExists(_filepath));

            for (var i = 0; i < 5; i ++)
                db.Revisions.Add(CreateStubRevision(fs));

            db.WriteDbDisk();

            var doc = fs.OpenXml(_filepath);

            Assert.IsTrue(doc.HasChildNodes);
            Assert.IsTrue(doc.FirstChild.Name == "PonySFM");
            Assert.IsTrue(doc.FirstChild.HasChildNodes);
            Assert.IsTrue(doc.FirstChild.ChildNodes.Count == 5);

            foreach (XmlElement elem in doc.FirstChild.ChildNodes)
            {
                Assert.IsTrue(elem.HasAttribute("ID"));
                Assert.IsTrue(elem.HasAttribute("Test"));
                Assert.AreEqual("Cake", elem.GetAttribute("Test"));
                Assert.IsTrue(elem.HasChildNodes);

                foreach (XmlElement fileElem in elem.ChildNodes)
                {
                    Assert.IsTrue(fileElem.GetAttribute("Location") == _stubfile);
                }
            }
        }

        private static Revision CreateStubRevision(IFileSystem fs)
        {
            var r = new Random();
            var id = r.Next(1000);
            var files = new List<RevisionFileEntry>();
            for(var i = 0; i < 5; i++)
            {
                files.Add(RevisionFileEntry.FromFile(_stubfile, fs));
            }

            return new Revision(id, files) {Metadata = {["Test"] = "Cake"}};
        }
    }
}
