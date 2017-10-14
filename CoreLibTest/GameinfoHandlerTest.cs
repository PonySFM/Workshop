using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestClass]
    public class GameinfoHandlerTest
    {
        /// <summary>
        /// Simplistic version of how the gameinfo.txt file looks. Should do it for testing.
        /// </summary>
        private const string GameinfoData =
            "\"GameInfo\"\n" +
            "{\n" +
            "   SearchPaths\n" +
            "   {\n" +
            "       Game    |gameinfo_path|.\n" +
            "       Game    tf_movies\n" +
            "       Game    tf\n" +
            "   }\n" +
            "}";

        private readonly string GameinfoDataComplete =
            "\"GameInfo\"\n" +
            "{\n" +
            "   SearchPaths\n" +
            "   {\n" +
            "       Game    |gameinfo_path|.\n" +
            "       Game    tf_movies\n" +
            "       Game    tf\n" +
            GameinfoHandler.GameinfoLine + "\n" +
            "   }\n" +
            "}\n";

        /// <summary>
        /// Gameinfo data missing 'SearchPaths' key
        /// </summary>
        private const string GameinfoDataInvalid =
            "\"GameInfo\"\n" +
            "{\n" +
            "}";

        [TestMethod]
        public void TestCorrectExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(GameinfoData));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.Success, error);

            var newdata = Encoding.UTF8.GetString(fs.ReadFile("C:\\gameinfo.txt")).Replace("\r", "");
            Assert.IsTrue(newdata.IndexOf(GameinfoHandler.GameinfoLine, StringComparison.Ordinal) != -1);
            Assert.AreEqual(GameinfoDataComplete, newdata);
        }

        [TestMethod]
        public void TestFaultyExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(GameinfoDataInvalid));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.FileInvalid, error);
        }

        [TestMethod]
        public void TestNOPExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(GameinfoDataComplete));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.AlreadyAdded, error);
        }

        [TestMethod]
        public void TestBackup()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(GameinfoData));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            handler.CreateBackup();

            Assert.IsTrue(fs.FileExists("C:\\gameinfo.txt.bak"));
            var backupdata = fs.ReadFile("C:\\gameinfo.txt.bak");

            Assert.AreEqual(GameinfoData, Encoding.UTF8.GetString(backupdata));

            /* Now test RestoreBackup */
            fs.DeleteFile("C:\\gameinfo.txt");

            handler.RestoreBackup();

            Assert.IsTrue(fs.FileExists("C:\\gameinfo.txt"));
        }
    }
}
