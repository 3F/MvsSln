using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSlnTest.Extensions
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void GuidTest1()
        {
            string data = " MvsSln_-_v1 ";

            Assert.AreEqual(new Guid("{ee265a58-1e72-6c44-60aa-134eaf5c6f9c}"), data.Guid());
            Assert.AreEqual(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), "".Guid());
            Assert.AreEqual(new Guid("{ef8db523-b411-2757-d335-1702515f86af}"), "  ".Guid());
            Assert.AreEqual(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), ((string)null).Guid());
        }

        [TestMethod]
        public void GuidSlnFormatTest1()
        {
            Assert.AreEqual
            (
                "{D98C1DD4-008F-04B2-E980-0998ECF8427E}", 
                new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}").SlnFormat()
            );
        }

        [TestMethod]
        public void ReformatSlnGuidTest1()
        {
            Assert.AreEqual("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "".ReformatSlnGuid());
            Assert.AreEqual("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", " ".ReformatSlnGuid());
            Assert.AreEqual(null, ((string)null).ReformatSlnGuid());
            Assert.AreEqual("{842DDBFE-FECA-8620-2CB4-399751A8A7E3}", "invalid".ReformatSlnGuid());
            Assert.AreEqual("{DCE5BB88-7640-4CFB-861D-6CBAA1F6EF0E}", "dce5bb88-7640-4cfb-861d-6cbaa1f6ef0e".ReformatSlnGuid());
            Assert.AreEqual("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "{d98c1dd4-008f-04b2-e980-0998ecf8427e}".ReformatSlnGuid());
        }

        [TestMethod]
        public void DirectoryPathFormatTest1()
        {
            string dir1 = @"D:\path\to\dir1";
            Assert.AreEqual($"{dir1}{Path.DirectorySeparatorChar}", dir1.DirectoryPathFormat());

            string dir2 = @"D:\path\to\dir2\";
            Assert.AreEqual(dir2, dir2.DirectoryPathFormat());

            string dir3 = null;
            Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), dir3.DirectoryPathFormat());
            Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), "".DirectoryPathFormat());
        }

        [TestMethod]
        public void IsDirectoryPathTest1()
        {
            string dir1 = @"D:\path\to\dir1";
            Assert.AreEqual(false, dir1.IsDirectoryPath());

            string dir2 = @"D:\path\to\dir2\";
            Assert.AreEqual(true, dir2.IsDirectoryPath());

            string dir3 = null;
            Assert.AreEqual(false, dir3.IsDirectoryPath());
            Assert.AreEqual(false, "".IsDirectoryPath());
        }

        [TestMethod]
        public void MakeRelativePathTest1()
        {
            string dir0 = @"D:\path\to\dir0";

            string dir1 = null;
            Assert.AreEqual(null, dir1.MakeRelativePath(dir0));
            Assert.AreEqual(null, dir1.MakeRelativePath(String.Empty));
            Assert.AreEqual(null, dir1.MakeRelativePath(null));

            Assert.AreEqual(null, "".MakeRelativePath(dir0));
            Assert.AreEqual(null, "".MakeRelativePath(String.Empty));
            Assert.AreEqual(null, "".MakeRelativePath(null));

            Assert.AreEqual(null, dir0.MakeRelativePath(String.Empty));
            Assert.AreEqual(null, dir0.MakeRelativePath(null));
        }

        [TestMethod]
        public void MakeRelativePathTest2()
        {
            Assert.AreEqual(@"bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\dir1\bin\Release\file"));

            Assert.AreEqual(@"..\bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\bin\Release\file"));
            Assert.AreEqual(@"..\dir2\bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\dir2\bin\Release\file"));

            Assert.AreEqual(@"bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"bin\Release\file"));

            Assert.AreEqual(null, @"path\to\dir1".MakeRelativePath(@"D:\path\to\dir1\bin\Release\file"));
        }

        [TestMethod]
        public void FirstNonWhiteSpaceTest1()
        {
            Assert.AreEqual(10, "  ActiveCfg  = ".FirstNonWhiteSpace(12, true));
            Assert.AreEqual(10, "  ActiveCfg = ".FirstNonWhiteSpace(11, true));

            Assert.AreEqual(13, "  ActiveCfg  = ".FirstNonWhiteSpace(12, false));
            Assert.AreEqual(14, "  ActiveCfg   = ".FirstNonWhiteSpace(12, false));

            Assert.AreEqual(2, "  A ctiveCfg".FirstNonWhiteSpace(0, false));
            Assert.AreEqual(4, "  A ctiveCfg".FirstNonWhiteSpace(4, true));
            Assert.AreEqual(2, "  A ctiveCfg".FirstNonWhiteSpace(3, true));
            Assert.AreEqual(0, "A ctiveCfg".FirstNonWhiteSpace(0, false));
        }

        [TestMethod]
        public void FirstNonWhiteSpaceTest2()
        {
            Assert.AreEqual(-1, "  ActiveCfg = ".FirstNonWhiteSpace(0, true));
            Assert.AreEqual(0, "ActiveCfg = ".FirstNonWhiteSpace(0, true));

            Assert.AreEqual(-1, "  ActiveCfg = ".FirstNonWhiteSpace(1, true));
            Assert.AreEqual(1, "ActiveCfg = ".FirstNonWhiteSpace(1, true));

            Assert.AreEqual(-1, "  ActiveCfg = ".FirstNonWhiteSpace(13, false));
            Assert.AreEqual(-1, "  ActiveCfg =".FirstNonWhiteSpace(13, false));

            Assert.AreEqual(12, "  ActiveCfg = ".FirstNonWhiteSpace(12, false));
            Assert.AreEqual(12, "  ActiveCfg =".FirstNonWhiteSpace(12, false));
        }

        [TestMethod]
        public void FirstNonWhiteSpaceTest3()
        {
            Assert.AreEqual(-1, "ActiveCfg".FirstNonWhiteSpace(50, false));
            Assert.AreEqual(-1, "ActiveCfg".FirstNonWhiteSpace(50, true));

            Assert.AreEqual(-1, " ActiveCfg ".FirstNonWhiteSpace(50, false));
            Assert.AreEqual(-1, " ActiveCfg ".FirstNonWhiteSpace(50, true));

            Assert.AreEqual(-1, "ActiveCfg".FirstNonWhiteSpace(-50, false));
            Assert.AreEqual(-1, "ActiveCfg".FirstNonWhiteSpace(-50, true));

            Assert.AreEqual(-1, " ActiveCfg ".FirstNonWhiteSpace(-50, false));
            Assert.AreEqual(-1, " ActiveCfg ".FirstNonWhiteSpace(-50, true));
        }

        [TestMethod]
        public void FirstNonWhiteSpaceTest4()
        {
            Assert.AreEqual(-1, "  ".FirstNonWhiteSpace(0, false));
            Assert.AreEqual(-1, " ".FirstNonWhiteSpace(0, false));
            Assert.AreEqual(-1, String.Empty.FirstNonWhiteSpace(0, false));
            Assert.AreEqual(-1, ((string)null).FirstNonWhiteSpace(0, false));

            Assert.AreEqual(-1, "  ".FirstNonWhiteSpace(0, true));
            Assert.AreEqual(-1, " ".FirstNonWhiteSpace(0, true));
            Assert.AreEqual(-1, String.Empty.FirstNonWhiteSpace(0, true));
            Assert.AreEqual(-1, ((string)null).FirstNonWhiteSpace(0, true));

            Assert.AreEqual(-1, "  ".FirstNonWhiteSpace(1, true));
            Assert.AreEqual(-1, " ".FirstNonWhiteSpace(1, true));
            Assert.AreEqual(-1, String.Empty.FirstNonWhiteSpace(1, true));
            Assert.AreEqual(-1, ((string)null).FirstNonWhiteSpace(1, true));
        }
    }
}
