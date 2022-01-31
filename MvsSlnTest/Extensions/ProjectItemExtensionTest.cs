using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    public class ProjectItemExtensionTest
    {
        [Theory]
        [InlineData(true, ProjectType.CsSdk)]
        [InlineData(true, ProjectType.FsSdk)]
        [InlineData(true, ProjectType.VbSdk)]
        [InlineData(false, ProjectType.Cs)]
        [InlineData(false, ProjectType.Fs)]
        [InlineData(false, ProjectType.Vb)]
        [InlineData(false, ProjectType.Vc)]
        public void CheckProjectTypesTest1(bool flag, ProjectType type)
        {
            ProjectItem prj = new("", type);
            Assert.True(prj.IsSdk() == flag);
        }

        [Theory]
        [InlineData(ProjectType.Cs)]
        [InlineData(ProjectType.CsSdk)]
        public void CheckProjectTypeCsTest(ProjectType type)
        {
            ProjectItem prj = new("", type);
            Assert.False(prj.IsVc());
            Assert.True(prj.IsCs());
            Assert.False(prj.IsFs());
            Assert.False(prj.IsVb());
        }

        [Theory]
        [InlineData(ProjectType.Fs)]
        [InlineData(ProjectType.FsSdk)]
        public void CheckProjectTypeFsTest(ProjectType type)
        {
            ProjectItem prj = new("", type);
            Assert.False(prj.IsVc());
            Assert.False(prj.IsCs());
            Assert.True(prj.IsFs());
            Assert.False(prj.IsVb());
        }

        [Theory]
        [InlineData(ProjectType.Vb)]
        [InlineData(ProjectType.VbSdk)]
        public void CheckProjectTypeVbTest(ProjectType type)
        {
            ProjectItem prj = new("", type);
            Assert.False(prj.IsVc());
            Assert.False(prj.IsCs());
            Assert.False(prj.IsFs());
            Assert.True(prj.IsVb());
        }

        [Fact]
        public void CheckProjectTypeVcTest()
        {
            ProjectItem prj = new("", ProjectType.Vc);
            Assert.True(prj.IsVc());
            Assert.False(prj.IsCs());
            Assert.False(prj.IsFs());
            Assert.False(prj.IsVb());
        }
    }
}
