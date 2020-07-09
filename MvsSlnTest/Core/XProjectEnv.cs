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
    }
}
