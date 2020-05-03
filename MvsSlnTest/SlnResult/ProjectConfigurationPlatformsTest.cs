using net.r_eg.MvsSln;
using Xunit;

namespace MvsSlnTest.Core
{
    [Collection("Sequential")] //TODO: msbuild GlobalProjectCollection
    public class ProjectConfigurationPlatformsTest
    {
        [Theory]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]

        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        public void ProjectGuidTheory1(string file, SlnItems items)
        {
            using(var sln = new Sln(file, items))
            {
                foreach(var pcfg in sln.Result.ProjectItemsConfigs)
                {
                    Assert.NotNull(pcfg.project.pGuid);
                }
            }
        }
    }
}
