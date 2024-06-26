﻿using System;
using System.IO;
using net.r_eg.MvsSln.Extensions;
using Xunit;

namespace MvsSlnTest.Extensions
{
    using _svc.Static;

    public class StringExtensionTest
    {
        [Fact]
        public void GuidTest1()
        {
            string data = " MvsSln_-_v1 ";

#if FEATURE_HUID

            Assert.Equal(new Guid("{cbdd6048-feda-8dac-9dc4-c6c857c4abb7}"), data.Guid());
            Assert.Equal(new Guid("{e0b7c8ae-3333-8623-aeb8-7decf99dd400}"), "".Guid());
            Assert.Equal(new Guid("{7635fb3f-6ae4-8d6e-becf-2165e03ae3e9}"), "  ".Guid());
            Assert.Equal(new Guid("{e0b7c8ae-3333-8623-aeb8-7decf99dd400}"), ((string)null).Guid());

#elif FEATURE_GUID_SHA1

            Assert.Equal(new Guid("{8f78ea2d-50d2-5371-b55d-c09993af4ba8}"), data.Guid());
            Assert.Equal(new Guid("{67edabcd-520c-5836-b363-f5b468c6f198}"), "".Guid());
            Assert.Equal(new Guid("{e1fcb595-ffed-5081-92f1-64fee31bbaa5}"), "  ".Guid());
            Assert.Equal(new Guid("{67edabcd-520c-5836-b363-f5b468c6f198}"), ((string)null).Guid());
#else
            Assert.Equal(new Guid("{ee265a58-1e72-6c44-60aa-134eaf5c6f9c}"), data.Guid());
            Assert.Equal(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), "".Guid());
            Assert.Equal(new Guid("{ef8db523-b411-2757-d335-1702515f86af}"), "  ".Guid());
            Assert.Equal(new Guid("{d98c1dd4-008f-04b2-e980-0998ecf8427e}"), ((string)null).Guid());
#endif
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
#if FEATURE_HUID

            Assert.Equal("{E0B7C8AE-3333-8623-AEB8-7DECF99DD400}", "".ReformatSlnGuid());
            Assert.Equal("{E0B7C8AE-3333-8623-AEB8-7DECF99DD400}", " ".ReformatSlnGuid());
            Assert.Equal("{9939088C-7F2F-8FEE-B5F3-7DEF927816C5}", "invalid".ReformatSlnGuid());

#elif FEATURE_GUID_SHA1

            Assert.Equal("{67EDABCD-520C-5836-B363-F5B468C6F198}", "".ReformatSlnGuid());
            Assert.Equal("{67EDABCD-520C-5836-B363-F5B468C6F198}", " ".ReformatSlnGuid());
            Assert.Equal("{EB71634D-4275-5E2E-9AA2-15419F819ACC}", "invalid".ReformatSlnGuid());
#else
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "".ReformatSlnGuid());
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", " ".ReformatSlnGuid());
            Assert.Equal("{842DDBFE-FECA-8620-2CB4-399751A8A7E3}", "invalid".ReformatSlnGuid());
#endif
            Assert.Null(((string)null).ReformatSlnGuid());

