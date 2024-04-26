using System.Collections.Generic;
using System.IO;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest
{
    public class DefaultHandlersTest
    {
        [Fact]
        public void MakeFromTest1()
        {
            string file = TestData.GetPathTo(@"SlnWriter\DefaultHandlers\src.sln");
            using Sln sln = new(file, SlnItems.AllNoLoad);

            string exp = File.ReadAllText(file);

            using SlnWriter w = new(DefaultHandlers.MakeFrom(sln.Result));
            Assert.Equal(exp, w.WriteAsString(sln.Result.Map));

            using SlnWriter w2 = new(sln.Result);
            Assert.Equal(exp, w2.WriteAsString(sln.Result.Map));
        }

        [Fact]
        public void MakeSkeletonTest1()
        {
            List<ISection> skeleton = DefaultHandlers.MakeSkeleton();
            Assert.NotNull(skeleton);
            Assert.True(skeleton.Count > 0);
        }
    }
}
