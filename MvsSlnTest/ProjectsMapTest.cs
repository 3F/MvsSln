using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts.Components.Build;

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

            Assert.AreEqual("Project1", target.First.name);
            Assert.AreEqual("path\\to.sln", target.First.path);
            Assert.AreEqual(EXIST_GUID, target.First.guid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.First.type);

            Assert.AreEqual("Project3", target.Last.name);
            Assert.AreEqual("path\\to3.sln", target.Last.path);
            Assert.AreEqual(EXIST_GUID3, target.Last.guid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.Last.type);
        }

        [TestMethod]
        public void dataTest2()
        {
            var target = new SProjectsMap();

            Assert.AreEqual("Project3", target.FirstBy(Bridge.BuildType.Clean).name);
            Assert.AreEqual("path\\to3.sln", target.FirstBy(Bridge.BuildType.Clean).path);
            Assert.AreEqual(EXIST_GUID3, target.FirstBy(Bridge.BuildType.Clean).guid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.FirstBy(Bridge.BuildType.Clean).type);

            Assert.AreEqual("Project1", target.FirstBy(Bridge.BuildType.Build).name);
            Assert.AreEqual("path\\to.sln", target.FirstBy(Bridge.BuildType.Build).path);
            Assert.AreEqual(EXIST_GUID, target.FirstBy(Bridge.BuildType.Build).guid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.FirstBy(Bridge.BuildType.Build).type);
        }

        [TestMethod]
        public void dataTest3()
        {
            var target = new SProjectsMap();

            Assert.AreEqual("Project1", target.LastBy(Bridge.BuildType.Clean).name);
            Assert.AreEqual("path\\to.sln", target.LastBy(Bridge.BuildType.Clean).path);
            Assert.AreEqual(EXIST_GUID, target.LastBy(Bridge.BuildType.Clean).guid);
            Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.LastBy(Bridge.BuildType.Clean).type);

            Assert.AreEqual("Project3", target.LastBy(Bridge.BuildType.Build).name);
            Assert.AreEqual("path\\to3.sln", target.LastBy(Bridge.BuildType.Build).path);
            Assert.AreEqual(EXIST_GUID3, target.LastBy(Bridge.BuildType.Build).guid);
            Assert.AreEqual("{55555555-5555-5555-5555-555555555555}", target.LastBy(Bridge.BuildType.Build).type);
        }

        private class SProjectsMap: ProjectsMap
        {
            public SProjectsMap()
            {
                projects[EXIST_GUID] = new Project()
                {
                    guid = EXIST_GUID,
                    name = "Project1",
                    path = "path\\to.sln",
                    type = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID);

                projects[EXIST_GUID2] = new Project()
                {
                    guid = EXIST_GUID2,
                    name = "Project2",
                    path = "path\\to2.sln",
                    type = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID2);

                projects[EXIST_GUID3] = new Project()
                {
                    guid = EXIST_GUID3,
                    name = "Project3",
                    path = "path\\to3.sln",
                    type = "{55555555-5555-5555-5555-555555555555}",
                };
                order.Add(EXIST_GUID3);
            }
        }
    }
}
