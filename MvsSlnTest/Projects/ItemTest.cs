using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSlnTest.Projects
{
    [TestClass]
    public class ItemTest
    {
        [TestMethod]
        public void ParseTest1()
        {
            var target1 = new Item() { unevaluatedInclude = "System.Core", evaluatedInclude = "System.Xml" };
            Assert.AreEqual("System.Xml", target1.AssemblyInfo.Name);

            var target2 = new Item() { unevaluatedInclude = "System.Core" };
            Assert.AreEqual("System.Core", target2.AssemblyInfo.Name);

            var target3 = new Item() { evaluatedInclude = "System.Xml" };
            Assert.AreEqual("System.Xml", target3.AssemblyInfo.Name);
        }

        [TestMethod]
        public void ParseTest2()
        {
            var target1 = new Item() { evaluatedInclude = "packages\\DllExport.dll" };
            Assert.AreEqual(null, target1.AssemblyInfo);

            var target2 = new Item() { evaluatedInclude = "packages/DllExport.dll" };
            Assert.AreEqual(null, target2.AssemblyInfo);

            var target3 = new Item();
            Assert.AreEqual(null, target3.AssemblyInfo);
        }
    }
}
