using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class RoPropertiesTest
    {
#if !NET40
        [Theory]
        [MemberData(nameof(GetDictData))]
        public void EqualsTest1(Dictionary<int, string> input, bool eq = false)
        {
            RoProperties<int, string> a = new(new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bbb" }, { 3, "ccc" } });

            RoProperties<int, string> b = new(input);
            if(eq)
            {
                Assert.True(a.Equals(b));
                Assert.True(a == b);
                Assert.False(a != b);
            }
            else
            {
                Assert.False(a.Equals(b));
                Assert.False(a == b);
                Assert.True(a != b);
            }
        }
#endif

        public static IEnumerable<object[]> GetDictData()
        {
            yield return [ new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bbb" }, { 3, "ccc" } }, true ];
            yield return [ new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bxb" }, { 3, "ccc" } } ];
            yield return [ new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bbb" }, { 4, "ccc" } } ];
            yield return [ new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bbb" }, { 3, "ccc" }, { 4, "ddd" } } ];
            yield return [ new Dictionary<int, string>() { { 1, "aaa" }, { 2, "bbb" } } ];
            yield return [ new Dictionary<int, string>() { { 1, "aa" }, { 2, "bbb" }, { 3, "ccc" } } ];
            yield return [ new Dictionary<int, string>() { { 1, "aaA" }, { 2, "bbb" }, { 3, "ccc" } } ];

#if FEATURE_EXACT_ROP_ORDER_CMP
            yield return [ new Dictionary<int, string>() { { 2, "bbb" }, { 1, "aaa" }, { 3, "ccc" } }, false ];
#else
            yield return [ new Dictionary<int, string>() { { 2, "bbb" }, { 1, "aaa" }, { 3, "ccc" } }, true ];
#endif
        }
    }
}
