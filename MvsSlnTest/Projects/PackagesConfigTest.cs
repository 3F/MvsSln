using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MvsSlnTest._svc;
using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class PackagesConfigTest
    {
        private static readonly PackagesConfigOptions customLoad    = PackagesConfigOptions.Load | PackagesConfigOptions.PathToStorage;
        private static readonly PackagesConfigOptions customNew     = PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.PathToStorage;

        public static IEnumerable<object[]> GetPackageInfo()
        {
            PackagesConfig s = new(TestData.ROOT + @"PackagesConfig\folder");

            yield return new object[] { s.GetPackage("LX4Cnh"), "LX4Cnh", "1.1.0", "net472" };

            PackagesConfig s2 = new(TestData.ROOT + @"PackagesConfig\packages.1.txt", customLoad);

            yield return new object[] { s2.Packages.ElementAt(0), "vsSolutionBuildEvent", "1.14.1.1", null, "vsSBE" };
            yield return new object[] { s2.GetPackage("vssolutionbuildevent", true), "vsSolutionBuildEvent", "1.14.1.1", null, "vsSBE" };
            yield return new object[] { s2.Packages.ElementAt(1), "Conari", "1.5", "net472" };

            PackagesConfig s3 = new(TestData.ROOT + @"PackagesConfig\packages.2.txt", customLoad);

            yield return new object[] { s3.GetPackage("EnvDTE"), "EnvDTE", "8.0.2", "net45" };
            yield return new object[] { s3.GetPackage("EnvDTE80"), "EnvDTE80", "8.0.3", null };
            yield return new object[] { s3.GetPackage("stdole"), "stdole", "7.0.3303", "net10" };
        }

        [Theory]
        [MemberData(nameof(GetPackageInfo))]
        public void ParsedItemsTest1(IPackageInfo p, string id, string version, string tfm, string output = null)
        {
            Assert.Equal(id, p.Id);
            Assert.Equal(version, p.Version);
            Assert.Equal(new Version(version), p.VersionParsed);

            if(tfm == null & output == null)
            {
                Assert.Empty(p.Meta);
            }
            else
            {
                Assert.NotNull(p.Meta);
                Assert.Equal(1, p.Meta.Count);

                if(tfm != null) Assert.Equal(tfm, p.Meta[PackageInfo.ATTR_TFM]);
                if(output != null) Assert.Equal(output, p.Meta[PackageInfo.ATTR_OUT]);
            }
        }

        [Fact]
        public void ParsedItemsTest2()
        {
            PackagesConfig pkg = new(TestData.ROOT + @"PackagesConfig\packages.1.txt", customLoad);

            Assert.Null(pkg.GetPackage(string.Empty));
            Assert.Null(pkg.GetPackage(string.Empty, true));
            Assert.Null(pkg.GetPackage(" "));
            Assert.Null(pkg.GetPackage(" ", true));
            Assert.Null(pkg.GetPackage("NotReal"));
            Assert.Null(pkg.GetPackage("NotReal", true));
            Assert.Null(pkg.GetPackage("vssolutionbuildevent"));
            Assert.NotNull(pkg.GetPackage("vssolutionbuildevent", true));
            Assert.Throws<ArgumentNullException>(() => pkg.GetPackage(null));
            Assert.Throws<ArgumentNullException>(() => pkg.GetPackage(null, true));
        }

        [Fact]
        public void LoadTest1()
        {
            PackagesConfig pkg = new(TestData.ROOT + @"PackagesConfig\packages.1.txt", customLoad);

            Assert.Null(pkg.FailedLoading);
            Assert.False(pkg.IsNew);
            Assert.False(string.IsNullOrEmpty(pkg.DefaultTfm));

            Assert.Equal(2, pkg.Packages.Count());
        }

        public static IEnumerable<object[]> GetFailedContent()
        {
            yield return new object[] { TestData.ROOT + @"PackagesConfig\packages.3.txt" };
            yield return new object[] { TestData.ROOT + @"PackagesConfig\packages.4.txt" };
        }

        [Theory]
        [MemberData(nameof(GetFailedContent))]
        public void LoadTest2(string input)
        {
            PackagesConfig pkg = new
            (
                input, 
                PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.SilentLoading | PackagesConfigOptions.PathToStorage
            );

            Assert.NotNull(pkg.FailedLoading);
            Assert.True(pkg.IsNew);

            Assert.Empty(pkg.Packages);
        }

        [Theory]
        [MemberData(nameof(GetFailedContent))]
        public void LoadTest3(string input)
        {
            Assert.Throws<XmlException>(() => new PackagesConfig
            (
                input,
                PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.PathToStorage
            ));
        }

        [Fact]
        public void LoadTest4()
        {
            PackagesConfig pkg = new
            (
                TestData.ROOT + @"PackagesConfig\packages.5.txt",
                PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.PathToStorage
            );

            Assert.Null(pkg.FailedLoading);
            Assert.False(pkg.IsNew);

            Assert.Empty(pkg.Packages);
        }

        [Fact]
        public void LoadTest5()
        {
            PackagesConfig pkg = new(TestData.ROOT + @"PackagesConfig\folder", PackagesConfigOptions.Load);
            Assert.False(pkg.IsNew);
            Assert.NotNull(pkg.GetPackage("LX4Cnh"));

            PackagesConfig pkg2 = new(TestData.ROOT + @"PackagesConfig\NotRealFolder", PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.SilentLoading);
            Assert.True(pkg2.IsNew);
            Assert.Null(pkg2.GetPackage("LX4Cnh"));
        }

        [Theory]
        [InlineData(PackagesConfigOptions.None)]
        [InlineData(PackagesConfigOptions.SilentLoading)]
        public void CtorTest1(PackagesConfigOptions silentLoading)
        {
            Assert.Throws<ArgumentNullException>(() => new PackagesConfig(null, PackagesConfigOptions.Default | silentLoading));

            Assert.Throws<NotSupportedException>(() => 
                new PackagesConfig(TestData.ROOT + @"PackagesConfig\packages.1.txt", PackagesConfigOptions.None | silentLoading)
            );

            Assert.Throws<FileNotFoundException>(() => 
            {
                string dst = TestData.ROOT + @"PackagesConfig\NotRealFolder";
                Assert.False(Directory.Exists(dst));

                new PackagesConfig(dst, PackagesConfigOptions.Load | silentLoading);
            });

            Assert.Throws<FileNotFoundException>(() =>
            {
                string dst = TestData.ROOT + @"PackagesConfig\folder";
                Assert.True(Directory.Exists(dst));

                new PackagesConfig(dst, PackagesConfigOptions.Load | PackagesConfigOptions.PathToStorage | silentLoading);
            });
        }

        [Fact]
        public void RemoveItemTest1()
        {
            const string _NAME = "EnvDTE";

            PackagesConfig pkg = new(TestData.ROOT + @"PackagesConfig\packages.2.txt", customLoad);

            Assert.NotNull(pkg.GetPackage(_NAME));
            pkg.GetPackage(_NAME).Remove();
            Assert.Null(pkg.GetPackage(_NAME));

            PackagesConfig pkg2 = new(TestData.ROOT + @"PackagesConfig\packages.2.txt", customLoad);
            Assert.NotNull(pkg2.GetPackage(_NAME));
        }

        [Fact]
        public void ModifyItemsTest1()
        {
            const string _FILE = TestData.ROOT + @"PackagesConfig\test.1.tmp";
            if(File.Exists(_FILE)) File.Delete(_FILE);

            using TempPackagesConfig pkg = new(_FILE, customNew);
            Assert.Equal(pkg.File, _FILE);
            Assert.Empty(pkg.Packages);

            const string _P1 = "LX4Cnh";
            const string _P2 = "Fnv1a128";
            const string _P3 = "Huid";

            // duplicates

            pkg.DefaultTfm = "net40";
            Assert.True(pkg.AddPackage(_P1, "1.1.0"));

            pkg.DefaultTfm = "net472";
            Assert.True(pkg.AddPackage(_P2, "1.0.0"));
            Assert.False(pkg.AddPackage(_P1, "1.2.0"));

            Assert.Equal("1.1.0", pkg.GetPackage(_P1).Version);

            // default TFM

            Assert.Equal("net40", pkg.GetPackage(_P1).Meta[PackageInfo.ATTR_TFM]);
            Assert.Equal("net472", pkg.GetPackage(_P2).Meta[PackageInfo.ATTR_TFM]);

            // commit

            Assert.False(File.Exists(_FILE));
            pkg.Commit();
            Assert.True(File.Exists(_FILE));

            // rollback

            Assert.Equal(2, pkg.Packages.Count());

            Assert.True(pkg.AddPackage(_P3, "1.0.0"));
            Assert.Equal("1.0.0", pkg.GetPackage(_P3).Version);

            Assert.Equal(3, pkg.Packages.Count());
            pkg.Rollback();
            Assert.Equal(2, pkg.Packages.Count());

            Assert.True(pkg.AddPackage(_P3, "2.0.0"));
            Assert.Equal("2.0.0", pkg.GetPackage(_P3).Version);

            Assert.Equal(3, pkg.Packages.Count());

            // re-load

            PackagesConfig pkg2 = new(_FILE, customNew);
            Assert.Equal(pkg2.File, _FILE);
            Assert.Equal(2, pkg2.Packages.Count());

            Assert.Null(pkg2.GetPackage(_P3));
            Assert.NotNull(pkg.GetPackage(_P1));

            Assert.Equal("1.1.0", pkg.GetPackage(_P1).Version);
        }

        [Fact]
        public void ModifyItemsTest2()
        {
            const string _FILE = TestData.ROOT + @"PackagesConfig\test.2.tmp";
            if(File.Exists(_FILE)) File.Delete(_FILE);

            using TempPackagesConfig pkg = new(_FILE, customNew);
            Assert.Equal(pkg.File, _FILE);
            Assert.Empty(pkg.Packages);

            const string _P1 = "LX4Cnh";

            Assert.True(pkg.AddOrUpdatePackage(_P1, "1.0.0"));
            Assert.Equal("1.0.0", pkg.GetPackage(_P1).Version);
            Assert.Equal(pkg.DefaultTfm, pkg.GetPackage(_P1).Meta[PackageInfo.ATTR_TFM]);

            Assert.Single(pkg.Packages);

            Assert.False(pkg.AddOrUpdatePackage(_P1, "1.1.0", "net472"));
            Assert.Equal("1.1.0", pkg.GetPackage(_P1).Version);
            Assert.Equal("net472", pkg.GetPackage(_P1).Meta[PackageInfo.ATTR_TFM]);

            Assert.Single(pkg.Packages);

            Assert.True(pkg.UpdatePackage(_P1, "1.2.0", "net450"));
            Assert.Equal("1.2.0", pkg.GetPackage(_P1).Version);
            Assert.Equal("net450", pkg.GetPackage(_P1).Meta[PackageInfo.ATTR_TFM]);

            Assert.False(pkg.UpdatePackage("NotReal", "1.0.0", "net450"));
            Assert.Single(pkg.Packages);

            Assert.False(pkg.RemovePackage("NotReal"));
            Assert.Single(pkg.Packages);

            Assert.True(pkg.RemovePackage(_P1));
            Assert.Empty(pkg.Packages);
        }

        [Fact]
        public void ModifyItemsTest3()
        {
            const string _FILE = TestData.ROOT + @"PackagesConfig\test.3.tmp";
            if(File.Exists(_FILE)) File.Delete(_FILE);

            using TempPackagesConfig pkg = new(_FILE, customNew);
            Assert.Equal(pkg.File, _FILE);
            Assert.Empty(pkg.Packages);

            const string _P1 = "LX4Cnh";
            const string _P2 = "Fnv1a128";
            const string _P3 = "Huid";

            pkg.AutoCommit = true;

            Assert.True(pkg.AddPackage(_P1, "1.1.0"));
            pkg.Rollback();

            Assert.Single(pkg.Packages);
            Assert.Equal("1.1.0", pkg.GetPackage(_P1).Version);

            Assert.True(pkg.AddPackage(_P2, "2.0.0"));
            Assert.Equal(2, pkg.Packages.Count());

            pkg.AutoCommit = false;

            Assert.True(pkg.AddPackage(_P3, "1.2.3"));
            Assert.Equal(3, pkg.Packages.Count());

            // re-load

            PackagesConfig pkg2 = new(_FILE, customNew);
            Assert.Equal(pkg2.File, _FILE);
            Assert.Equal(2, pkg2.Packages.Count());

            Assert.NotNull(pkg2.GetPackage(_P2));
            Assert.NotNull(pkg.GetPackage(_P1));

            Assert.Equal("1.1.0", pkg.GetPackage(_P1).Version);
            Assert.Equal("2.0.0", pkg.GetPackage(_P2).Version);
        }

        [Fact]
        public void ModifyItemsTest4()
        {
            const string _FILE = TestData.ROOT + @"PackagesConfig\test.4.tmp";
            if(File.Exists(_FILE)) File.Delete(_FILE);

            using TempPackagesConfig pkg = new(_FILE, customNew);
            Assert.Empty(pkg.Packages);

            const string _P1 = "vsSolutionBuildEvent";
            const string _P2 = "7z.Libs";

            Assert.True(pkg.AddOrUpdateGntPackage(_P1, "1.14.1", "vsSBE"));
            Assert.Equal("1.14.1", pkg.GetPackage(_P1).Version);
            Assert.Equal("vsSBE", pkg.GetPackage(_P1).MetaOutput);

            Assert.False(pkg.AddOrUpdateGntPackage(_P1, "1.14.2"));
            Assert.Equal("1.14.2", pkg.GetPackage(_P1).Version);

            Assert.True(pkg.AddGntPackage(_P2, "19.0.2"));
            Assert.Equal("19.0.2", pkg.GetPackage(_P2).Version);
            Assert.False(pkg.GetPackage(_P2).Meta.ContainsKey(PackageInfo.ATTR_OUT));
            Assert.Null(pkg.GetPackage(_P2).MetaOutput);

            Assert.False(pkg.AddGntPackage(_P2, "16.0"));
            Assert.Equal("19.0.2", pkg.GetPackage(_P2).Version);

            Assert.True(pkg.UpdateGntPackage(_P2, "14", "7z"));
            Assert.Equal("14", pkg.GetPackage(_P2).Version);
            Assert.Equal("7z", pkg.GetPackage(_P2).Meta[PackageInfo.ATTR_OUT]);

            Assert.False(pkg.UpdateGntPackage("NotReal", "1.0"));

            Assert.Equal(2, pkg.Packages.Count());

            Assert.NotNull(pkg.GetPackage(_P2));
            Assert.NotNull(pkg.GetPackage(_P1));

            Assert.Single(pkg.GetPackage(_P2).Remove().Packages);
            Assert.Null(pkg.GetPackage(_P2));
            Assert.NotNull(pkg.GetPackage(_P1));
        }

        [Fact]
        public void ModifyItemsTest5()
        {
            const string _FILE = TestData.ROOT + @"PackagesConfig\test.5.tmp";
            if(File.Exists(_FILE)) File.Delete(_FILE);

            using TempPackagesConfig pkg = new(_FILE, customNew);
            Assert.Empty(pkg.Packages);

            PackageInfo p0 = new("NotReal", "0.1");
            PackageInfo p1 = new("LX4Cnh", "1.0");
            PackageInfo p2 = new("LX4Cnh", "1.1", new Dictionary<string, string>() { { PackageInfo.ATTR_TFM, "net472" } });
            PackageInfo p3 = new("Fnv1a128", "3.0");
            PackageInfo p4 = new("Fnv1a128", "3.4", new Dictionary<string, string>() { { PackageInfo.ATTR_TFM, "net472" } });

            Assert.True(pkg.AddPackage(p1));
            Assert.Equal("1.0", pkg.GetPackage(p1.Id).Version);
            Assert.Empty(pkg.GetPackage(p1.Id).Meta);

            Assert.Single(pkg.Packages);

            Assert.False(pkg.AddPackage(p2));
            Assert.Equal("1.0", pkg.GetPackage(p1.Id).Version);
            Assert.Null(pkg.GetPackage(p1.Id).MetaTFM);

            Assert.Single(pkg.Packages);

            Assert.False(pkg.AddOrUpdatePackage(p2));
            Assert.Equal("1.1", pkg.GetPackage(p1.Id).Version);
            Assert.Equal("net472", pkg.GetPackage(p1.Id).MetaTFM);

            Assert.Single(pkg.Packages);

            Assert.True(pkg.AddOrUpdatePackage(p3));
            Assert.Equal("3.0", pkg.GetPackage(p3.Id).Version);
            Assert.Empty(pkg.GetPackage(p3.Id).Meta);

            Assert.Equal(2, pkg.Packages.Count());

            Assert.True(pkg.UpdatePackage(p4));
            Assert.Equal("3.4", pkg.GetPackage(p3.Id).Version);
            Assert.Equal("net472", pkg.GetPackage(p3.Id).Meta[PackageInfo.ATTR_TFM]);

            Assert.Equal(2, pkg.Packages.Count());

            Assert.False(pkg.UpdatePackage(p0));
            Assert.Equal(2, pkg.Packages.Count());

            Assert.False(pkg.RemovePackage(p0));
            Assert.Equal(2, pkg.Packages.Count());

            Assert.True(pkg.RemovePackage(p4));
            Assert.Single(pkg.Packages);

            Assert.Null(pkg.GetPackage(p4.Id));
            Assert.NotNull(pkg.GetPackage(p1.Id));
            Assert.Equal("1.1", pkg.GetPackage(p1.Id).Version);
        }
    }
}
