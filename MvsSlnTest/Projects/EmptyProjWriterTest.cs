using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Construction;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;
using Xunit;

namespace MvsSlnTest.Projects
{
    public class EmptyProjWriterTest
    {
        [Fact]
        public void TouchTest1()
        {
            _EmptyProjWriter epw = new();

            epw.Touch(new ProjectItem("Cs", ProjectType.Cs));

            Assert.Single(epw.Data);
            AssertEqual
            (
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>",
                epw.GetContent("Cs")
            );

            epw.Touch([ new ProjectItem("Vc", ProjectType.Vc), new ProjectItem("Vb", ProjectType.Vb)] );
            Assert.Equal(3, epw.Data.Count);

            AssertEqual
            (
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <Import Project=""$(MSBuildToolsPath)\Microsoft.VisualBasic.targets"" />
</Project>",
                epw.GetContent("Vb")
            );

            epw.Touch(new ProjectItem("Fs", ProjectType.Fs));
            Assert.Equal(4, epw.Data.Count);

            AssertEqual
            (
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
</Project>",
                epw.GetContent("Fs")
            );

            AssertEqual
            (
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.props"" />
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.targets"" />
</Project>",
                epw.GetContent("Vc")
            );
        }

        [Fact]
        public void TouchTest2()
        {
            _EmptyProjWriter epw = new();

            epw.Touch
            ([
                new ProjectItem("CsSdk", ProjectType.CsSdk),
                new ProjectItem("FsSdk", ProjectType.FsSdk),
                new ProjectItem("VbSdk", ProjectType.VbSdk),
            ]);
            Assert.Equal(3, epw.Data.Count);

            string exp = @"<Project Sdk=""Microsoft.NET.Sdk""></Project>";

            Assert.Equal(exp, epw.GetContent("CsSdk"));
            Assert.Equal(exp, epw.GetContent("FsSdk"));
            Assert.Equal(exp, epw.GetContent("VbSdk"));
        }

        [Fact]
        public void TouchTest3()
        {
            _EmptyProjWriter epw = new();

            epw.Touch
            ([
                new ProjectItem("Unknown", ProjectType.Unknown),
                new ProjectItem("SlnFolder", ProjectType.SlnFolder),
            ]);
            Assert.Empty(epw.Data);

            epw.Touch(new ProjectItem("Deploy", ProjectType.Deploy));
            Assert.Empty(epw.Data);
        }

        [Fact]
        public void TouchTest4()
        {
            _EmptyProjWriter epw = new();

            Assert.Throws<NotSupportedException>(() => epw.Touch
            ([
                new ProjectItem("Unknown", ProjectType.Unknown),
                new ProjectItem("SlnFolder", ProjectType.SlnFolder),
            ], 
            strict: true));

            Assert.Throws<NotSupportedException>(() =>
                epw.Touch(new ProjectItem("Deploy", ProjectType.Deploy), strict: true)
            );
        }

        [Fact]
        public void TouchTest5()
        {
            _EmptyProjWriter epw = new();

            epw.Touch
            ([
                new ProjectItem("Unknown", ProjectType.Unknown, path: string.Empty),
                new ProjectItem("SlnFolder", ProjectType.SlnFolder, path: null),
            ]);
            Assert.Empty(epw.Data);

            epw.Touch(new ProjectItem("Deploy", ProjectType.Deploy, path: string.Empty));
            Assert.Empty(epw.Data);
        }

        [Fact]
        public void TouchTest6()
        {
            _EmptyProjWriter epw = new();

            Assert.Throws<ArgumentNullException>(() => epw.Touch
            ([
                new ProjectItem("Unknown", ProjectType.Unknown, path: string.Empty),
                new ProjectItem("SlnFolder", ProjectType.SlnFolder, path: null),
            ],
            strict: true));

            Assert.Throws<ArgumentNullException>(() =>
                epw.Touch(new ProjectItem("Deploy", ProjectType.Deploy, path: null), strict: true)
            );
        }

        [Fact]
        public void TouchTest7()
        {
            _EmptyProjWriter epw = new();

            epw.Touch
            ([
                new ProjectItem("Unknown", ProjectType.Unknown, path: null),
                new ProjectItem("CsSdk", ProjectType.CsSdk),
                new ProjectItem("SlnFolder", ProjectType.SlnFolder, path: string.Empty),
            ]);

            Assert.Single(epw.Data);
            Assert.Equal(@"<Project Sdk=""Microsoft.NET.Sdk""></Project>", epw.GetContent("CsSdk"));
        }

