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
            var data = new SlnHeader();
            data.SetFormatVersion("11.00");
            data.SetProgramVersion("2010");
            data.SetVisualStudioVersion("14.0.25420.1");
            data.SetMinimumVersion("10.0.30319.1");

            var target = (new WVisualStudioVersion(data)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_Header, target);
        }
    }
}
