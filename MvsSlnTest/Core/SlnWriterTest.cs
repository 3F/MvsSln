﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MvsSlnTest._svc;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Exceptions;
using Xunit;

namespace MvsSlnTest.Core
{
    using static net.r_eg.MvsSln.Static.Members;

    public class SlnWriterTest
    {
        private readonly SlnItems defaultSlnItems = SlnItems.Projects
                                            | SlnItems.SolutionConfPlatforms
                                            | SlnItems.ProjectConfPlatforms
                                            | SlnItems.ProjectDependencies
                                            | SlnItems.Header
                                            | SlnItems.Map;

        [Fact]
        public void ValidationTest1()
        {
            using(var sln = new Sln(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent))
            {
                var handlers = new Dictionary<Type, HandlerValue>() {
                    [typeof(LProject)] = new HandlerValue(),
                    [typeof(LProjectDependencies)] = new HandlerValue(),
                    [typeof(LProjectConfigurationPlatforms)] = new HandlerValue(),
                };

                using(var stream = new StreamWriter(new MemoryStream()))
                {
                    var writer = new SlnWriter(stream, handlers);
                    Assert.Throws<CoHandlerRuleException>(() => writer.Write(sln.Result.Map));
                }
            }
        }

        [Fact]
        public void ValidationTest2()
        {
            using(var sln = new Sln(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent))
            {
                var handlers = new Dictionary<Type, HandlerValue>() {
                    [typeof(LProjectDependencies)] = new HandlerValue(),
                };

                using(var stream = new StreamWriter(new MemoryStream()))
                {
                    var writer = new SlnWriter(stream, handlers);
                    Assert.Throws<CoHandlerRuleException>(() => writer.Write(sln.Result.Map));
                }
            }
        }

        [Fact]
        public void MapRefTest1()
        {
            using Sln sln = new(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent);
            int expectedCount = sln.Result.Map.Count;
            var expectedMap = sln.Result.Map.Select(s => s.Clone()).ToArray();

            Dictionary<Type, HandlerValue> handlers = new()
            {
                [typeof(LProject)] = new(),
                [typeof(LProjectConfigurationPlatforms)] = new(),
                [typeof(LSolutionConfigurationPlatforms)] = new(),
            };

            using SlnWriter sw = new(new StreamWriter(new MemoryStream()), handlers);
            sw.Write(sln.Result.Map);

            Assert.Equal(expectedCount, sln.Result.Map.Count);

            for(int i = 0; i < expectedCount; ++i)
            {
                Assert.Equal
                (
                    (expectedMap[i].Handler as IObjHandler)?.Id,
                    (sln.Result.Map[i].Handler as IObjHandler)?.Id
                );
                Assert.Equal(expectedMap[i].Line, sln.Result.Map[i].Line);
                Assert.Equal(expectedMap[i].Raw, sln.Result.Map[i].Raw);
            }
        }

#if !NET40
        [Theory]
        [InlineData(SlnItems.AllNoLoad)]
        [InlineData(SlnItems.AllNoLoad & ~SlnItems.Header)]
        [InlineData(SlnItems.AllNoLoad & ~SlnItems.SolutionItems)]
        [InlineData(SlnItems.AllNoLoad & ~(SlnItems.SolutionItems | SlnItems.ExtItems))]
        public void L102Test1(SlnItems conf)
        {
            using Sln sln = new(TestData.GetPathTo(@"SlnWriter\L-102\src.sln"), conf);

            RwChecker.Check(sln, []);
            RwChecker.Check(sln, new()
            {
                [typeof(LProject)] = new(new WProject(sln.Result.ProjectItems, sln.Result.ProjectDependencies)),
            });
        }
#endif

