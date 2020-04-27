using System;
using System.IO;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    public class StringExtensionTest
    {
        [Fact]
        public void GuidTest1()
        {
            string data = " MvsSln_-_v1 ";

            Assert.Equal(new Guid("{ee265a58-1e72-6c44-60aa-134eaf5c6f9c}"), data.Guid());
            Assert.Equal(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), "".Guid());
            Assert.Equal(new Guid("{ef8db523-b411-2757-d335-1702515f86af}"), "  ".Guid());
            Assert.Equal(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), ((string)null).Guid());
        }

        [Fact]
        public void GuidSlnFormatTest1()
        {
            Assert.Equal
            (
                "{D98C1DD4-008F-04B2-E980-0998ECF8427E}", 
                new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}").SlnFormat()
            );
        }

        [Fact]
        public void ReformatSlnGuidTest1()
        {
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "".ReformatSlnGuid());
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", " ".ReformatSlnGuid());
            Assert.Null(((string)null).ReformatSlnGuid());
            Assert.Equal("{842DDBFE-FECA-8620-2CB4-399751A8A7E3}", "invalid".ReformatSlnGuid());
            Assert.Equal("{DCE5BB88-7640-4CFB-861D-6CBAA1F6EF0E}", "dce5bb88-7640-4cfb-861d-6cbaa1f6ef0e".ReformatSlnGuid());
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "{d98c1dd4-008f-04b2-e980-0998ecf8427e}".ReformatSlnGuid());
        }

        [Fact]
        public void DirectoryPathFormatTest1()
        {
            string dir1 = @"D:\path\to\dir1";
            Assert.Equal($"{dir1}{Path.DirectorySeparatorChar}", dir1.DirectoryPathFormat());

            string dir2 = @"D:\path\to\dir2\";
            Assert.Equal(dir2, dir2.DirectoryPathFormat());

            string dir3 = null;
            Assert.Equal(Path.DirectorySeparatorChar.ToString(), dir3.DirectoryPathFormat());
            Assert.Equal(Path.DirectorySeparatorChar.ToString(), "".DirectoryPathFormat());
        }

        [Fact]
        public void IsDirectoryPathTest1()
        {
            string dir1 = @"D:\path\to\dir1";
            Assert.False(dir1.IsDirectoryPath());

            string dir2 = @"D:\path\to\dir2\";
            Assert.True(dir2.IsDirectoryPath());

            string dir3 = null;
            Assert.False(dir3.IsDirectoryPath());
            Assert.False("".IsDirectoryPath());
        }

        [Fact]
        public void MakeRelativePathTest1()
        {
            string dir0 = @"D:\path\to\dir0";

            string dir1 = null;
            Assert.Null(dir1.MakeRelativePath(dir0));
            Assert.Null(dir1.MakeRelativePath(String.Empty));
            Assert.Null(dir1.MakeRelativePath(null));

            Assert.Null("".MakeRelativePath(dir0));
            Assert.Null("".MakeRelativePath(String.Empty));
            Assert.Null("".MakeRelativePath(null));

            Assert.Null(dir0.MakeRelativePath(String.Empty));
            Assert.Null(dir0.MakeRelativePath(null));
        }

        [Fact]
        public void MakeRelativePathTest2()
        {
            Assert.Equal(@"bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\dir1\bin\Release\file"));

            Assert.Equal(@"..\bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\bin\Release\file"));
            Assert.Equal(@"..\dir2\bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"D:\path\to\dir2\bin\Release\file"));

            Assert.Equal(@"bin\Release\file", @"D:\path\to\dir1".MakeRelativePath(@"bin\Release\file"));

            Assert.Null(@"path\to\dir1".MakeRelativePath(@"D:\path\to\dir1\bin\Release\file"));
        }

        [Fact]
        public void FirstNonWhiteSpaceTest1()
        {
            Assert.Equal(10, "  ActiveCfg  = ".FirstNonWhiteSpace(12, true));
            Assert.Equal(10, "  ActiveCfg = ".FirstNonWhiteSpace(11, true));

            Assert.Equal(13, "  ActiveCfg  = ".FirstNonWhiteSpace(12, false));
            Assert.Equal(14, "  ActiveCfg   = ".FirstNonWhiteSpace(12, false));

            Assert.Equal(2, "  A ctiveCfg".FirstNonWhiteSpace(0, false));
            Assert.Equal(4, "  A ctiveCfg".FirstNonWhiteSpace(4, true));
            Assert.Equal(2, "  A ctiveCfg".FirstNonWhiteSpace(3, true));
            Assert.Equal(0, "A ctiveCfg".FirstNonWhiteSpace(0, false));
        }

        [Fact]
        public void FirstNonWhiteSpaceTest2()
        {
            Assert.Equal(-1, "  ActiveCfg = ".FirstNonWhiteSpace(0, true));
            Assert.Equal(0, "ActiveCfg = ".FirstNonWhiteSpace(0, true));

            Assert.Equal(-1, "  ActiveCfg = ".FirstNonWhiteSpace(1, true));
            Assert.Equal(1, "ActiveCfg = ".FirstNonWhiteSpace(1, true));

            Assert.Equal(-1, "  ActiveCfg = ".FirstNonWhiteSpace(13, false));
            Assert.Equal(-1, "  ActiveCfg =".FirstNonWhiteSpace(13, false));

            Assert.Equal(12, "  ActiveCfg = ".FirstNonWhiteSpace(12, false));
            Assert.Equal(12, "  ActiveCfg =".FirstNonWhiteSpace(12, false));
        }

        [Fact]
        public void FirstNonWhiteSpaceTest3()
        {
            Assert.Equal(-1, "ActiveCfg".FirstNonWhiteSpace(50, false));
            Assert.Equal(-1, "ActiveCfg".FirstNonWhiteSpace(50, true));

            Assert.Equal(-1, " ActiveCfg ".FirstNonWhiteSpace(50, false));
            Assert.Equal(-1, " ActiveCfg ".FirstNonWhiteSpace(50, true));

            Assert.Equal(-1, "ActiveCfg".FirstNonWhiteSpace(-50, false));
            Assert.Equal(-1, "ActiveCfg".FirstNonWhiteSpace(-50, true));

            Assert.Equal(-1, " ActiveCfg ".FirstNonWhiteSpace(-50, false));
            Assert.Equal(-1, " ActiveCfg ".FirstNonWhiteSpace(-50, true));
        }

        [Fact]
        public void FirstNonWhiteSpaceTest4()
        {
            Assert.Equal(-1, "  ".FirstNonWhiteSpace(0, false));
            Assert.Equal(-1, " ".FirstNonWhiteSpace(0, false));
            Assert.Equal(-1, String.Empty.FirstNonWhiteSpace(0, false));
            Assert.Equal(-1, ((string)null).FirstNonWhiteSpace(0, false));

            Assert.Equal(-1, "  ".FirstNonWhiteSpace(0, true));
            Assert.Equal(-1, " ".FirstNonWhiteSpace(0, true));
            Assert.Equal(-1, String.Empty.FirstNonWhiteSpace(0, true));
            Assert.Equal(-1, ((string)null).FirstNonWhiteSpace(0, true));

            Assert.Equal(-1, "  ".FirstNonWhiteSpace(1, true));
            Assert.Equal(-1, " ".FirstNonWhiteSpace(1, true));
            Assert.Equal(-1, String.Empty.FirstNonWhiteSpace(1, true));
            Assert.Equal(-1, ((string)null).FirstNonWhiteSpace(1, true));
        }

        [Fact]
        public void NullIfEmptyTest4()
        {
            Assert.Null(string.Empty.NullIfEmpty());
            Assert.NotNull(" ".NullIfEmpty());
            Assert.Null(((string)null).NullIfEmpty());
        }
    }
}
