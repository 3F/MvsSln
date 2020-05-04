using System.Linq;
using net.r_eg.MvsSln;
using Xunit;

namespace MvsSlnTest.Core
{
    [Collection("Sequential")] //TODO: msbuild GlobalProjectCollection
    public class ProjectReferencesTest
    {
        [Theory]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependencies)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependenciesXml)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependencies)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependenciesXml)]
        public void ActivationTheory1(string file, SlnItems items)
        {
            using(var sln = new Sln(file, items))
            {
                var dep = sln.Result.ProjectDependencies.Dependencies;
                Assert.Equal(2, dep.Count);
                Assert.Empty(dep["{64AD76CA-2C85-4039-B0B3-734CF02B2999}"]);
                Assert.Empty(dep["{6CE57BB1-4A6D-4714-B775-74A3637EC992}"]);

                if((items & SlnItems.ProjectDependenciesXml) == SlnItems.ProjectDependenciesXml) {
                    Assert.Empty(sln.Result.Env.Projects);
                }
                else {
                    Assert.Null(sln.Result.Env);
                }
            }
        }

        [Theory]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]

        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\onlypath\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\onlypath\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]

        // Part of SlnParser.SetProjectItemsConfigs, see also tests in ProjectConfigurationPlatformsTest
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\projectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\noprojectguid\test2.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]

        // Part of Item.Metadata
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\metatag\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadMinimalDefaultData)]
        [InlineData(TestData.ROOT + @"ProjectDependenciesXml\metatag\test.sln", SlnItems.ProjectDependenciesXml | SlnItems.LoadDefaultData)]
        public void ActivationTheory2(string file, SlnItems items)
        {
            const string _P1 = "{64AD76CA-2C85-4039-B0B3-734CF02B2999}";
            const string _P2 = "{6CE57BB1-4A6D-4714-B775-74A3637EC992}";

            using(var sln = new Sln(file, items))
            {
                Assert.NotNull(sln.Result.Env);
                Assert.NotEmpty(sln.Result.Env.Projects);

                Assert.Equal(sln.Result.ProjectDependencies.Projects.Count, sln.Result.ProjectDependencies.Dependencies.Count);

                var dep = sln.Result.ProjectDependencies.Dependencies;
                Assert.Equal(2, dep.Count);
                Assert.Empty(dep[_P1]);
                Assert.Single(dep[_P2]);

                Assert.Equal(_P1, dep[_P2].First());
            }
        }
    }
}
