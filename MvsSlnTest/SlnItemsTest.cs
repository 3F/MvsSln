using System;
using net.r_eg.MvsSln;
using Xunit;

namespace MvsSlnTest
{
    public class SlnItemsTest
    {
#if !NET40
        public static TheoryData<SlnItems, SlnItems[]> GetAnSlnItemsAll() => new()
        {
            { SlnItems.All, [ SlnItems.LoadMinimalDefaultData ] },

            { SlnItems.AllMinimal, [ SlnItems.LoadDefaultData, SlnItems.PackagesConfigLegacy ] },

            { SlnItems.AllNoLoad, [ SlnItems.LoadDefaultData,
                                    SlnItems.LoadMinimalDefaultData,
                                    SlnItems.PackagesConfigLegacy,
                                    SlnItems.PackagesConfigSolution ] },
        };

        [Theory]
        [MemberData(nameof(GetAnSlnItemsAll))]
        public void AllItemsTest2(SlnItems input, params SlnItems[] ignoring)
        {
            foreach(var item in Enum.GetValues(typeof(SlnItems)))
            {
                SlnItems v = (SlnItems)item;
                if(!input.HasFlag(v))
                {
                    bool failed = true;
                    foreach(var ignore in ignoring)
                    {
                        if(v.HasFlag(ignore))
                        {
                            failed = false;
                            break;
                        }
                    }

                    if(failed)
                    {
                        Assert.Fail($"`{input}` is not completed. Found `{v}`");
                    }
                }
            }
        }
#endif

        [Fact]
        public void ItemsTest1()
        {
            Assert.Equal<uint>(0, (uint)SlnItems.None);
        }
    }
}
