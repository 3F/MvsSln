using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    public class ObjectExtensionTest
    {
        [Fact]
        public void EReturnTest1()
        {
            string data = "12345  ";
            Assert.Equal(data, data.E(s => s.Trim()));

            void _upd(ref string input) => input = "9876";
            Assert.Equal(data, data.E(s => _upd(ref s)));

            const int _V = 14;
            var obj = new List<int>(1) { _V };
            Assert.NotEqual(_V, obj.E(v => v[0] = 8)[0]);
        }

        [Fact]
        public void EActTest1()
        {
            var x = new List<int>(1) { 0 };
            Assert.Equal(1, x.E(() => x[0]++)[0]);

            var y = new List<int>(1) { 0 };
            Assert.Equal(0, y.E(() => x[0]++)[0]);

            var z = new List<int>(1) { 0 };
            Assert.Equal(3, x.E(_x => _x[0]++)[0]);
        }
    }
}
