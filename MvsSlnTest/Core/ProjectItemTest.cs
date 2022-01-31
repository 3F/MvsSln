using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class ProjectItemTest
    {
        [Fact]
        public void CtorTest1()
        {
            ProjectItem p = new ProjectItem("Project1", ProjectType.Cs);

            Assert.Equal
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
            Assert.Equal
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
            Assert.Equal
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
            Assert.Equal
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
            Assert.Equal
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

        [Fact]
        public void CtorTest2()
        {
            ProjectItem p = new ProjectItem("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "Project1", ProjectType.Cs);

            Assert.Equal
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
            Assert.Equal
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
            Assert.Equal
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

            Assert.NotEqual
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
            Assert.Equal
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
            Assert.Equal
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

        [Fact]
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

            Assert.Equal
            (
                new ProjectItem(p),
                p
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void CtorTest4(string name)
        {
            ProjectItem p = new(name, ProjectType.Cs);

            Assert.Null(p.fullPath);
            Assert.Equal(p.path, p.name);
        }

        [Fact]
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

            Assert.Equal(project1, project2SameAs1);
            Assert.NotEqual(project1, project3AreNotSameAs1);
        }

        [Fact]
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

            Assert.Equal(project1, project2SameAs1);
            Assert.NotEqual(project1, project3AreNotSameAs1);
        }

        [Fact]
        public void ParseTest1()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.Equal("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target.pGuid);
            Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", target.pType);
            Assert.Equal("Conari", target.name);
            Assert.Equal("Conari\\Conari.csproj", target.path);
            Assert.Equal($"{slnDir}Conari\\Conari.csproj", target.fullPath);
            Assert.Equal(ProjectType.Cs, target.EpType);
        }

        [Fact]
        public void ParseTest2()
        {
            var slnDir = "X:\\dir1\\";
            var target = new ProjectItem(
                "\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                slnDir
            );

            Assert.Null(target.pGuid);
            Assert.Null(target.pType);
            Assert.Null(target.name);
            Assert.Null(target.path);
            Assert.Null(target.fullPath);
            Assert.Equal(ProjectType.Unknown, target.EpType);
        }

        [Fact]
        public void ParseTest3()
        {
            var target1 = new ProjectItem(null, "X:\\dir1\\");
            Assert.Null(target1.pGuid);
            Assert.Null(target1.fullPath);

            var target2 = new ProjectItem(
                "Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"Conari\", \"Conari\\Conari.csproj\", \"{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}\"",
                null
            );

            Assert.Equal("{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}", target2.pGuid);
            Assert.Equal("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}", target2.pType);
            Assert.Equal("Conari", target2.name);
            Assert.Equal("Conari\\Conari.csproj", target2.path);
            Assert.Equal(ProjectType.Vc, target2.EpType);
        }
    }
}
