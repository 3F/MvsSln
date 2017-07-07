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
    }
}
