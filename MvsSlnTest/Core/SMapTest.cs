using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.SlnHandlers;
using Xunit;

namespace MvsSlnTest.Core
{
    public class SMapTest
    {
        [Fact]
        public void AddRemoveTest1()
        {
            using Sln sln = new(TestData.GetPathTo(@"SlnWriter\L-13\src.sln"), SlnItems.AllNoLoad);
            SMap smap = sln.Result.Map;

            Assert.Equal(9, smap.Count);

            Assert.Equal(typeof(LVisualStudioVersion), smap[0].Handler.GetType());
            Assert.Equal(typeof(LVisualStudioVersion), smap[1].Handler.GetType());
            Assert.Equal(typeof(LVisualStudioVersion), smap[2].Handler.GetType());
            Assert.Equal(typeof(LVisualStudioVersion), smap[3].Handler.GetType());

            Assert.Equal(Keywords.Global, (string)smap[4].Raw);

            Assert.Equal(typeof(LExtensibilityGlobals), smap[5].Handler.GetType());
            Assert.Equal(typeof(LExtensibilityGlobals), smap[6].Handler.GetType());
            Assert.Equal(typeof(LExtensibilityGlobals), smap[7].Handler.GetType());

            Assert.Equal(Keywords.EndGlobal, (string)smap[8].Raw);

            smap.Add
            (
                SMap.AddType.After,
                typeof(LExtensibilityGlobals),
                [ new Section(null, "custom1"), new Section(null, "custom2")]
            );

            smap.Add
            (
                SMap.AddType.Before,
                typeof(LExtensibilityGlobals),
                new Section(null, "custom3")
            );

            Assert.Equal(12, smap.Count);

            Assert.Equal("custom1", (string)smap[9].Raw);
            Assert.Equal("custom2", (string)smap[10].Raw);
            Assert.Equal(Keywords.EndGlobal, (string)smap[11].Raw);

            Assert.Equal("custom3", (string)smap[5].Raw);

            sln.Result.Map.Remove("custom3");
            Assert.Equal(11, smap.Count);
            Assert.Equal(Keywords.Global, (string)smap[4].Raw);
            Assert.Equal(typeof(LExtensibilityGlobals), smap[5].Handler.GetType());

            smap.Add
            (
                SMap.AddType.After,
                Keywords.Global,
                new Section(null, "custom4")
            );

            Assert.Equal(12, smap.Count);
            Assert.Equal(Keywords.Global, (string)smap[4].Raw);
            Assert.Equal("custom4", (string)smap[5].Raw);

            smap.Add
            (
                SMap.AddType.Before,
                SMap.RawSectionType.Global,
                [new Section(null, "custom5"), new Section(null, "custom6")]
            );

            Assert.Equal(14, smap.Count);
            Assert.Equal(typeof(LVisualStudioVersion), smap[3].Handler.GetType());
            Assert.Equal("custom5", (string)smap[4].Raw);
            Assert.Equal("custom6", (string)smap[5].Raw);
            Assert.Equal(Keywords.Global, (string)smap[6].Raw);

            smap.Remove(typeof(LExtensibilityGlobals));
            Assert.Equal(11, smap.Count);
            Assert.Equal(Keywords.Global, (string)smap[6].Raw);
            Assert.Equal("custom4", (string)smap[7].Raw);
            Assert.Equal("custom1", (string)smap[8].Raw);
            Assert.Equal("custom2", (string)smap[9].Raw);
            Assert.Equal(Keywords.EndGlobal, (string)smap[10].Raw);

            smap.Add(SMap.AddType.After, 10, new Section(null, "custom7"));
            smap.Add(SMap.AddType.Before, 10, new Section(null, "custom8"));

            Assert.Equal(13, smap.Count);
            Assert.Equal("custom8", (string)smap[10].Raw);
            Assert.Equal(Keywords.EndGlobal, (string)smap[11].Raw);
            Assert.Equal("custom7", (string)smap[12].Raw);

            smap.Remove(SMap.RawSectionType.EndGlobal);
            Assert.Equal(12, smap.Count);
            Assert.Equal("custom7", (string)smap[11].Raw);

            smap.Clear();
            Assert.Empty(smap);
        }
    }
}