        [Fact]
        public void SlnWriteTest1()
        {
            _EmptyProjWriter epw = new();

            string projName = "test";
            ProjectItem[] projects = [new ProjectItem("{141194F4-B4E6-4FAA-8DA8-F902C435D9D1}", projName, ProjectType.CsSdk, @$"{projName}\src.csproj")];

            LhDataHelper hdata = new();
            hdata.SetHeader(SlnHeader.MakeDefault()).SetProjects(projects);

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 17
VisualStudioVersion = 17.8.34525.116
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""test"", ""test\src.csproj"", ""{141194F4-B4E6-4FAA-8DA8-F902C435D9D1}""
EndProject
Global
EndGlobal
";
            using SlnWriter w = new(hdata);
            w.Options = SlnWriterOptions.CreateProjectsIfNotExist;
            w.SetToucher(epw);

            AssertEqual(expected, w.WriteAsString());

            Assert.Single(epw.Data);
            Assert.Equal(@"<Project Sdk=""Microsoft.NET.Sdk""></Project>", epw.GetContent(@"test\src.csproj"));
        }

        [Fact]
        public void SlnWriteTest2()
        {
            _EmptyProjWriter epw = new();

            string projName = "test";
            ProjectItem[] projects = [new ProjectItem(ProjectType.Cs, @$"{projName}\src.csproj", "{71E9877D-A13C-45F4-BE14-D8040EDD6B18}")];

            LhDataHelper hdata = new();
            hdata.SetHeader(new SlnHeader("11.0")).SetProjects(projects);

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 11.00
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""test"", ""test\src.csproj"", ""{71E9877D-A13C-45F4-BE14-D8040EDD6B18}""
EndProject
Global
EndGlobal
";
            using SlnWriter w = new(hdata);
            w.Options = SlnWriterOptions.CreateProjectsIfNotExist;
            w.SetToucher(epw);

            AssertEqual(expected, w.WriteAsString());

            Assert.Single(epw.Data);
            AssertEqual
            (
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>",
                epw.GetContent(@"test\src.csproj")
            );
        }

        //FIXME: AssertEqual() wrapper because of strange Assert.Equal() behavior on Ubuntu 20.04
        //      e.g.: expected and actual values are swapped between and no matter \ or / is used to equalize values between `expected` and `actual`:
        //
        //   Error Message:
        //        Assert.Equal() Failure: Strings differ
        //                                  ↓ (pos 194)
        // Expected: ···"BuildExtensionsPath)\\$(MSBuildToolsVersio"···
        // Actual:   ···"BuildExtensionsPath)/$(MSBuildToolsVersio"···     <<<<<<<
        //
        // But! when you're trying even to replace `\` as `/` in Expected value:
        //
        //   Error Message:
        //        Assert.Equal() Failure: Strings differ
        //                                  ↓ (pos 194)
        // Expected: ···"BuildExtensionsPath)/$(MSBuildToolsVersio"···
        // Actual:   ···"BuildExtensionsPath)\\$(MSBuildToolsVersio"···    <<<<<<<<<<<<<
        //
        // In general, this should not affect either one or the other.
        // That is, the original logic must be OK. only tests. TODO
        private static void AssertEqual(string expected, string actual)
            => Assert.Equal(expected.AdaptPath(), actual.AdaptPath());

        private sealed class _EmptyProjWriter: EmptyProjWriter
        {
            public Dictionary<string, string> Data { get; } = [];

            public string GetContent(string key)
                => Data[Path.Combine(Directory.GetCurrentDirectory(), key.AdaptPath())];

            protected override void Save(ProjectRootElement data)
            {
                string xml = data.RawXml;

                //NOTE: netfx-based ProjectRootElement will generate encoding="utf-16" instead of "utf-8" in header at RawXml result even when Encoding.UTF8 is used;
                //      wile original base .Save(Encoding.UTF8) seems to be working correctly o_o
#if !(NET || NETCOREAPP || NETSTANDARD)
                xml = xml.Replace("utf-16", encoding.BodyName);
#endif
                Data[data.FullPath] = xml;
            }

            protected override void Save(string data, string destination)
                => Data[destination] = data;
        }
    }
}
