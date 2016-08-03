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

        [TestMethod]
        public void TestExecution()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\gameinfo.txt", Encoding.UTF8.GetBytes(gameinfoData));
            var handler = new GameinfoHandler("C:\\gameinfo.txt", fs);

            handler.Execute();

            var newdata = Encoding.UTF8.GetString(fs.ReadFile("C:\\gameinfo.txt"));
            Assert.IsTrue(newdata.IndexOf(GameinfoHandler.GameinfoLine) != -1);
        }
    }
}