        [Fact]
#if !NET40
        public async Task WriteTest1Async()
#else
        public void WriteTest1()
#endif
        {
            using Sln sln = new(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent);

            Dictionary<Type, HandlerValue> handlers = new()
            {
                [typeof(LVisualStudioVersion)] = new(new WVisualStudioVersion
                (
                    SlnHeader.MakeDefault())
                ),
                [typeof(LProjectConfigurationPlatforms)] = new(new WProjectConfigurationPlatforms
                (
                    sln.Result.ProjectConfigs.Where(c => c.Sln.Configuration == "CI_Debug_net45")
                )),
                [typeof(LProject)] = new(new WProject
                (
                    sln.Result.ProjectItems.Where(p => p.name == "Devenv"),
                    sln.Result.ProjectDependencies)
                ),
                [typeof(LSolutionConfigurationPlatforms)] = new(),
            };

            using SlnWriter w = new(handlers);

#if !NET40
            string data = await w.WriteAsStringAsync(sln.Result.Map);
#else
            Task<string> dataAsync = w.WriteAsStringAsync(sln.Result.Map);
            dataAsync.Wait();
            string data = dataAsync.Result;
#endif

            Assert.Equal(data, w.WriteAsString(sln.Result.Map));


            StreamWriter sw = new(new MemoryStream());
            using SlnWriter wm = new(sw, handlers);

#if !NET40
            await wm.WriteAsync(sln.Result.Map);
#else
            wm.WriteAsync(sln.Result.Map).Wait();
#endif

            sw.Flush();
            sw.BaseStream.Position = 0;

            using StreamReader sr = new(sw.BaseStream);

#if !NET40
            Assert.Equal(data, await sr.ReadToEndAsync());
#else
            Assert.Equal(data, sr.ReadToEnd());
#endif

            w.Write(sln.Result.Map);

#if !NET40
            await w.WriteAsync(sln.Result.Map);
            Assert.StartsWith(data, await w.WriteAsStringAsync(sln.Result.Map));

#else
            Task<bool> startWithAsync = w.WriteAsync(sln.Result.Map)
                .ContinueWith(t => w.WriteAsStringAsync(sln.Result.Map))
                .ContinueWith(t => t.Result.Result.StartsWith(data));

            startWithAsync.Wait();
            Assert.True(startWithAsync.Result);
#endif

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 17
VisualStudioVersion = 17.8.34525.116
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Devenv"", ""Devenv\Devenv.csproj"", ""{56437CBB-4AE5-4405-B928-600009D60E2D}""
	ProjectSection(ProjectDependencies) = postProject
		{73919171-44B6-4536-B892-F1FCA653887C} = {73919171-44B6-4536-B892-F1FCA653887C}
	EndProjectSection
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""root"", ""root"", ""{7ADBBB81-2FA7-45CA-B97D-486C3A6624D3}""
	ProjectSection(SolutionItems) = preProject
		.gitignore = .gitignore
		.vssbe = .vssbe
		3rd-party = 3rd-party
		appveyor.yml = appveyor.yml
		AUTHORS = AUTHORS
		build_[CI_Debug].bat = build_[CI_Debug].bat
		build_[CI_Release].bat = build_[CI_Release].bat
		changelog.txt = changelog.txt
		LICENSE = LICENSE
		README.md = README.md
	EndProjectSection
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = "".gnt"", "".gnt"", ""{65FF5D56-E814-4956-89BD-7C53EC557BFE}""
	ProjectSection(SolutionItems) = preProject
		.gnt\gnt.core = .gnt\gnt.core
		.gnt\packages.config = .gnt\packages.config
	EndProjectSection
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""tools"", ""tools"", ""{849DD790-F856-493C-A19E-2560A21F6AF1}""
	ProjectSection(SolutionItems) = preProject
		tools\gnt.bat = tools\gnt.bat
		tools\msbuild.bat = tools\msbuild.bat
		tools\powershell.bat = tools\powershell.bat
	EndProjectSection
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""VsixLib"", ""VsixLib"", ""{A6FD95A3-7093-47C2-9183-5629ABC4A955}""
	ProjectSection(SolutionItems) = preProject
		VsixLib.targets = VsixLib.targets
	EndProjectSection
EndProject
Global
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Debug_net45|Any CPU.ActiveCfg = Release_net45|x64
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
            Assert.Equal(expected, data);
        }

        [Fact]
        public void MergeTest1()
        {
            using Sln sln = new(TestData.GetPathTo(@"SlnWriter\L-13\src.sln"), SlnItems.AllNoLoad);

            List<SolutionFolder> folders =
            [
                new SolutionFolder("{5738BD21-E021-4D0F-B391-0448D074302F}", "My Folder", ["item1.log", "item2.txt"])
            ];

            using SlnWriter w = new(new Dictionary<Type, HandlerValue>()
            {
                [typeof(LNestedProjects)] = new(new WNestedProjects(folders, sln.Result.ProjectItems)),
                [typeof(LProjectSolutionItems)] = new(new WProjectSolutionItems(folders)),
            });

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 17
VisualStudioVersion = 17.8.34525.116
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""My Folder"", ""My Folder"", ""{5738BD21-E021-4D0F-B391-0448D074302F}""
	ProjectSection(SolutionItems) = preProject
		item1.log = item1.log
		item2.txt = item2.txt
	EndProjectSection
EndProject
Global
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {6AB957CD-4591-40A5-B117-A7839E367EC2}
	EndGlobalSection
EndGlobal
";
            Assert.Equal(expected, w.WriteAsString(sln.Result.Map));

            using Sln sln2 = new(SlnItems.AllNoLoad, expected);
            using SlnWriter w2 = new(DefaultHandlers.MakeFrom(sln2.Result));
            Assert.Equal(expected, w2.WriteAsString(sln.Result.Map));
        }

        [Fact]
        public void MergeTest2()
        {
            using Sln sln = new(TestData.GetPathTo(@"SlnWriter\L-13\src_d.sln"), SlnItems.AllNoLoad);

            ConfigSln[] slnConfs = [ new("Debug", "x64") ];

            IConfPlatformPrj[] prjConfs =
            [
                new ConfigPrj("Debug", "x64", "{8F92E183-0B6A-406D-8ABB-77930F0F494D}", build: true, slnConfs[0])
            ];

            Dictionary<Type, HandlerValue> whandlers = new()
            {
                [typeof(LVisualStudioVersion)] = new(new WVisualStudioVersion(new SlnHeader("12.0"))),
                [typeof(LProjectConfigurationPlatforms)] = new(new WProjectConfigurationPlatforms(prjConfs)),
                [typeof(LSolutionConfigurationPlatforms)] = new(new WSolutionConfigurationPlatforms(slnConfs)),
            };

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 12.00
Global
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{8F92E183-0B6A-406D-8ABB-77930F0F494D}.Debug|x64.ActiveCfg = Debug|x64
		{8F92E183-0B6A-406D-8ABB-77930F0F494D}.Debug|x64.Build.0 = Debug|x64
	EndGlobalSection
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|x64 = Debug|x64
	EndGlobalSection
EndGlobal
";
            using SlnWriter w = new(whandlers);
            Assert.Equal(expected, w.WriteAsString([
                new Section(new LVisualStudioVersion(), null),
                new Section(new LProjectConfigurationPlatforms(), null),
                new Section(new LSolutionConfigurationPlatforms(), null),
            ]));
        }
    }
}
