using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using System.Xml;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class RevisionDatabaseTest
    {
        private string filepath;
        private string stubfile;

        [SetUp]
        protected void Setup()
        {
            filepath = Path.Combine(Path.GetTempPath(), "ponysfmtest.xml");
            stubfile = Path.Combine(Path.GetTempPath(), "stubtest.xml");
        }

        [Test]
        public void CreateDefaultDB()
        {
            var fs = new MockFileSystem();
            var db = new RevisionDatabase(filepath, fs);
            Assert.True(fs.FileExists(filepath));

            var doc = fs.OpenXML(filepath);

            Assert.That(doc.HasChildNodes);
            Assert.That(doc.FirstChild.Name == "PonySFM");
            Assert.That(!doc.FirstChild.HasChildNodes);
        }

        [Test]
        public void PopulateData()
        {
            var fs = new MockFileSystem();
            fs.CreateFile(stubfile);
            var db = new RevisionDatabase(filepath, fs);
            Assert.True(fs.FileExists(filepath));

            for (int i = 0; i < 5; i ++)
                db.Revisions.Add(CreateStubRevision());

            db.WriteDBDisk();

            var doc = fs.OpenXML(filepath);

            Assert.That(doc.HasChildNodes);
            Assert.That(doc.FirstChild.Name == "PonySFM");
            Assert.That(doc.FirstChild.HasChildNodes);
            Assert.That(doc.FirstChild.ChildNodes.Count == 5);

            foreach (XmlElement elem in doc.FirstChild.ChildNodes)
            {
                Assert.That(elem.HasAttribute("ID"));
                Assert.That(elem.HasChildNodes);

                foreach (XmlElement fileElem in elem.ChildNodes)
                {
                    Assert.That(fileElem.GetAttribute("Location") == stubfile);
                }
            }
        }

        private Revision CreateStubRevision()
        {
            var r = new Random();
            int id = r.Next(1000);
            List<string> files = new List<string>();
            for(int i = 1; i < r.Next(10); i++)
            {
                files.Add(stubfile);
            }

            return new Revision(id, files);
        }
    }
}
