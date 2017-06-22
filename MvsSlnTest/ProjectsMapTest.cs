using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass]
    public class ProjectsMapTest
    {
        private const string EXIST_GUID     = "{11111111-1111-1111-1111-111111111111}";
        private const string EXIST_GUID2    = "{44444444-4444-4444-4444-444444444444}";
        private const string EXIST_GUID3    = "{77777777-7777-7777-7777-777777777777}";
        private const string NOTEXIST_GUID  = "{00000000-0000-0000-0000-000000000000}";

        [TestMethod]
        public void dataTest1()
        {
            var target = new SProjectsMap();
            Assert.AreEqual(EXIST_GUID, target.GuidList[0]);
            Assert.AreEqual(EXIST_GUID2, target.GuidList[1]);
            Assert.AreEqual(EXIST_GUID3, target.GuidList[2]);

            Assert.AreEqual("Project1", target.FirstProject.name);
            Assert.AreEqual("path\\to.sln", target.FirstProject.path);
            Assert.AreEqual(EXIST_GUID, target.FirstProject.pGuid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.FirstProject.type);

            Assert.AreEqual("Project3", target.LastProject.name);
            Assert.AreEqual("path\\to3.sln", target.LastProject.path);
            Assert.AreEqual(EXIST_GUID3, target.LastProject.pGuid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.LastProject.type);
        }

        [TestMethod]
        public void dataTest2()
        {
            var target = new SProjectsMap();

            Assert.AreEqual("Project3", target.FirstBy(BuildType.Clean).name);
            Assert.AreEqual("path\\to3.sln", target.FirstBy(BuildType.Clean).path);
            Assert.AreEqual(EXIST_GUID3, target.FirstBy(BuildType.Clean).pGuid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.FirstBy(BuildType.Clean).type);

            Assert.AreEqual("Project1", target.FirstBy(BuildType.Build).name);
            Assert.AreEqual("path\\to.sln", target.FirstBy(BuildType.Build).path);
            Assert.AreEqual(EXIST_GUID, target.FirstBy(BuildType.Build).pGuid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.FirstBy(BuildType.Build).type);
        }

        [TestMethod]
        public void dataTest3()
        {
            var target = new SProjectsMap();

            Assert.AreEqual("Project1", target.LastBy(BuildType.Clean).name);
            Assert.AreEqual("path\\to.sln", target.LastBy(BuildType.Clean).path);
            Assert.AreEqual(EXIST_GUID, target.LastBy(BuildType.Clean).pGuid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.LastBy(BuildType.Clean).type);

            Assert.AreEqual("Project3", target.LastBy(BuildType.Build).name);
            Assert.AreEqual("path\\to3.sln", target.LastBy(BuildType.Build).path);
            Assert.AreEqual(EXIST_GUID3, target.LastBy(BuildType.Build).pGuid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.LastBy(BuildType.Build).type);
        }

        private class SProjectsMap: SlnProjectDependencies
        {
            public SProjectsMap()
            {
                Projects[EXIST_GUID] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID,
                    name    = "Project1",
                    path    = "path\\to.sln",
                    type    = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID);

                Projects[EXIST_GUID2] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID2,
                    name    = "Project2",
                    path    = "path\\to2.sln",
                    type    = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID2);

                Projects[EXIST_GUID3] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID3,
                    name    = "Project3",
                    path    = "path\\to3.sln",
                    type    = "{55555555-5555-5555-5555-555555555555}",
                };
                order.Add(EXIST_GUID3);
            }
        }
    }
}
