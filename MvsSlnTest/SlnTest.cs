using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.SlnHandlers;

namespace net.r_eg.MvsSlnTest
{
    [TestClass]
    public class SlnTest
    {
        [TestMethod]
        public void SlnResultTest1()
        {
            using(var sln = new Sln(SlnItems.Projects, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual("\\", sln.Result.SolutionDir);
                Assert.AreEqual(SlnParser.MEM_FILE, sln.Result.Properties["SolutionPath"]);

                Assert.AreEqual(sln.Result.SolutionDir, sln.Result.Properties["SolutionDir"]);
                Assert.AreEqual(Path.GetExtension(SlnParser.MEM_FILE), sln.Result.Properties["SolutionExt"]);
                Assert.AreEqual(Path.GetFileName(SlnParser.MEM_FILE), sln.Result.Properties["SolutionFileName"]);
                Assert.AreEqual(Path.GetFileNameWithoutExtension(SlnParser.MEM_FILE), sln.Result.Properties["SolutionName"]);
                Assert.AreEqual(null, sln.Result.Properties["Configuration"]);
                Assert.AreEqual(null, sln.Result.Properties["Platform"]);
            }
        }

        [TestMethod]
        public void SlnResultTest2()
        {
            using(var sln = new Sln(SlnItems.Projects, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual(8, sln.Result.ProjectItems.Count());

                var p0 = sln.Result.ProjectItems.ElementAt(0);
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", p0.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p0.pType);
                Assert.AreEqual("CIMLib", p0.name);
                Assert.AreEqual("CIMLib\\CIMLib.csproj", p0.path);

                var p1 = sln.Result.ProjectItems.ElementAt(1);
                Assert.AreEqual("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", p1.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p1.pType);
                Assert.AreEqual("vsSolutionBuildEvent", p1.name);
                Assert.AreEqual("vsSolutionBuildEvent\\vsSolutionBuildEvent.csproj", p1.path);

                var p2 = sln.Result.ProjectItems.ElementAt(2);
                Assert.AreEqual("{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}", p2.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p2.pType);
                Assert.AreEqual("vsSolutionBuildEventTest", p2.name);
                Assert.AreEqual("vsSolutionBuildEventTest\\vsSolutionBuildEventTest.csproj", p2.path);

                var p3 = sln.Result.ProjectItems.ElementAt(3);
                Assert.AreEqual("{73919171-44B6-4536-B892-F1FCA653887C}", p3.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p3.pType);
                Assert.AreEqual("Bridge", p3.name);
                Assert.AreEqual("Bridge\\Bridge.csproj", p3.path);

                var p4 = sln.Result.ProjectItems.ElementAt(4);
                Assert.AreEqual("{56437CBB-4AE5-4405-B928-600009D60E2D}", p4.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p4.pType);
                Assert.AreEqual("Devenv", p4.name);
                Assert.AreEqual("Devenv\\Devenv.csproj", p4.path);

                var p5 = sln.Result.ProjectItems.ElementAt(5);
                Assert.AreEqual("{97F0E2FF-42DB-4506-856D-8694DD99F827}", p5.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p5.pType);
                Assert.AreEqual("CI.MSBuild", p5.name);
                Assert.AreEqual("CI.MSBuild\\CI.MSBuild.csproj", p5.path);

                var p6 = sln.Result.ProjectItems.ElementAt(6);
                Assert.AreEqual("{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}", p6.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p6.pType);
                Assert.AreEqual("Provider", p6.name);
                Assert.AreEqual("Provider\\Provider.csproj", p6.path);

                var p7 = sln.Result.ProjectItems.ElementAt(7);
                Assert.AreEqual("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", p7.pGuid);
                Assert.AreEqual("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p7.pType);
                Assert.AreEqual("ClientDemo", p7.name);
                Assert.AreEqual("ClientDemo\\ClientDemo.csproj", p7.path);
            }
        }

        [TestMethod]
        public void SlnResultTest3()
        {
            using(var sln = new Sln(SlnItems.SolutionConfPlatforms, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual(new ConfigItem("Debug", "Any CPU"), sln.Result.DefaultConfig);

                Assert.AreEqual(8, sln.Result.SolutionConfigs.Count());

                Assert.AreEqual(new ConfigItem("CI_Debug_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(0));
                Assert.AreEqual(new ConfigItem("CI_Debug", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(1));
                Assert.AreEqual(new ConfigItem("CI_Release_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(2));
                Assert.AreEqual(new ConfigItem("CI_Release", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(3));
                Assert.AreEqual(new ConfigItem("Debug_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(4));
                Assert.AreEqual(new ConfigItem("Debug", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(5));
                Assert.AreEqual(new ConfigItem("Release_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(6));
                Assert.AreEqual(new ConfigItem("Release", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(7));
            }
        }

        [TestMethod]
        public void SlnResultTest4()
        {
            using(var sln = new Sln(SlnItems.ProjectConfPlatforms, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual(6, sln.Result.ProjectConfigs.Count());

                var cfg0 = sln.Result.ProjectConfigs.ElementAt(0);
                Assert.AreEqual(new ConfigItem("Release_net45", "x64"), (ConfigItem)cfg0);
                Assert.AreEqual(false, cfg0.IncludeInBuild);
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg0.PGuid);
                Assert.AreEqual(new ConfigItem("CI_Debug_net45", "Any CPU"), cfg0.Sln);

                var cfg1 = sln.Result.ProjectConfigs.ElementAt(1);
                Assert.AreEqual(new ConfigItem("Release", "x64"), (ConfigItem)cfg1);
                Assert.AreEqual(true, cfg1.IncludeInBuild);
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg1.PGuid);
                Assert.AreEqual(new ConfigItem("CI_Debug", "Any CPU"), cfg1.Sln);

                var cfg2 = sln.Result.ProjectConfigs.ElementAt(2);
                Assert.AreEqual(new ConfigItem("Release_net45", "Any CPU"), (ConfigItem)cfg2);
                Assert.AreEqual(false, cfg2.IncludeInBuild);
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg2.PGuid);
                Assert.AreEqual(new ConfigItem("CI_Release_net45", "Any CPU"), cfg2.Sln);

                var cfg3 = sln.Result.ProjectConfigs.ElementAt(3);
                Assert.AreEqual(new ConfigItem("Release", "Any CPU"), (ConfigItem)cfg3);
                Assert.AreEqual(true, cfg3.IncludeInBuild);
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg3.PGuid);
                Assert.AreEqual(new ConfigItem("CI_Release", "Any CPU"), cfg3.Sln);

                var cfg4 = sln.Result.ProjectConfigs.ElementAt(4);
                Assert.AreEqual(new ConfigItem("Debug", "x86"), (ConfigItem)cfg4);
                Assert.AreEqual(true, cfg4.IncludeInBuild);
                Assert.AreEqual("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg4.PGuid);
                Assert.AreEqual(new ConfigItem("Debug", "Any CPU"), cfg4.Sln);

                var cfg5 = sln.Result.ProjectConfigs.ElementAt(5);
                Assert.AreEqual(new ConfigItem("Release", "Any CPU"), (ConfigItem)cfg5);
                Assert.AreEqual(true, cfg5.IncludeInBuild);
                Assert.AreEqual("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg5.PGuid);
                Assert.AreEqual(new ConfigItem("Release", "Any CPU"), cfg5.Sln);
            }
        }

        [TestMethod]
        public void SlnResultTest5()
        {
            using(var sln = new Sln(SlnItems.ProjectDependencies, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual(8, sln.Result.ProjectDependencies.Projects.Count());

                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", sln.Result.ProjectDependencies.FirstProject.pGuid);
                Assert.AreEqual("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", sln.Result.ProjectDependencies.LastProject.pGuid);

                Assert.AreEqual(8, sln.Result.ProjectDependencies.GuidList.Count());
                Assert.AreEqual("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", sln.Result.ProjectDependencies.GuidList[0]);
                Assert.AreEqual("{73919171-44B6-4536-B892-F1FCA653887C}", sln.Result.ProjectDependencies.GuidList[1]);
                Assert.AreEqual("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", sln.Result.ProjectDependencies.GuidList[2]);
                Assert.AreEqual("{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}", sln.Result.ProjectDependencies.GuidList[3]);
                Assert.AreEqual("{56437CBB-4AE5-4405-B928-600009D60E2D}", sln.Result.ProjectDependencies.GuidList[4]);
                Assert.AreEqual("{97F0E2FF-42DB-4506-856D-8694DD99F827}", sln.Result.ProjectDependencies.GuidList[5]);
                Assert.AreEqual("{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}", sln.Result.ProjectDependencies.GuidList[6]);
                Assert.AreEqual("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", sln.Result.ProjectDependencies.GuidList[7]);

                Assert.AreEqual(8, sln.Result.ProjectDependencies.Dependencies.Count());
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}"].Count);
                Assert.AreEqual(2, sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"].Count);
                {
                    Assert.AreEqual(true, sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"].Contains("{73919171-44B6-4536-B892-F1FCA653887C}"));
                    Assert.AreEqual(true, sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"].Contains("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}"));
                }
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}"].Count);
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{73919171-44B6-4536-B892-F1FCA653887C}"].Count);
                Assert.AreEqual(1, sln.Result.ProjectDependencies.Dependencies["{56437CBB-4AE5-4405-B928-600009D60E2D}"].Count);
                {
                    Assert.AreEqual(true, sln.Result.ProjectDependencies.Dependencies["{56437CBB-4AE5-4405-B928-600009D60E2D}"].Contains("{73919171-44B6-4536-B892-F1FCA653887C}"));
                }
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{97F0E2FF-42DB-4506-856D-8694DD99F827}"].Count);
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}"].Count);
                Assert.AreEqual(0, sln.Result.ProjectDependencies.Dependencies["{9673A8FC-07E1-4BB3-A97E-020481A5275E}"].Count);
            }
        }

        [TestMethod]
        public void EnvTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IEnvironment env = sln.Result.Env;

                Assert.AreEqual(4, env.Projects.Count());
                Assert.AreEqual(1, env.UniqueByGuidProjects.Count());

                Assert.AreEqual("{12B25935-229F-4128-B66B-7561A77ABC54}", env.XProjectByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}", new ConfigItem("Debug", "x86")).ProjectGuid);
                Assert.AreEqual(null, env.XProjectByGuid("none", new ConfigItem("Debug", "x86")));
                Assert.AreEqual(null, env.XProjectByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}", null));

                Assert.AreEqual(0, env.XProjectsByGuid(null).Count());

                var xprojects = env.XProjectsByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}");
                Assert.AreEqual(4, xprojects.Count());
                Assert.AreEqual("{12B25935-229F-4128-B66B-7561A77ABC54}", xprojects.ElementAt(0).ProjectGuid);
            }
        }

        [TestMethod]
        public void SlnItemsTest1()
        {
            using(var sln = new Sln(SlnItems.None, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreEqual(null, sln.Result.DefaultConfig);
                Assert.AreEqual(null, sln.Result.Env);
                Assert.AreEqual(null, sln.Result.ProjectConfigs);
                Assert.AreEqual(null, sln.Result.ProjectConfigurationPlatforms);
                Assert.AreEqual(null, sln.Result.ProjectDependencies);
                Assert.AreEqual(null, sln.Result.ProjectItems);
                Assert.AreEqual(null, sln.Result.ProjectItemsConfigs);
                Assert.AreEqual(null, sln.Result.SolutionConfigs);

                Assert.AreNotEqual(null, sln.Result.Properties);
                Assert.AreEqual(SlnItems.None, sln.Result.ResultType);
            }
        }

        [TestMethod]
        public void SlnItemsTest2()
        {
            using(var sln = new Sln(SlnItems.All &~ SlnItems.LoadDefaultData, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.AreNotEqual(null, sln.Result.DefaultConfig);
                Assert.AreNotEqual(null, sln.Result.Env);
                Assert.AreNotEqual(null, sln.Result.ProjectConfigs);
                Assert.AreNotEqual(null, sln.Result.ProjectConfigurationPlatforms);
                Assert.AreNotEqual(null, sln.Result.ProjectDependencies);
                Assert.AreNotEqual(null, sln.Result.ProjectItems);
                Assert.AreNotEqual(null, sln.Result.ProjectItemsConfigs);
                Assert.AreNotEqual(null, sln.Result.SolutionConfigs);
                Assert.AreNotEqual(null, sln.Result.Properties);

                Assert.AreEqual(SlnItems.All & ~SlnItems.LoadDefaultData, sln.Result.ResultType);
            }
        }

        [TestMethod]
        public void MapTest1()
        {
            using(var sln = new Sln(SlnItems.All &~ SlnItems.LoadDefaultData, SlnSamplesResource.vsSolutionBuildEvent_map))
            {
                var map = sln.Result.Map;
                Assert.AreEqual(38, map.Count);

                Assert.AreEqual(null, map[0].Handler);

                Assert.AreEqual(typeof(LProject), map[1].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[2].Handler.GetType());

                Assert.AreEqual(typeof(LProject), map[3].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[4].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[5].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[6].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[7].Handler.GetType());

                Assert.AreEqual(typeof(LProjectSolutionItems), map[8].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[9].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[10].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[11].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[12].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[13].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[14].Handler.GetType());

                Assert.AreEqual(typeof(LProjectSolutionItems), map[15].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[16].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[17].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[18].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[19].Handler.GetType());
                Assert.AreEqual(typeof(LProjectSolutionItems), map[20].Handler.GetType());

                Assert.AreEqual(null, map[21].Handler);

                Assert.AreEqual(typeof(LSolutionConfigurationPlatforms), map[22].Handler.GetType());
                Assert.AreEqual(typeof(LSolutionConfigurationPlatforms), map[23].Handler.GetType());
                Assert.AreEqual(typeof(LSolutionConfigurationPlatforms), map[24].Handler.GetType());
                Assert.AreEqual(typeof(LSolutionConfigurationPlatforms), map[25].Handler.GetType());
                Assert.AreEqual(typeof(LSolutionConfigurationPlatforms), map[26].Handler.GetType());

                Assert.AreEqual(typeof(LProjectConfigurationPlatforms), map[27].Handler.GetType());
                Assert.AreEqual(typeof(LProjectConfigurationPlatforms), map[28].Handler.GetType());
                Assert.AreEqual(typeof(LProjectConfigurationPlatforms), map[29].Handler.GetType());
                Assert.AreEqual(typeof(LProjectConfigurationPlatforms), map[30].Handler.GetType());
                Assert.AreEqual(typeof(LProjectConfigurationPlatforms), map[31].Handler.GetType());

                Assert.AreEqual(null, map[32].Handler);
                Assert.AreEqual(null, map[33].Handler);
                Assert.AreEqual(null, map[34].Handler);
                Assert.AreEqual(null, map[35].Handler);

                Assert.AreEqual(typeof(LProject), map[36].Handler.GetType());
                Assert.AreEqual(typeof(LProjectDependencies), map[37].Handler.GetType());
            }
        }
    }
}
