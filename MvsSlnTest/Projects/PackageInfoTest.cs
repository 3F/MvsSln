using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class PackageInfoTest
    {
        [Fact]
        public void CtorTest1()
        {
            PackageInfo pkg = new
            (
                "Name1", 
                "1.2.3", 
                new Dictionary<string, string>() { 
                    { "attr1", "value1" }, { "attr2", "value2" } 
                }
            );

            Assert.Equal("Name1", pkg.Id);
            Assert.Equal("1.2.3", pkg.Version);
            Assert.Equal(new Version("1.2.3"), pkg.VersionParsed);

            Assert.NotNull(pkg.Meta);
            Assert.Equal("value1", pkg.Meta["attr1"]);
            Assert.Equal("value2", pkg.Meta["attr2"]);

            Assert.Throws<InvalidOperationException>(() => pkg.Remove());
        }

        [Fact]
        public void CtorTest2()
        {
            XDocument xml   = XDocument.Load(TestData.ROOT + @"PackagesConfig\packages.2.txt");
            PackageInfo pkg = new(xml.Element(PackagesConfig.ROOT).Elements().First());

            Assert.Equal("EnvDTE", pkg.Id);
            Assert.Equal("8.0.2", pkg.Version);
            Assert.Equal(new Version("8.0.2"), pkg.VersionParsed);

            Assert.NotNull(pkg.Meta);
            Assert.Equal("net45", pkg.Meta[PackageInfo.ATTR_TFM]);

            Assert.Throws<InvalidOperationException>(() => pkg.Remove());
        }
    }
}
