using System;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class SlnHeaderTest
    {
        [Theory]
        [InlineData("12.00", "17.0.31912.275", "17")]
        [InlineData("11.00", "17.0.31912.275")]
        [InlineData("10.00", "10.0.12345.2", "2010")]
        public void CtorTest1(string fVersion, string visualStudio, string program = null)
        {
            SlnHeader header = new(fVersion, visualStudio, program);

            Assert.Equal(new Version(fVersion), header.FormatVersion);
            Assert.Equal(fVersion, $"{header.FormatVersion.ToString(2)}0");
            Assert.Equal(fVersion, header.FormatVersionMajorMinor);
            Assert.Equal(visualStudio, header.VisualStudioVersion.ToString());
            Assert.Equal(SlnHeader.VisualStudio10_0_40219_1, header.MinimumVisualStudioVersion);
            Assert.Equal(program ?? header.VisualStudioVersion.Major.ToString(), header.ProgramVersion);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("2010")]
        public void CtorTest2(string program)
        {
            Version min = new(10, 4, 20, 2);
            SlnHeader header = new("11.0", "11.1", min, program);

            Assert.Equal("11.00", header.FormatVersionMajorMinor);
            Assert.Equal("11.1", header.VisualStudioVersion.ToString());
            Assert.Equal(min, header.MinimumVisualStudioVersion);
            Assert.Equal(program ?? header.VisualStudioVersion.Major.ToString(), header.ProgramVersion);
        }

        [Theory]
        [InlineData("12.10", "17.0.31912", "10.4.2.1", "17")]
        [InlineData("11.01", "17.0.31912", "11.0", "11")]
        [InlineData("10.00", "10.0", "10.1.2", "2010")]
        public void CtorTest3(string fVersion, string visualStudio, string min, string program)
        {
            SlnHeader header = new(fVersion, visualStudio, min, program);

            Assert.Equal(new Version(fVersion), header.FormatVersion);
            Assert.Equal(fVersion, string.Format("{0}.{1:00}", header.FormatVersion.Major, header.FormatVersion.Minor));
            Assert.Equal(fVersion, header.FormatVersionMajorMinor);
            Assert.Equal(visualStudio, header.VisualStudioVersion.ToString());
            Assert.Equal(min, header.MinimumVisualStudioVersion.ToString());
            Assert.Equal(program ?? header.VisualStudioVersion.Major.ToString(), header.ProgramVersion);
        }

        [Theory]
        [InlineData("12.00")]
        public void CtorTest4(string formatVersion)
        {
            SlnHeader header = new(formatVersion);

            Assert.Equal(new Version(formatVersion), header.FormatVersion);
            Assert.Equal(formatVersion, header.FormatVersionMajorMinor);
            Assert.Null(header.VisualStudioVersion);
            Assert.Null(header.MinimumVisualStudioVersion);
            Assert.Null(header.ProgramVersion);

            Assert.Throws<ArgumentNullException>(() => new SlnHeader(formatVersion: null));
        }

        [Fact]
        public void MakeDefaultTest1()
        {
            SlnHeader h = SlnHeader.MakeDefault();

            Assert.InRange(h.FormatVersion, new Version("11.0"), new Version("99.99"));
            Assert.InRange(h.VisualStudioVersion, new Version("17.0"), new Version("99.99"));
            Assert.InRange(h.MinimumVisualStudioVersion, new Version("10.0"), h.VisualStudioVersion);
            Assert.Equal(h.VisualStudioVersion.Major.ToString(), h.ProgramVersion);
        }

        [Fact]
        public void DataTest1()
        {
            SlnHeader data = new();

            Assert.Null(data.FormatVersionMajorMinor);

            data.SetFormatVersion("11.00")
                .SetProgramVersion("2010")
                .SetVisualStudioVersion("14.0.25420.1")
                .SetMinimumVersion("10.0.30319.1");

            Assert.NotNull(data.FormatVersion);
            Assert.NotNull(data.FormatVersionMajorMinor);
            Assert.NotNull(data.ProgramVersion);
            Assert.NotNull(data.VisualStudioVersion);
            Assert.NotNull(data.MinimumVisualStudioVersion);

            Assert.Equal(11, data.FormatVersion.Major);
            Assert.Equal(0, data.FormatVersion.Minor);
            Assert.Equal("11.00", data.FormatVersionMajorMinor);

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
