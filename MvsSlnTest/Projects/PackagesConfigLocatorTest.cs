using System;
using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class PackagesConfigLocatorTest
    {
        [Theory]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfig)]

        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfigSolution)]

        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfigLegacy)]
        public void FindAndLoadTest1(string file, SlnItems items)
        {
            using Sln sln = new(file, items);
            IEnumerable<PackagesConfig> pkgs = PackagesConfigLocator.FindAndLoadConfigs(sln.Result, sln.Result.ResultType);

            Assert.Equal(sln.Result.PackagesConfigs.Count(), pkgs.Count());

            int idx = 0;
            foreach(var config in sln.Result.PackagesConfigs)
            {
                Assert.Equal(config.Packages, pkgs.ElementAt(idx++).Packages);
            }
        }

        [Theory]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfig)]

        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfigSolution)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfigSolution)]

        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfigLegacy)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln", SlnItems.PackagesConfigLegacy)]
        public void FindTest1(string file, SlnItems items)
        {
            using Sln sln = new(file, items);
            IEnumerable<string> pkgs = PackagesConfigLocator.FindConfigs(sln.Result, sln.Result.ResultType);

            Assert.Equal(sln.Result.PackagesConfigs.Count(), pkgs.Count());

            int idx = 0;
            foreach(var config in sln.Result.PackagesConfigs)
            {
                Assert.Equal(config.File, pkgs.ElementAt(idx++));
            }
        }

        [Fact]
        public void NullTest1()
        {
            Assert.Throws<ArgumentNullException>(() => 
                PackagesConfigLocator.FindAndLoadConfigs(null, SlnItems.PackagesConfig).ToArray()
            );

            Assert.Throws<ArgumentNullException>(() => 
                PackagesConfigLocator.FindConfigs(null, SlnItems.PackagesConfig).ToArray()
            );
        }
    }
}
