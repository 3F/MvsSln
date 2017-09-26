using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class SlnHeaderTest
    {
        [TestMethod]
        public void DataTest1()
        {
            var data = new SlnHeader();
            data.SetFormatVersion("11.00");
            data.SetProgramVersion("2010");
            data.SetVisualStudioVersion("14.0.25420.1");
            data.SetMinimumVersion("10.0.30319.1");

            Assert.AreNotEqual(null, data.FormatVersion);
            Assert.AreNotEqual(null, data.ProgramVersion);
            Assert.AreNotEqual(null, data.VisualStudioVersion);
            Assert.AreNotEqual(null, data.MinimumVisualStudioVersion);

            Assert.AreEqual(11, data.FormatVersion.Major);
            Assert.AreEqual(0, data.FormatVersion.Minor);

            Assert.AreEqual("2010", data.ProgramVersion);

            Assert.AreEqual(14, data.VisualStudioVersion.Major);
            Assert.AreEqual(0, data.VisualStudioVersion.Minor);
            Assert.AreEqual(25420, data.VisualStudioVersion.Build);
            Assert.AreEqual(1, data.VisualStudioVersion.Revision);

            Assert.AreEqual(10, data.MinimumVisualStudioVersion.Major);
            Assert.AreEqual(0, data.MinimumVisualStudioVersion.Minor);
            Assert.AreEqual(30319, data.MinimumVisualStudioVersion.Build);
            Assert.AreEqual(1, data.MinimumVisualStudioVersion.Revision);
        }
    }
}
