using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class ProjectItemTest
    {
        [TestMethod]
        public void ParseTest1()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.AreEqual("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target.pGuid);
            Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", target.pType);
            Assert.AreEqual("Conari", target.name);
            Assert.AreEqual("Conari\\Conari.csproj", target.path);
            Assert.AreEqual($"{slnDir}Conari\\Conari.csproj", target.fullPath);
            Assert.AreEqual(ProjectType.Cs, target.EpType);
        }

        [TestMethod]
        public void ParseTest2()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.AreEqual(null, target.pGuid);
            Assert.AreEqual(null, target.pType);
            Assert.AreEqual(null, target.name);
            Assert.AreEqual(null, target.path);
            Assert.AreEqual(null, target.fullPath);
            Assert.AreEqual(ProjectType.Unknown, target.EpType);
        }

        [TestMethod]
        public void ParseTest3()
        {
            var target1 = new ProjectItem(null, "X:\\dir1\\");
            Assert.AreEqual(null, target1.pGuid);
            Assert.AreEqual(null, target1.fullPath);

            var target2 = new ProjectItem(
                "Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                null
            );

            Assert.AreEqual("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target2.pGuid);
            Assert.AreEqual("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}", target2.pType);
            Assert.AreEqual("Conari", target2.name);
            Assert.AreEqual("Conari\\Conari.csproj", target2.path);
            Assert.AreEqual(ProjectType.Vc, target2.EpType);
        }
    }
}
