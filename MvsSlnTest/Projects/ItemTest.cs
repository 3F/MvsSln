using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class ItemTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target1 = new Item() { unevaluatedInclude = "System.Core", evaluatedInclude = "System.Xml" };
            Assert.Equal("System.Xml", target1.Assembly.Info.Name);

            var target2 = new Item() { unevaluatedInclude = "System.Core" };
            Assert.Equal("System.Core", target2.Assembly.Info.Name);

            var target3 = new Item() { evaluatedInclude = "System.Xml" };
            Assert.Equal("System.Xml", target3.Assembly.Info.Name);
        }

        [Fact]
        public void ParseTest2()
        {
            var target1 = new Item() { evaluatedInclude = "packages\\DllExport.dll" };
            Assert.Null(target1.Assembly.Info);

            var target2 = new Item() { evaluatedInclude = "packages/DllExport.dll" };
            Assert.Null(target2.Assembly.Info);

            var target3 = new Item();
            Assert.Null(target3.Assembly.Info);
        }

        [Fact]
        public void ParseTest3()
        {
            var target1 = new Item() {
                evaluatedInclude = "DllExport, Version=1.5.2.34258, Culture=neutral, PublicKeyToken=8337224c9ad9e356"
            };

            Assert.Equal("DllExport, Version=1.5.2.34258, Culture=neutral, PublicKeyToken=8337224c9ad9e356", target1.Assembly.Info.FullName);
            Assert.Equal("DllExport", target1.Assembly.Info.Name);
        }
    }
}
