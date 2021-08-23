using System.IO;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest
{
    public class SlnPackagesConfigTest
    {
        [Theory]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln", SlnItems.PackagesConfigSolution)]

        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfig)]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln", SlnItems.PackagesConfigSolution)]
        public void SlnPackagesConfigTest1(string input, SlnItems items)
        {
            using Sln l = new(input, items);

            Assert.Single(l.Result.PackagesConfigs);
            PackagesConfig pkg = l.Result.PackagesConfigs.First();

            Assert.Equal(2, pkg.Packages.Count());

            IPackageInfo info = pkg.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info.Version);
            Assert.Equal("net472", info.MetaTFM);

            IPackageInfo info2 = pkg.GetPackage("vsSolutionBuildEvent");
            Assert.Equal("1.14.1.1", info2.Version);
            Assert.Equal("vsSBE", info2.MetaOutput);
        }

        [Theory]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\1\test.sln")]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\2\test.sln")]
        public void SlnPackagesConfigTest2(string input)
        {
            using Sln l = new(input, SlnItems.PackagesConfigLegacy);
            Assert.Empty(l.Result.PackagesConfigs);

            using Sln l2 = new(input, SlnItems.Projects);
            Assert.Empty(l.Result.PackagesConfigs);
        }

        [Fact]
        public void SlnPackagesConfigTest3()
        {
            using Sln l = new(TestData.ROOT + @"PackagesConfig\sln\3\test.sln", SlnItems.PackagesConfigLegacy);

            Assert.Single(l.Result.PackagesConfigs);
            PackagesConfig pkg = l.Result.PackagesConfigs.First();

            Assert.Single(pkg.Packages);

            IPackageInfo info = pkg.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info.Version);
            Assert.Equal("net472", info.MetaTFM);
        }

        [Theory]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\3\test.sln")]
        [InlineData(TestData.ROOT + @"PackagesConfig\sln\4\test.sln")]
        public void SlnPackagesConfigTest4(string input)
        {
            using Sln l = new(input, SlnItems.PackagesConfigSolution);

            Assert.Single(l.Result.PackagesConfigs);
            PackagesConfig pkg = l.Result.PackagesConfigs.First();

            Assert.Single(pkg.Packages);

            IPackageInfo info = pkg.GetPackage("vsSolutionBuildEvent");
            Assert.Equal("1.14.1.1", info.Version);
            Assert.Equal("vsSolutionBuildEvent", info.MetaOutput);
        }

        [Fact]
        public void SlnPackagesConfigTest5()
        {
            const string _IN_DIR = @"PackagesConfig\sln\3\";
            const string _IN_PKG_LEGACY = _IN_DIR + @"packages\";

            using Sln l = new(TestData.ROOT + _IN_DIR + "test.sln", SlnItems.PackagesConfigSolution | SlnItems.PackagesConfigLegacy);

            Assert.Equal(2, l.Result.PackagesConfigs.Count());
            PackagesConfig pkg1 = l.Result.PackagesConfigs.First(p => !p.File.Contains(_IN_PKG_LEGACY));
            PackagesConfig pkg2 = l.Result.PackagesConfigs.First(p => p.File.Contains(_IN_PKG_LEGACY));

            Assert.Single(pkg1.Packages);

            IPackageInfo info = pkg1.GetPackage("vsSolutionBuildEvent");
            Assert.Equal("1.14.1.1", info.Version);
            Assert.Equal("vsSolutionBuildEvent", info.MetaOutput);

            Assert.Single(pkg2.Packages);

            IPackageInfo info2 = pkg2.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info2.Version);
            Assert.Equal("net472", info2.MetaTFM);
        }

        [Fact]
        public void SlnPackagesConfigTest6()
        {
            const string _IN_DIR = @"PackagesConfig\sln\4\";
            const string _IN_PKG_LEGACY = _IN_DIR + @"packages\";

            using Sln l = new(TestData.ROOT + _IN_DIR + "test.sln", SlnItems.PackagesConfigLegacy);

            Assert.Equal(3, l.Result.PackagesConfigs.Count());
            Assert.Equal(2, l.Result.ProjectItems.Count());

            PackagesConfig pkg1 = l.Result.PackagesConfigs.First(p => 
                p.File.Contains(Path.GetDirectoryName(l.Result.ProjectItems.ElementAt(0).fullPath))
            );

            PackagesConfig pkg2 = l.Result.PackagesConfigs.First(p =>
                p.File.Contains(Path.GetDirectoryName(l.Result.ProjectItems.ElementAt(1).fullPath))
            );

            PackagesConfig pkg3 = l.Result.PackagesConfigs.First(p =>
                p.File.Contains(_IN_PKG_LEGACY)
            );


            Assert.Single(pkg1.Packages);

            IPackageInfo info1 = pkg1.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info1.Version);
            Assert.Equal("net45", info1.MetaTFM);

            Assert.Single(pkg2.Packages);

            IPackageInfo info2 = pkg2.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info2.Version);
            Assert.Equal("net40", info2.MetaTFM);

            Assert.Single(pkg3.Packages);

            IPackageInfo info3 = pkg3.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info3.Version);
            Assert.Equal("net472", info3.MetaTFM);
        }

        [Fact]
        public void SlnPackagesConfigTest7()
        {
            const string _IN_DIR = @"PackagesConfig\sln\4\";
            const string _IN_PKG_LEGACY = _IN_DIR + @"packages\";

            using Sln l = new(TestData.ROOT + _IN_DIR + "test.sln", SlnItems.PackagesConfigSolution | SlnItems.PackagesConfigLegacy);

            Assert.Equal(4, l.Result.PackagesConfigs.Count());
            Assert.Equal(2, l.Result.ProjectItems.Count());

            PackagesConfig pkg1 = l.Result.PackagesConfigs.First(p => 
                p.File.Contains(Path.GetDirectoryName(l.Result.ProjectItems.ElementAt(0).fullPath))
            );

            PackagesConfig pkg2 = l.Result.PackagesConfigs.First(p =>
                p.File.Contains(Path.GetDirectoryName(l.Result.ProjectItems.ElementAt(1).fullPath))
            );

            PackagesConfig pkg3 = l.Result.PackagesConfigs.First(p =>
                p.File.Contains(_IN_PKG_LEGACY)
            );


            Assert.Single(pkg1.Packages);

            IPackageInfo info1 = pkg1.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info1.Version);
            Assert.Equal("net45", info1.MetaTFM);

            Assert.Single(pkg2.Packages);

            IPackageInfo info2 = pkg2.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info2.Version);
            Assert.Equal("net40", info2.MetaTFM);

            Assert.Single(pkg3.Packages);

            IPackageInfo info3 = pkg3.GetPackage("LX4Cnh");
            Assert.Equal("1.1.0", info3.Version);
            Assert.Equal("net472", info3.MetaTFM);


            PackagesConfig pkg4 = l.Result.PackagesConfigs.First(p =>
                !p.File.Contains(_IN_PKG_LEGACY)
            );

            Assert.Single(pkg4.Packages);

            IPackageInfo info4 = pkg4.GetPackage("vsSolutionBuildEvent");
            Assert.Equal("1.14.1.1", info4.Version);
            Assert.Equal("vsSolutionBuildEvent", info4.MetaOutput);
        }
    }
}
