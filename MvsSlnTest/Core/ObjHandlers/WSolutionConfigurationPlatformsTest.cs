using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;

namespace net.r_eg.MvsSlnTest.Core.ObjHandlers
{
    [TestClass]
    public class WSolutionConfigurationPlatformsTest
    {
        [TestMethod]
        public void ExtractTest1()
        {
            var data = new List<IConfPlatform>() {
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

            Assert.AreEqual(SlnSamplesResource.Section_Sln_Config, target);
        }
    }
}
