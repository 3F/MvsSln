using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class ProjectItemTest
    {
        [TestMethod]
        public void CtorTest1()
        {
            ProjectItem p = new ProjectItem("Project1", ProjectType.Cs);

            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = p.pGuid,
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "Project1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
                },
                p
            );

            SolutionFolder f = new SolutionFolder("dir1");

            p = new ProjectItem("Project1", ProjectType.Cs, f);
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = p.pGuid,
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "Project1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                    parent      = f
                },
                p
            );

            p = new ProjectItem("Project2", ProjectType.Vc, "path 1");
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = p.pGuid,
                    fullPath    = p.fullPath,
                    name        = "Project2",
                    path        = "path 1",
                    EpType      = ProjectType.Vc,
                    pType       = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}",
                },
                p
            );

            p = new ProjectItem("Project2", ProjectType.Vc, "path 1", f);
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = p.pGuid,
                    fullPath    = p.fullPath,
                    name        = "Project2",
                    path        = "path 1",
                    EpType      = ProjectType.Vc,
                    pType       = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}",
                    parent      = f
                },
                p
            );

            p = new ProjectItem("Project 3", ProjectType.Vc, "prj path", f, @"C:\path\");
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = p.pGuid,
                    fullPath    = @"C:\path\prj path",
                    name        = "Project 3",
                    path        = "prj path",
                    EpType      = ProjectType.Vc,
                    pType       = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}",
                    parent      = f
                },
                p
            );
        }

        [TestMethod]
        public void CtorTest2()
        {
            ProjectItem p = new ProjectItem("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "Project1", ProjectType.Cs);

            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{EE7DD6B7-56F4-478D-8745-3D204D915473}",
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "Project1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
                },
                p
            );

            SolutionFolder f = new SolutionFolder("dir1");

            p = new ProjectItem("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "Project1", ProjectType.Cs, f);
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{EE7DD6B7-56F4-478D-8745-3D204D915473}",
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "Project1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                    parent      = f
                },
                p
            );

            p = new ProjectItem("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "Project1", ProjectType.Cs, "path 1");
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{EE7DD6B7-56F4-478D-8745-3D204D915473}",
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "path 1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                },
                p
            );

            Assert.AreNotEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{EE7DD6B7-56F4-478D-8745-3D204D915473}",
                    fullPath    = p.fullPath,
                    name        = "Project1",
                    path        = "path 1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                    parent      = f
                },
                p
            );

            p = new ProjectItem("{47EF5301-84E5-4210-A145-6460A1C8627A}", "Project2", ProjectType.Cs, "path 1", f);
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{47EF5301-84E5-4210-A145-6460A1C8627A}",
                    fullPath    = p.fullPath,
                    name        = "Project2",
                    path        = "path 1",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                    parent      = f
                },
                p
            );

            p = new ProjectItem("{47EF5301-84E5-4210-A145-6460A1C8627A}", "Project2", ProjectType.Cs, "path 2", f, @"D:\slndir");
            Assert.AreEqual
            (
                new ProjectItem()
                {
                    pGuid       = "{47EF5301-84E5-4210-A145-6460A1C8627A}",
                    fullPath    = @"D:\slndir\path 2",
                    name        = "Project2",
                    path        = "path 2",
                    EpType      = ProjectType.Cs,
                    pType       = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                    parent      = f
                },
                p
            );
        }

        [TestMethod]
        public void CtorTest3()
        {
            var p = new ProjectItem
            (
                "{47EF5301-84E5-4210-A145-6460A1C8627A}", 
                "Project2", 
                ProjectType.Cs, 
                "path 1",
                new SolutionFolder("dir1")
            );

            Assert.AreEqual
            (
                new ProjectItem(p),
                p
            );
        }

        [TestMethod]
        public void EqTest1()
        {
            var project1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 1",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown
            };

            var project2SameAs1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 1",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown
            };

            var project3AreNotSameAs1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 2",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown
            };

            Assert.AreEqual(project1, project2SameAs1);
            Assert.AreNotEqual(project1, project3AreNotSameAs1);
        }

        [TestMethod]
        public void EqTest2()
        {
            SolutionFolder folder1 = new SolutionFolder
            (
                new ProjectItem()
                {
                    pGuid = "{C5EB299D-B7C4-41C7-992C-233D3721180D}",
                    pType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}",
                    name = "dir 1",
                    path = "dir 1",
                    EpType = ProjectType.SlnFolder
                }
            );

            SolutionFolder folder2 = new SolutionFolder
            (
                new ProjectItem()
                {
                    pGuid = "{55096B63-0420-4D9B-9DAD-77A94276D07D}",
                    pType = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}",
                    name = "dir 2",
                    path = "dir 2",
                    EpType = ProjectType.SlnFolder
                }
            );

            var project1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 1",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown,
                parent = new RefType<SolutionFolder?>(folder1)
            };

            var project2SameAs1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 1",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown,
                parent = new RefType<SolutionFolder?>(folder1)
            };

            var project3AreNotSameAs1 = new ProjectItem()
            {
                pGuid = "{8473B51C-3A9B-4A1B-8A41-D40FAC465DDD}",
                pType = "{76FB8376-8A6F-48F8-BB0F-656FD6E36F09}",
                name = "name 1",
                path = "path 1",
                fullPath = "full path 1",
                EpType = ProjectType.Unknown,
                parent = new RefType<SolutionFolder?>(folder2)
            };

            Assert.AreEqual(project1, project2SameAs1);
            Assert.AreNotEqual(project1, project3AreNotSameAs1);
        }

        [TestMethod]
        public void ParseTest1()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.AreEqual("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target.pGuid);
            Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", target.pType);
            Assert.AreEqual("Conari", target.name);
            Assert.AreEqual("Conari\\Conari.csproj", target.path);
            Assert.AreEqual($"{slnDir}Conari\\Conari.csproj", target.fullPath);
            Assert.AreEqual(ProjectType.Cs, target.EpType);
        }

        [TestMethod]
        public void ParseTest2()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.AreEqual(null, target.pGuid);
            Assert.AreEqual(null, target.pType);
            Assert.AreEqual(null, target.name);
            Assert.AreEqual(null, target.path);
            Assert.AreEqual(null, target.fullPath);
            Assert.AreEqual(ProjectType.Unknown, target.EpType);
        }

        [TestMethod]
        public void ParseTest3()
        {
            var target1 = new ProjectItem(null, "X:\\dir1\\");
            Assert.AreEqual(null, target1.pGuid);
            Assert.AreEqual(null, target1.fullPath);

            var target2 = new ProjectItem(
                "Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                null
            );

            Assert.AreEqual("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target2.pGuid);
            Assert.AreEqual("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}", target2.pType);
            Assert.AreEqual("Conari", target2.name);
            Assert.AreEqual("Conari\\Conari.csproj", target2.path);
            Assert.AreEqual(ProjectType.Vc, target2.EpType);
        }
    }
}
