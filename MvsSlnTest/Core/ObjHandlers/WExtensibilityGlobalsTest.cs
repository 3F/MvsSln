using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WExtensibilityGlobalsTest
    {
        [Fact]
        public void ExtractTest1()
        {
            var target = new WExtensibilityGlobals(null).Extract(null);

            Assert.Equal(String.Empty, target);
        }

        [Fact]
        public void ExtractTest2()
        {
            var actual = new Dictionary<string, string>()
            {
                { "SolutionGuid", "{B3244B90-20DE-4D69-8692-EBC686503F90}" },
                { "SomeOtherEmptyData", "" },
                { "SomeNullData", null },
                { "EnterpriseLibraryConfigurationToolBinariesPath", @"packages\Conari.1.3.0\lib\NET40;packages\vsSBE.CI.MSBuild\bin" }
            };
            var target = (new WExtensibilityGlobals(actual)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_WExtensibilityGlobals_Test, target);
        }
    }
}
