using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WSolutionConfigurationPlatformsTest
    {
        [Fact]
        public void ExtractTest1()
        {
            var data = new List<IConfPlatform>()
            {
                new ConfigSln("CI_Debug_net45", "Any CPU"),
                new ConfigSln("CI_Debug", "Any CPU"),
                new ConfigSln("CI_Release_net45", "Any CPU"),
                new ConfigSln("CI_Release", "x86"),
                new ConfigSln("Debug_net45", "Any CPU"),
                new ConfigSln("Debug", "Any CPU"),
                new ConfigSln("Release_net45", "Any CPU"),
                new ConfigSln("Release", "x64"),
            };

            var target = (new WSolutionConfigurationPlatforms(data)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_Sln_Config, target);
        }
    }
}
