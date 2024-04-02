using System;
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

        [Fact]
        public async Task WriteTest1Async()
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
            string data = await w.WriteAsStringAsync(sln.Result.Map);

            Assert.Equal(data, w.WriteAsString(sln.Result.Map));


            StreamWriter sw = new(new MemoryStream());
            using SlnWriter wm = new(sw, handlers);

            await wm.WriteAsync(sln.Result.Map);

            sw.Flush();
            sw.BaseStream.Position = 0;

            using StreamReader sr = new(sw.BaseStream);

            Assert.Equal(data, await sr.ReadToEndAsync());

            w.Write(sln.Result.Map);
            await w.WriteAsync(sln.Result.Map);
            
            Assert.StartsWith(data, await w.WriteAsStringAsync(sln.Result.Map));

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
    }
}
