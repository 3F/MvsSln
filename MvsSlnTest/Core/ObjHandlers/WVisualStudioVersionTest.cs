using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WVisualStudioVersionTest
    {
        [Fact]
        public void ExtractTest1()
        {
            string target = new WVisualStudioVersion
            (
                new SlnHeader()
                .SetFormatVersion("11.00")
                .SetProgramVersion("2010")
                .SetVisualStudioVersion("14.0.25420.1")
                .SetMinimumVersion("10.0.30319.1")
            )
            .Extract(data: null);

            Assert.Equal(SlnSamplesResource.Section_Header, target);
        }

        [Fact]
        public void ExtractTest2()
        {
            Assert.Equal
            (
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                new WVisualStudioVersion(new SlnHeader("12.0")).Extract(data: null)
            );
        }
    }
}
