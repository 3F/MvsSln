using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class XProjectEnv
    {
        [Fact]
        public void SlnPropertiesTest()
        {
            var cfgsln = new Dictionary<string, string>() { { PropertyNames.CONFIG, "Debug" }, { PropertyNames.PLATFORM, "Any CPU" } };
            var cfgprj = new Dictionary<string, string>() { { PropertyNames.CONFIG, "DBGprj" }, { PropertyNames.PLATFORM, "Win32" } };
            var cfg = new ConfigItem("DBGprj", "Win32");

            using(var sln = new Sln(TestData.PathTo(@"XProjectEnv\slnProperties\Cpp\App.sln"), SlnItems.Env))
            {
                var env = new XProjectEnvStub(sln.Result, cfgsln);
                env.XProjectByFile(sln.Result.ProjectItems.First().fullPath, cfg, cfgprj);

                Assert.Equal("Debug", cfgsln[PropertyNames.CONFIG]);
                Assert.Equal("Any CPU", cfgsln[PropertyNames.PLATFORM]);

                Assert.Equal("DBGprj", cfgprj[PropertyNames.CONFIG]);
                Assert.Equal("Win32", cfgprj[PropertyNames.PLATFORM]);

                Assert.Equal("DBGprj", cfg.Configuration);
                Assert.Equal("Win32", cfg.Platform);

                Assert.Equal(cfgsln[PropertyNames.CONFIG], env.SlnProperties[PropertyNames.CONFIG]);
                Assert.Equal(cfgsln[PropertyNames.PLATFORM], env.SlnProperties[PropertyNames.PLATFORM]);
            }
        }

        /// <summary>
        /// Verifies that project instances are valid for the active solution configuration in XProject Environment.
        /// Related problem: https://github.com/3F/vsSolutionBuildEvent/pull/77
        /// </summary>
        [Theory]
        [InlineData("Debug", "Any CPU", SlnItems.Env)]
        [InlineData("Release", "Any CPU", SlnItems.Env)]
        [InlineData("Debug", "Any CPU", SlnItems.EnvWithProjects)]
        [InlineData("Release", "Any CPU", SlnItems.EnvWithProjects)]
        [InlineData("Debug", "Any CPU", SlnItems.EnvWithMinimalProjects)]
        [InlineData("Release", "Any CPU", SlnItems.EnvWithMinimalProjects)]
        public void CorrectProjectInstnacesTest(string configuration, string platform, SlnItems opt)
        {
            using Sln sln = new(TestData.PathTo(@"XProjectEnv\projectInstnaces\ClassLibrary1.sln"), opt);
            ISlnResult l = sln.Result;

            ConfigItem input = new(configuration, platform);
            ProjectItem prj = l.ProjectItems.FirstOrDefault();

            IXProject xp = l.Env.XProjectByFile
            (
                prj.fullPath, 
                input, 
                new Dictionary<string, string>() { { PropertyNames.CONFIG, configuration }, { PropertyNames.PLATFORM, platform } }
            );

            Assert.True(input.Equals(xp.ProjectItem.projectConfig));

            Assert.Equal
            (
                input,
                new(xp.Project.GlobalProperties[PropertyNames.CONFIG], xp.Project.GlobalProperties[PropertyNames.PLATFORM])
            );

            var p = l.Env.GetOrLoadProject
            (
                l.ProjectItems.FirstOrDefault(),
                l.ProjectItemsConfigs
                    .FirstOrDefault(p => input.Equals(p.solutionConfig) == true)
                    .projectConfig
            );

            Assert.Equal
            (
                new(p.GlobalProperties[PropertyNames.CONFIG], p.GlobalProperties[PropertyNames.PLATFORM]),
                input
            );
        }
    }
}
