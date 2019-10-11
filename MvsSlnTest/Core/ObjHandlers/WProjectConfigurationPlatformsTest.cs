using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WProjectConfigurationPlatformsTest
    {
        [Fact]
        public void ExtractTest1()
        {
            var data = new List<IConfPlatformPrj>() {
                new ConfigPrj("Release_net45|Any CPU", "{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", true, new ConfigSln("CI_Debug_net45|Any CPU")),
                new ConfigPrj("Release|Any CPU", "{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", false, new ConfigSln("CI_Debug|Any CPU")),
                new ConfigPrj("CI_Release_net45|Any CPU", "{32E44995-F621-4E77-B46A-8F65D64E7FEA}", true, new ConfigSln("CI_Release_net45|Any CPU")),
                new ConfigPrj("Debug|Any CPU", "{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}", true, new ConfigSln("Debug|Any CPU")),
            };

            var target = (new WProjectConfigurationPlatforms(data)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_Prj_Config, target);
        }
    }
}
