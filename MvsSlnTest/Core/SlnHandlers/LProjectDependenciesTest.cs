using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.SlnHandlers;
using Xunit;

namespace MvsSlnTest.Core.SlnHandlers
{
    public class LProjectDependenciesTest
    {
        private const string EXIST_GUID     = "{11111111-1111-1111-1111-111111111111}";
        private const string EXIST_GUID2    = "{44444444-4444-4444-4444-444444444444}";
        private const string EXIST_GUID3    = "{77777777-7777-7777-7777-777777777777}";
        private const string NOTEXIST_GUID  = "{00000000-0000-0000-0000-000000000000}";

        [Fact]
        public void DataTest1()
        {
            var target = new SProjectsMap();
            Assert.Equal(EXIST_GUID, target.GuidList[0]);
            Assert.Equal(EXIST_GUID2, target.GuidList[1]);
            Assert.Equal(EXIST_GUID3, target.GuidList[2]);

            Assert.Equal("Project1", target.FirstProject.name);
            Assert.Equal("path\\to.sln", target.FirstProject.path);
            Assert.Equal(EXIST_GUID, target.FirstProject.pGuid);
            Assert.Equal("{22222222-2222-2222-2222-222222222222}", target.FirstProject.pType);

            Assert.Equal("Project3", target.LastProject.name);
            Assert.Equal("path\\to3.sln", target.LastProject.path);
            Assert.Equal(EXIST_GUID3, target.LastProject.pGuid);
            Assert.Equal("{55555555-5555-5555-5555-555555555555}", target.LastProject.pType);
        }

        [Fact]
        public void DataTest2()
        {
            var target = new SProjectsMap();

            Assert.Equal("Project3", target.FirstBy(BuildType.Clean).name);
            Assert.Equal("path\\to3.sln", target.FirstBy(BuildType.Clean).path);
            Assert.Equal(EXIST_GUID3, target.FirstBy(BuildType.Clean).pGuid);
            Assert.Equal("{55555555-5555-5555-5555-555555555555}", target.FirstBy(BuildType.Clean).pType);

            Assert.Equal("Project1", target.FirstBy(BuildType.Build).name);
            Assert.Equal("path\\to.sln", target.FirstBy(BuildType.Build).path);
            Assert.Equal(EXIST_GUID, target.FirstBy(BuildType.Build).pGuid);
            Assert.Equal("{22222222-2222-2222-2222-222222222222}", target.FirstBy(BuildType.Build).pType);
        }

        [Fact]
        public void DataTest3()
        {
            var target = new SProjectsMap();

            Assert.Equal("Project1", target.LastBy(BuildType.Clean).name);
            Assert.Equal("path\\to.sln", target.LastBy(BuildType.Clean).path);
            Assert.Equal(EXIST_GUID, target.LastBy(BuildType.Clean).pGuid);
            Assert.Equal("{22222222-2222-2222-2222-222222222222}", target.LastBy(BuildType.Clean).pType);

            Assert.Equal("Project3", target.LastBy(BuildType.Build).name);
            Assert.Equal("path\\to3.sln", target.LastBy(BuildType.Build).path);
            Assert.Equal(EXIST_GUID3, target.LastBy(BuildType.Build).pGuid);
            Assert.Equal("{55555555-5555-5555-5555-555555555555}", target.LastBy(BuildType.Build).pType);
        }

        [Fact]
        public void GetProjectByTest1()
        {
            var target = new SProjectsMap();
            Assert.Null(target.GetProjectBy(null).pGuid);
            Assert.Null(target.GetProjectBy(string.Empty).pGuid);
            Assert.Equal(EXIST_GUID, target.GetProjectBy(EXIST_GUID).pGuid);
            Assert.Equal(EXIST_GUID2, target.GetProjectBy(EXIST_GUID2).pGuid);
        }

        private class SProjectsMap: LProjectDependencies
        {
            public SProjectsMap()
            {
                Projects[EXIST_GUID] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID,
                    name    = "Project1",
                    path    = "path\\to.sln",
                    pType    = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID);

                Projects[EXIST_GUID2] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID2,
                    name    = "Project2",
                    path    = "path\\to2.sln",
                    pType    = "{22222222-2222-2222-2222-222222222222}",
                };
                order.Add(EXIST_GUID2);

                Projects[EXIST_GUID3] = new ProjectItem()
                {
                    pGuid   = EXIST_GUID3,
                    name    = "Project3",
                    path    = "path\\to3.sln",
                    pType    = "{55555555-5555-5555-5555-555555555555}",
                };
                order.Add(EXIST_GUID3);
            }
        }
    }
}
