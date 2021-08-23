using System;
using System.Collections.Generic;
using net.r_eg.MvsSln;
using Xunit;

namespace MvsSlnTest
{
    public class SlnItemsTest
    {
        public static IEnumerable<object[]> GetAnSlnItemsAll()
        {
            yield return new object[] { SlnItems.All, SlnItems.LoadMinimalDefaultData };

            yield return new object[] { SlnItems.AllMinimal, SlnItems.LoadDefaultData, 
                                                             SlnItems.PackagesConfigLegacy };

            yield return new object[] { SlnItems.AllNoLoad, SlnItems.LoadDefaultData, 
                                                            SlnItems.LoadMinimalDefaultData, 
                                                            SlnItems.PackagesConfigLegacy, 
                                                            SlnItems.PackagesConfigSolution };
        }

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
                        Assert.False(true, $"`{input}` is not completed. Found `{v}`");
                    }
                }
            }
        }

        [Fact]
        public void ItemsTest1()
        {
            Assert.Equal<uint>(0, (uint)SlnItems.None);
        }
    }
}
