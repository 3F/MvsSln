using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core.ObjHandlers;

namespace net.r_eg.MvsSlnTest.Core.ObjHandlers
{
    [TestClass]
    public class WExtensibilityGlobalsTest
    {
        [TestMethod]
        public void ExtractTest1()
        {
            var target = (new WExtensibilityGlobals(null)).Extract(null);

            Assert.AreEqual(String.Empty, target);
        }

        [TestMethod]
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

            Assert.AreEqual(SlnSamplesResource.Section_WExtensibilityGlobals_Test, target);
        }
    }
}
