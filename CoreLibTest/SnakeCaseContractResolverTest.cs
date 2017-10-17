using CoreLib.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreLibTest
{
    [TestClass]
    public class SnakeCaseContractResolverTest
    {
        [TestMethod]
        public void ResolvesCorrectly()
        {
            Assert.AreEqual(SnakeCaseContractResolver.GetSnakeCase("ID"), "id");
            Assert.AreEqual(SnakeCaseContractResolver.GetSnakeCase("ResourceID"), "resource_id");
        }
    }
}
