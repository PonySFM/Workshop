using NUnit.Framework;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class SnakeCaseContractResolverTest
    {
        [Test]
        public void ResolvesCorrectly()
        {
            Assert.AreEqual(SnakeCaseContractResolver.GetSnakeCase("ID"), "id");
            Assert.AreEqual(SnakeCaseContractResolver.GetSnakeCase("ResourceID"), "resource_id");
        }
    }
}
