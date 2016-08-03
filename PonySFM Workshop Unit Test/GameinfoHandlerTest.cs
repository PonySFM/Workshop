using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonySFM_Workshop;

namespace PonySFM_Desktop_Unit_Testing
{
    [TestClass]
    public class GameinfoHandlerTest
    {
        /// <summary>
        /// Simplistic version of how the gameinfo.txt file looks. Should do it for testing.
        /// </summary>
        private readonly string gameinfoData =
            "\"GameInfo\"\n" +
            "{\n" +
            "   SearchPaths\n" +
            "   {\n" +
            "       Game    |gameinfo_path|.\n" +
            "       Game    tf_movies\n" +
            "       Game    tf\n" +
            "   }\n" +
            "}";

        private readonly string gameinfoDataComplete =
            "\"GameInfo\"\n" +
            "{\n" +
            "   SearchPaths\n" +
            "   {\n" +
            GameinfoHandler.GameinfoLine +
            "       Game    |gameinfo_path|.\n" +
            "       Game    tf_movies\n" +
            "       Game    tf\n" +
            "   }\n" +
            "}";

        /// <summary>
        /// Gameinfo data missing 'SearchPaths' key
        /// </summary>
        private readonly string gameinfoDataInvalid =
            "\"GameInfo\"\n" +
            "{\n" +
            "}";

        [TestMethod]
        public void TestCorrectExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(gameinfoData));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.Success, error);

            var newdata = Encoding.UTF8.GetString(fs.ReadFile("C:\\gameinfo.txt"));
            Assert.IsTrue(newdata.IndexOf(GameinfoHandler.GameinfoLine) != -1);
        }

        [TestMethod]
        public void TestFaultyExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(gameinfoDataInvalid));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.FileInvalid, error);
        }

        [TestMethod]
        public void TestNOPExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(gameinfoDataComplete));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            var error = handler.Execute();

            Assert.AreEqual(GameinfoHandlerError.AlreadyAdded, error);
        }

        [TestMethod]
        public void TestBackup()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(gameinfoData));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            handler.CreateBackup();

            Assert.IsTrue(fs.FileExists("C:\\gameinfo.txt.bak"));
            var backupdata = fs.ReadFile("C:\\gameinfo.txt.bak");

            Assert.AreEqual(gameinfoData, Encoding.UTF8.GetString(backupdata));

            /* Now test RestoreBackup */
            fs.DeleteFile("C:\\gameinfo.txt");

            handler.RestoreBackup();

            Assert.IsTrue(fs.FileExists("C:\\gameinfo.txt"));
        }
    }
}