            Assert.Equal("{DCE5BB88-7640-4CFB-861D-6CBAA1F6EF0E}", "dce5bb88-7640-4cfb-861d-6cbaa1f6ef0e".ReformatSlnGuid());
            Assert.Equal("{D98C1DD4-008F-04B2-E980-0998ECF8427E}", "{d98c1dd4-008f-04b2-e980-0998ecf8427e}".ReformatSlnGuid());
        }

        [Fact]
        public void DirectoryPathFormatTest1()
        {
            string dir1 = @"D:\path\to\dir1".AdaptWinPath();
            Assert.Equal($"{dir1}{Path.DirectorySeparatorChar}", dir1.DirectoryPathFormat());

            string dir2 = @"D:\path\to\dir2\".AdaptWinPath();
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
        public void IsDirectoryPathTest2()
        {
            Assert.True("path/to/dir2/".IsDirectoryPath());
            Assert.True("/".IsDirectoryPath());
            Assert.True(@"\".IsDirectoryPath());
            Assert.True(@"..\".IsDirectoryPath());
            Assert.True(@".\".IsDirectoryPath());
        }

        [Fact]
        public void MakeRelativePathTest1()
        {
            string dir0 = @"D:\path\to\dir0".AdaptWinPath();

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
            string dir1 = @"D:\path\to\dir1".AdaptWinPath();

            Assert.Equal(@"bin\Release\file".AdaptPath(), dir1.MakeRelativePath(@"D:\path\to\dir1\bin\Release\file".AdaptWinPath()));

            Assert.Equal(@"..\bin\Release\file".AdaptPath(), dir1.MakeRelativePath(@"D:\path\to\bin\Release\file".AdaptWinPath()));
            Assert.Equal(@"..\dir2\bin\Release\file".AdaptPath(), dir1.MakeRelativePath(@"D:\path\to\dir2\bin\Release\file".AdaptWinPath()));

            Assert.Equal(@"bin\Release\file".AdaptPath(), dir1.MakeRelativePath(@"bin\Release\file"));

            Assert.Null(@"path\to\dir1".AdaptPath().MakeRelativePath(@"D:\path\to\dir1\bin\Release\file".AdaptWinPath()));
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

#if !NET40
        [Theory]
        [InlineData(@"aaa\bbbb\ TestProject 1\src.csproj", " TestProject 1")]
        [InlineData(@" TestProject 1\src.csproj", " TestProject 1")]
        [InlineData(@"\ src.csproj", " src")]
        [InlineData(@"\src.csproj", "src")]
        [InlineData(@"\\src.csproj", "src")]
        [InlineData(@"\\ src.csproj", " src")]
        [InlineData(@"src.csproj", "src")]
        [InlineData(@" src.csproj", " src")]
        [InlineData(@"/src.csproj", "src")]
        [InlineData(@"/ src.csproj", " src")]
        [InlineData(@"dir1/src.csproj", "dir1")]
        [InlineData(@"/dir1/src.csproj", "dir1")]
        [InlineData(@"\dir1\\src.csproj", "dir1")]
        [InlineData(@"\\dir1\\src.csproj", "dir1")]
        [InlineData(@"\ src", " src")]
        [InlineData(@"\src", "src")]
        [InlineData(@"\\src", "src")]
        [InlineData(@"\\ src", " src")]
        [InlineData(@"src", "src")]
        [InlineData(@" src", " src")]
        [InlineData(@"/src", "src")]
        [InlineData(@"/ src", " src")]
        [InlineData(@"aaa\bbbb\ TestProject 1\\src.csproj", " TestProject 1")]
        [InlineData(@"aaa\bbbb\ TestProject 1\\src", " TestProject 1")]
        [InlineData(@"", "")]
        [InlineData(@" ", " ")]
        [InlineData(null, null)]
        public void GetDirNameOrFileNameTest1(string input, string name)
        {
            Assert.Equal(name, input.GetDirNameOrFileName());
            Assert.Equal(name, input.GetDirNameOrFileName(trim: false));
            if(name != null)
            {
                Assert.Equal(name.Trim(), input.GetDirNameOrFileName(trim: true));
            }
        }
#else
        [Fact]
        public void GetDirNameOrFileNameTest1()
        {
            Assert.Equal("TestProject 1", @"aaa\bbbb\TestProject 1\src.csproj".GetDirNameOrFileName());
            Assert.Equal("TestProject 1", @"TestProject 1\src.csproj".GetDirNameOrFileName());
            Assert.Equal("src", @"\src.csproj".GetDirNameOrFileName());
            Assert.Equal("src", @"\\src.csproj".GetDirNameOrFileName());
            Assert.Equal("src", @"\src".GetDirNameOrFileName());
            Assert.Equal("src", @"\\src".GetDirNameOrFileName());
            Assert.Equal("src", @"src.csproj".GetDirNameOrFileName());
            Assert.Equal("src", @"/src.csproj".GetDirNameOrFileName());
            Assert.Equal("src", @"src".GetDirNameOrFileName());
            Assert.Equal("src", @"/src".GetDirNameOrFileName());
            Assert.Equal("dir1", @"dir1/src.csproj".GetDirNameOrFileName());
            Assert.Equal("dir1", @"/dir1/src.csproj".GetDirNameOrFileName());
            Assert.Equal("dir1", @"\dir1\\src.csproj".GetDirNameOrFileName());
            Assert.Equal("dir1", @"\dir1\\src".GetDirNameOrFileName());
            Assert.Equal("dir1", @"\\dir1\\src.csproj".GetDirNameOrFileName());
            Assert.Equal("TestProject 1", @"aaa\bbbb\TestProject 1\\src.csproj".GetDirNameOrFileName());
        }
#endif
    }
}
