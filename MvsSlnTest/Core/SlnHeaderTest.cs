using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class SlnHeaderTest
    {
        [Fact]
        public void DataTest1()
        {
            var data = new SlnHeader();
            data.SetFormatVersion("11.00");
            data.SetProgramVersion("2010");
            data.SetVisualStudioVersion("14.0.25420.1");
            data.SetMinimumVersion("10.0.30319.1");

            Assert.NotNull(data.FormatVersion);
            Assert.NotNull(data.ProgramVersion);
            Assert.NotNull(data.VisualStudioVersion);
            Assert.NotNull(data.MinimumVisualStudioVersion);

            Assert.Equal(11, data.FormatVersion.Major);
            Assert.Equal(0, data.FormatVersion.Minor);

            Assert.Equal("2010", data.ProgramVersion);

            Assert.Equal(14, data.VisualStudioVersion.Major);
            Assert.Equal(0, data.VisualStudioVersion.Minor);
            Assert.Equal(25420, data.VisualStudioVersion.Build);
            Assert.Equal(1, data.VisualStudioVersion.Revision);

            Assert.Equal(10, data.MinimumVisualStudioVersion.Major);
            Assert.Equal(0, data.MinimumVisualStudioVersion.Minor);
            Assert.Equal(30319, data.MinimumVisualStudioVersion.Build);
            Assert.Equal(1, data.MinimumVisualStudioVersion.Revision);
        }
    }
}
