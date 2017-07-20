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
            Assert.AreEqual("System.Xml", target1.Assembly.Info.Name);

            var target2 = new Item() { unevaluatedInclude = "System.Core" };
            Assert.AreEqual("System.Core", target2.Assembly.Info.Name);

            var target3 = new Item() { evaluatedInclude = "System.Xml" };
            Assert.AreEqual("System.Xml", target3.Assembly.Info.Name);
        }

        [TestMethod]
        public void ParseTest2()
        {
            var target1 = new Item() { evaluatedInclude = "packages\\DllExport.dll" };
            Assert.AreEqual(null, target1.Assembly.Info);

            var target2 = new Item() { evaluatedInclude = "packages/DllExport.dll" };
            Assert.AreEqual(null, target2.Assembly.Info);

            var target3 = new Item();
            Assert.AreEqual(null, target3.Assembly.Info);
        }

        [TestMethod]
        public void ParseTest3()
        {
            var target1 = new Item() {
                evaluatedInclude = "DllExport, Version=1.5.2.34258, Culture=neutral, PublicKeyToken=8337224c9ad9e356"
            };

            Assert.AreEqual("DllExport, Version=1.5.2.34258, Culture=neutral, PublicKeyToken=8337224c9ad9e356", target1.Assembly.Info.FullName);
            Assert.AreEqual("DllExport", target1.Assembly.Info.Name);
        }
    }
}
