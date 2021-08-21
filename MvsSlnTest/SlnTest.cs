using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.SlnHandlers;
using Xunit;

namespace MvsSlnTest
{
    public class SlnTest
    {
        [Fact]
        public void SlnResultTest1()
        {
            using(var sln = new Sln(SlnItems.Projects, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Equal("\\", sln.Result.SolutionDir);
                Assert.Equal(SlnParser.MEM_FILE, sln.Result.Properties[PropertyNames.SLN_PATH]);

                Assert.Equal(sln.Result.SolutionDir, sln.Result.Properties[PropertyNames.SLN_DIR]);
                Assert.Equal(Path.GetExtension(SlnParser.MEM_FILE), sln.Result.Properties[PropertyNames.SLN_EXT]);
                Assert.Equal(Path.GetFileName(SlnParser.MEM_FILE), sln.Result.Properties[PropertyNames.SLN_FNAME]);
                Assert.Equal(Path.GetFileNameWithoutExtension(SlnParser.MEM_FILE), sln.Result.Properties[PropertyNames.SLN_NAME]);
                Assert.Null(sln.Result.Properties[PropertyNames.CONFIG]);
                Assert.Null(sln.Result.Properties[PropertyNames.PLATFORM]);
            }
        }

        [Fact]
        public void SlnResultTest2()
        {
            using(var sln = new Sln(SlnItems.Projects, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Equal(8, sln.Result.ProjectItems.Count());

                var p0 = sln.Result.ProjectItems.ElementAt(0);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", p0.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p0.pType);
                Assert.Equal("CIMLib", p0.name);
                Assert.Equal("CIMLib\\CIMLib.csproj", p0.path);

                var p1 = sln.Result.ProjectItems.ElementAt(1);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", p1.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p1.pType);
                Assert.Equal("vsSolutionBuildEvent", p1.name);
                Assert.Equal("vsSolutionBuildEvent\\vsSolutionBuildEvent.csproj", p1.path);

                var p2 = sln.Result.ProjectItems.ElementAt(2);
                Assert.Equal("{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}", p2.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p2.pType);
                Assert.Equal("vsSolutionBuildEventTest", p2.name);
                Assert.Equal("vsSolutionBuildEventTest\\vsSolutionBuildEventTest.csproj", p2.path);

                var p3 = sln.Result.ProjectItems.ElementAt(3);
                Assert.Equal("{73919171-44B6-4536-B892-F1FCA653887C}", p3.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p3.pType);
                Assert.Equal("Bridge", p3.name);
                Assert.Equal("Bridge\\Bridge.csproj", p3.path);

                var p4 = sln.Result.ProjectItems.ElementAt(4);
                Assert.Equal("{56437CBB-4AE5-4405-B928-600009D60E2D}", p4.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p4.pType);
                Assert.Equal("Devenv", p4.name);
                Assert.Equal("Devenv\\Devenv.csproj", p4.path);

                var p5 = sln.Result.ProjectItems.ElementAt(5);
                Assert.Equal("{97F0E2FF-42DB-4506-856D-8694DD99F827}", p5.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p5.pType);
                Assert.Equal("CI.MSBuild", p5.name);
                Assert.Equal("CI.MSBuild\\CI.MSBuild.csproj", p5.path);

                var p6 = sln.Result.ProjectItems.ElementAt(6);
                Assert.Equal("{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}", p6.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p6.pType);
                Assert.Equal("Provider", p6.name);
                Assert.Equal("Provider\\Provider.csproj", p6.path);

                var p7 = sln.Result.ProjectItems.ElementAt(7);
                Assert.Equal("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", p7.pGuid);
                Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", p7.pType);
                Assert.Equal("ClientDemo", p7.name);
                Assert.Equal("ClientDemo\\ClientDemo.csproj", p7.path);
            }
        }

        [Fact]
        public void SlnResultTest3()
        {
            using(var sln = new Sln(SlnItems.SolutionConfPlatforms, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Equal(new ConfigItem("Debug", "Any CPU"), sln.Result.DefaultConfig);

                Assert.Equal(8, sln.Result.SolutionConfigs.Count());

                Assert.Equal(new ConfigItem("CI_Debug_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(0));
                Assert.Equal(new ConfigItem("CI_Debug", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(1));
                Assert.Equal(new ConfigItem("CI_Release_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(2));
                Assert.Equal(new ConfigItem("CI_Release", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(3));
                Assert.Equal(new ConfigItem("Debug_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(4));
                Assert.Equal(new ConfigItem("Debug", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(5));
                Assert.Equal(new ConfigItem("Release_net45", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(6));
                Assert.Equal(new ConfigItem("Release", "Any CPU"), sln.Result.SolutionConfigs.ElementAt(7));
            }
        }

        [Fact]
        public void SlnResultTest4()
        {
            using(var sln = new Sln(SlnItems.ProjectConfPlatforms, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Equal(6, sln.Result.ProjectConfigs.Count());

                var cfg0 = sln.Result.ProjectConfigs.ElementAt(0);
                Assert.Equal(new ConfigItem("Release_net45", "x64"), (ConfigItem)cfg0);
                Assert.False(cfg0.IncludeInBuild);
                Assert.False(cfg0.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg0.PGuid);
                Assert.Equal(new ConfigItem("CI_Debug_net45", "Any CPU"), cfg0.Sln);

                var cfg1 = sln.Result.ProjectConfigs.ElementAt(1);
                Assert.Equal(new ConfigItem("Release", "x64"), (ConfigItem)cfg1);
                Assert.True(cfg1.IncludeInBuild);
                Assert.False(cfg1.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg1.PGuid);
                Assert.Equal(new ConfigItem("CI_Debug", "Any CPU"), cfg1.Sln);

                var cfg2 = sln.Result.ProjectConfigs.ElementAt(2);
                Assert.Equal(new ConfigItem("Release_net45", "Any CPU"), (ConfigItem)cfg2);
                Assert.False(cfg2.IncludeInBuild);
                Assert.False(cfg2.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg2.PGuid);
                Assert.Equal(new ConfigItem("CI_Release_net45", "Any CPU"), cfg2.Sln);

                var cfg3 = sln.Result.ProjectConfigs.ElementAt(3);
                Assert.Equal(new ConfigItem("Release", "Any CPU"), (ConfigItem)cfg3);
                Assert.True(cfg3.IncludeInBuild);
                Assert.True(cfg3.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg3.PGuid);
                Assert.Equal(new ConfigItem("CI_Release", "Any CPU"), cfg3.Sln);

                var cfg4 = sln.Result.ProjectConfigs.ElementAt(4);
                Assert.Equal(new ConfigItem("Debug", "x86"), (ConfigItem)cfg4);
                Assert.True(cfg4.IncludeInBuild);
                Assert.False(cfg4.IncludeInDeploy);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg4.PGuid);
                Assert.Equal(new ConfigItem("Debug", "Any CPU"), cfg4.Sln);

                var cfg5 = sln.Result.ProjectConfigs.ElementAt(5);
                Assert.Equal(new ConfigItem("Release", "Any CPU"), (ConfigItem)cfg5);
                Assert.True(cfg5.IncludeInBuild);
                Assert.False(cfg5.IncludeInDeploy);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg5.PGuid);
                Assert.Equal(new ConfigItem("Release", "Any CPU"), cfg5.Sln);
            }
        }

        [Fact]
        public void SlnResultTest5()
        {
            using(var sln = new Sln(SlnItems.ProjectDependencies, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Equal(8, sln.Result.ProjectDependencies.Projects.Count());

                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", sln.Result.ProjectDependencies.FirstProject.pGuid);
                Assert.Equal("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", sln.Result.ProjectDependencies.LastProject.pGuid);

                Assert.Equal(8, sln.Result.ProjectDependencies.GuidList.Count());
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", sln.Result.ProjectDependencies.GuidList[0]);
                Assert.Equal("{73919171-44B6-4536-B892-F1FCA653887C}", sln.Result.ProjectDependencies.GuidList[1]);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", sln.Result.ProjectDependencies.GuidList[2]);
                Assert.Equal("{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}", sln.Result.ProjectDependencies.GuidList[3]);
                Assert.Equal("{56437CBB-4AE5-4405-B928-600009D60E2D}", sln.Result.ProjectDependencies.GuidList[4]);
                Assert.Equal("{97F0E2FF-42DB-4506-856D-8694DD99F827}", sln.Result.ProjectDependencies.GuidList[5]);
                Assert.Equal("{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}", sln.Result.ProjectDependencies.GuidList[6]);
                Assert.Equal("{9673A8FC-07E1-4BB3-A97E-020481A5275E}", sln.Result.ProjectDependencies.GuidList[7]);

                Assert.Equal(8, sln.Result.ProjectDependencies.Dependencies.Count());
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}"]);
                Assert.Equal(2, sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"].Count);
                {
                    Assert.Contains("{73919171-44B6-4536-B892-F1FCA653887C}", sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"]);
                    Assert.Contains("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", sln.Result.ProjectDependencies.Dependencies["{32E44995-F621-4E77-B46A-8F65D64E7FEA}"]);
                }
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{C00D04E8-8101-42F5-89DA-CBAD205CC1D3}"]);
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{73919171-44B6-4536-B892-F1FCA653887C}"]);
                Assert.Single(sln.Result.ProjectDependencies.Dependencies["{56437CBB-4AE5-4405-B928-600009D60E2D}"]);
                {
                    Assert.Contains("{73919171-44B6-4536-B892-F1FCA653887C}", sln.Result.ProjectDependencies.Dependencies["{56437CBB-4AE5-4405-B928-600009D60E2D}"]);
                }
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{97F0E2FF-42DB-4506-856D-8694DD99F827}"]);
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}"]);
                Assert.Empty(sln.Result.ProjectDependencies.Dependencies["{9673A8FC-07E1-4BB3-A97E-020481A5275E}"]);
            }
        }

        [Fact]
        public void SlnResultTest6()
        {
            using(var sln = new Sln(SlnItems.ProjectConfPlatforms, SlnSamplesResource.ConfNamesDots))
            {
                Assert.Equal(6, sln.Result.ProjectConfigs.Count());

                var cfg = sln.Result.ProjectConfigs.ElementAt(0);
                Assert.Equal(new ConfigItem("Release.net45", "x64"), (ConfigItem)cfg);
                Assert.False(cfg.IncludeInBuild);
                Assert.False(cfg.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg.PGuid);
                Assert.Equal(new ConfigItem("CI.Debug.net45", "Any.CPU.etc"), cfg.Sln);

                cfg = sln.Result.ProjectConfigs.ElementAt(1);
                Assert.Equal(new ConfigItem("Release", "x64"), (ConfigItem)cfg);
                Assert.True(cfg.IncludeInBuild);
                Assert.False(cfg.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg.PGuid);
                Assert.Equal(new ConfigItem("CI.Debug", "Any.CPU.etc"), cfg.Sln);

                cfg = sln.Result.ProjectConfigs.ElementAt(2);
                Assert.Equal(new ConfigItem("Release.net45", "Any.CPU.etc"), (ConfigItem)cfg);
                Assert.False(cfg.IncludeInBuild);
                Assert.False(cfg.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg.PGuid);
                Assert.Equal(new ConfigItem("CI.Release.net45", "Any.CPU.etc"), cfg.Sln);

                cfg = sln.Result.ProjectConfigs.ElementAt(3);
                Assert.Equal(new ConfigItem("Release", "Any.CPU.etc"), (ConfigItem)cfg);
                Assert.True(cfg.IncludeInBuild);
                Assert.True(cfg.IncludeInDeploy);
                Assert.Equal("{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}", cfg.PGuid);
                Assert.Equal(new ConfigItem("CI.Release", "Any.CPU.etc"), cfg.Sln);

                cfg = sln.Result.ProjectConfigs.ElementAt(4);
                Assert.Equal(new ConfigItem("Debug.x64.x86", "x86"), (ConfigItem)cfg);
                Assert.True(cfg.IncludeInBuild);
                Assert.False(cfg.IncludeInDeploy);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg.PGuid);
                Assert.Equal(new ConfigItem("Debug.x64.x86", "Any.CPU.etc"), cfg.Sln);

                cfg = sln.Result.ProjectConfigs.ElementAt(5);
                Assert.Equal(new ConfigItem("Release", "Any.CPU.etc"), (ConfigItem)cfg);
                Assert.True(cfg.IncludeInBuild);
                Assert.False(cfg.IncludeInDeploy);
                Assert.Equal("{32E44995-F621-4E77-B46A-8F65D64E7FEA}", cfg.PGuid);
                Assert.Equal(new ConfigItem("Release", "Any.CPU.etc"), cfg.Sln);
            }
        }

        [Fact]
        public void ExtensibilityGlobalsTest1()
        {
            using(var sln = new Sln(SlnItems.All &~ SlnItems.ExtItems, SlnSamplesResource.ExtensibilityGlobals))
            {
                Assert.Null(sln.Result.ExtItems);
            }
        }

        [Fact]
        public void ExtensibilityGlobalsTest2()
        {
            using(var sln = new Sln(SlnItems.ExtItems, SlnSamplesResource.ExtensibilityGlobals))
            {
                Assert.Equal(4, sln.Result.ExtItems.Count());

                Assert.True(sln.Result.ExtItems.ContainsKey("SolutionGuid"));
                Assert.True(sln.Result.ExtItems.ContainsKey("SomeOtherEmptyData"));
                Assert.True(sln.Result.ExtItems.ContainsKey("SomeNullData"));
                Assert.True(sln.Result.ExtItems.ContainsKey("EnterpriseLibraryConfigurationToolBinariesPath"));

                Assert.Equal("{B3244B90-20DE-4D69-8692-EBC686503F90}", sln.Result.ExtItems["SolutionGuid"]);
                Assert.Equal(String.Empty, sln.Result.ExtItems["SomeOtherEmptyData"]);
                Assert.Null(sln.Result.ExtItems["SomeNullData"]);
                Assert.Equal(@"packages\Conari.1.3.0\lib\NET40;packages\vsSBE.CI.MSBuild\bin", sln.Result.ExtItems["EnterpriseLibraryConfigurationToolBinariesPath"]);
            }
        }

        [Fact]
        public void SolutionFoldersAndProjectsTest1()
        {
            using(var sln = new Sln(SlnItems.SolutionItems | SlnItems.Projects, SlnSamplesResource.SolutionFoldersAndProjects))
            {
                Assert.Equal(4, sln.Result.SolutionFolders.Count());
                Assert.Equal(5, sln.Result.ProjectItems.Count());

                Assert.Equal("dir1", sln.Result.SolutionFolders.ElementAt(0).header.name);
                Assert.Equal("{1571A74C-579F-4C91-8484-322B7D89B430}", sln.Result.SolutionFolders.ElementAt(0).header.pGuid);

                Assert.Equal("subdir1", sln.Result.SolutionFolders.ElementAt(1).header.name);
                Assert.Equal("{260202C1-F43A-43E9-A43B-E3E75406A985}", sln.Result.SolutionFolders.ElementAt(1).header.pGuid);

                Assert.Equal("dir3", sln.Result.SolutionFolders.ElementAt(2).header.name);
                Assert.Equal("{49B0FC00-2D07-419C-86D4-FDEC8BF0F12C}", sln.Result.SolutionFolders.ElementAt(2).header.pGuid);

                Assert.Equal("dir2", sln.Result.SolutionFolders.ElementAt(3).header.name);
                Assert.Equal("{AB650B89-1B1B-43C6-B254-226B56ACB6EB}", sln.Result.SolutionFolders.ElementAt(3).header.pGuid);
            }
        }

        [Fact]
        public void SolutionFoldersAndProjectsTest2()
        {
            using(var sln = new Sln(SlnItems.SolutionItems | SlnItems.Projects, SlnSamplesResource.SolutionFoldersAndProjects))
            {
                Assert.Equal(4, sln.Result.SolutionFolders.Count());
                Assert.Equal(5, sln.Result.ProjectItems.Count());

                Assert.Empty(sln.Result.SolutionFolders.ElementAt(0).items);
                Assert.Equal(2, sln.Result.SolutionFolders.ElementAt(1).items.Count());
                Assert.Single(sln.Result.SolutionFolders.ElementAt(2).items);
                Assert.Empty(sln.Result.SolutionFolders.ElementAt(3).items);

                Assert.Equal(".gitignore", sln.Result.SolutionFolders.ElementAt(1).items.ElementAt(0).data);
                Assert.Equal("Readme.txt", sln.Result.SolutionFolders.ElementAt(1).items.ElementAt(1).data);

                Assert.Equal(".gitattributes", sln.Result.SolutionFolders.ElementAt(2).items.ElementAt(0).data);
            }
        }

        [Fact]
        public void SolutionFoldersAndProjectsTest3()
        {
            using(var sln = new Sln(SlnItems.SolutionItems | SlnItems.Projects, SlnSamplesResource.SolutionFoldersAndProjects))
            {
                Assert.Null(sln.Result.SolutionFolders.ElementAt(0).header.parent.Value);

                Assert.NotNull(sln.Result.SolutionFolders.ElementAt(1).header.parent.Value);
                Assert.Equal("{1571A74C-579F-4C91-8484-322B7D89B430}", sln.Result.SolutionFolders.ElementAt(1).header.parent.Value?.header.pGuid);
                Assert.Equal("dir1", sln.Result.SolutionFolders.ElementAt(1).header.parent.Value?.header.name);

                Assert.NotNull(sln.Result.SolutionFolders.ElementAt(2).header.parent.Value);
                Assert.Equal("{260202C1-F43A-43E9-A43B-E3E75406A985}", sln.Result.SolutionFolders.ElementAt(2).header.parent.Value?.header.pGuid);
                Assert.Equal("subdir1", sln.Result.SolutionFolders.ElementAt(2).header.parent.Value?.header.name);

                Assert.Null(sln.Result.SolutionFolders.ElementAt(3).header.parent.Value);
            }
        }

        [Fact]
        public void SolutionFoldersAndProjectsTest4()
        {
            using(var sln = new Sln(SlnItems.SolutionItems | SlnItems.Projects, SlnSamplesResource.SolutionFoldersAndProjects))
            {
                Assert.NotNull(sln.Result.ProjectItems.ElementAt(0).parent.Value);
                Assert.Equal("{1571A74C-579F-4C91-8484-322B7D89B430}", sln.Result.ProjectItems.ElementAt(0).parent.Value?.header.pGuid);
                Assert.Equal("dir1", sln.Result.ProjectItems.ElementAt(0).parent.Value?.header.name);

                Assert.NotNull(sln.Result.ProjectItems.ElementAt(1).parent.Value);
                Assert.Equal("{260202C1-F43A-43E9-A43B-E3E75406A985}", sln.Result.ProjectItems.ElementAt(1).parent.Value?.header.pGuid);
                Assert.Equal("subdir1", sln.Result.ProjectItems.ElementAt(1).parent.Value?.header.name);

                Assert.Null(sln.Result.ProjectItems.ElementAt(2).parent.Value);

                Assert.NotNull(sln.Result.ProjectItems.ElementAt(3).parent.Value);
                Assert.Equal("{49B0FC00-2D07-419C-86D4-FDEC8BF0F12C}", sln.Result.ProjectItems.ElementAt(3).parent.Value?.header.pGuid);
                Assert.Equal("dir3", sln.Result.ProjectItems.ElementAt(3).parent.Value?.header.name);

                Assert.NotNull(sln.Result.ProjectItems.ElementAt(4).parent.Value);
                Assert.Equal("{AB650B89-1B1B-43C6-B254-226B56ACB6EB}", sln.Result.ProjectItems.ElementAt(4).parent.Value?.header.pGuid);
                Assert.Equal("dir2", sln.Result.ProjectItems.ElementAt(4).parent.Value?.header.name);
            }
        }

        [Fact]
        public void EnvTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IEnvironment env = sln.Result.Env;

                Assert.Equal(4, env.Projects.Count());
                Assert.Single(env.UniqueByGuidProjects);

                Assert.Equal("{12B25935-229F-4128-B66B-7561A77ABC54}", env.XProjectByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}", new ConfigItem("Debug", "x86")).ProjectGuid);
                Assert.Null(env.XProjectByGuid("none", new ConfigItem("Debug", "x86")));
                Assert.Null(env.XProjectByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}", null));

                Assert.Empty(env.XProjectsByGuid(null));

                var xprojects = env.XProjectsByGuid("{12B25935-229F-4128-B66B-7561A77ABC54}");
                Assert.Equal(4, xprojects.Count());
                Assert.Equal("{12B25935-229F-4128-B66B-7561A77ABC54}", xprojects.ElementAt(0).ProjectGuid);
            }
        }

        [Fact]
        public void SlnItemsTest1()
        {
            using(var sln = new Sln(SlnItems.None, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.Null(sln.Result.DefaultConfig);
                Assert.Null(sln.Result.Env);
                Assert.Null(sln.Result.ProjectConfigs);
                Assert.Null(sln.Result.ProjectConfigurationPlatforms);
                Assert.Null(sln.Result.ProjectDependencies);
                Assert.Null(sln.Result.ProjectItems);
                Assert.Null(sln.Result.ProjectItemsConfigs);
                Assert.Null(sln.Result.SolutionConfigs);

                Assert.NotNull(sln.Result.Properties);
                Assert.Equal(SlnItems.None, sln.Result.ResultType);
            }
        }

        [Fact]
        public void SlnItemsTest2()
        {
            using(var sln = new Sln(SlnItems.All &~ SlnItems.LoadDefaultData, SlnSamplesResource.vsSolutionBuildEvent))
            {
                Assert.NotNull(sln.Result.DefaultConfig);
                Assert.NotNull(sln.Result.Env);
                Assert.NotNull(sln.Result.ProjectConfigs);
                Assert.NotNull(sln.Result.ProjectConfigurationPlatforms);
                Assert.NotNull(sln.Result.ProjectDependencies);
                Assert.NotNull(sln.Result.ProjectItems);
                Assert.NotNull(sln.Result.ProjectItemsConfigs);
                Assert.NotNull(sln.Result.SolutionConfigs);
                Assert.NotNull(sln.Result.Properties);

                Assert.Equal(SlnItems.All & ~SlnItems.LoadDefaultData, sln.Result.ResultType);
            }
        }

        [Theory]
        [InlineData(SlnItems.All)]
        [InlineData(SlnItems.ProjectDependenciesXml)]
        public void SlnItemsTest3(SlnItems items)
        {
            //report https://github.com/3F/MvsSln/issues/25#issuecomment-743840401
            using(var sln = new Sln(items, string.Empty))
            {
                Assert.Null(sln.Result.ProjectDependencies);
            }
        }

        [Fact]
        public void MapTest1()
        {
            using(var sln = new Sln(SlnItems.All &~ SlnItems.LoadDefaultData, SlnSamplesResource.vsSolutionBuildEvent_map))
            {
                var map = sln.Result.Map;
                Assert.Equal(38, map.Count);

                Assert.Equal(typeof(LVisualStudioVersion), map[0].Handler.GetType());

                Assert.Equal(typeof(LProject), map[1].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[2].Handler.GetType());

                Assert.Equal(typeof(LProject), map[3].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[4].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[5].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[6].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[7].Handler.GetType());

                Assert.Equal(typeof(LProjectSolutionItems), map[8].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[9].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[10].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[11].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[12].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[13].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[14].Handler.GetType());

                Assert.Equal(typeof(LProjectSolutionItems), map[15].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[16].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[17].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[18].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[19].Handler.GetType());
                Assert.Equal(typeof(LProjectSolutionItems), map[20].Handler.GetType());

                Assert.Null(map[21].Handler);

                Assert.Equal(typeof(LSolutionConfigurationPlatforms), map[22].Handler.GetType());
                Assert.Equal(typeof(LSolutionConfigurationPlatforms), map[23].Handler.GetType());
                Assert.Equal(typeof(LSolutionConfigurationPlatforms), map[24].Handler.GetType());
                Assert.Equal(typeof(LSolutionConfigurationPlatforms), map[25].Handler.GetType());
                Assert.Equal(typeof(LSolutionConfigurationPlatforms), map[26].Handler.GetType());

                Assert.Equal(typeof(LProjectConfigurationPlatforms), map[27].Handler.GetType());
                Assert.Equal(typeof(LProjectConfigurationPlatforms), map[28].Handler.GetType());
                Assert.Equal(typeof(LProjectConfigurationPlatforms), map[29].Handler.GetType());
                Assert.Equal(typeof(LProjectConfigurationPlatforms), map[30].Handler.GetType());
                Assert.Equal(typeof(LProjectConfigurationPlatforms), map[31].Handler.GetType());

                Assert.Null(map[32].Handler);
                Assert.Null(map[33].Handler);
                Assert.Null(map[34].Handler);
                Assert.Null(map[35].Handler);

                Assert.Equal(typeof(LProject), map[36].Handler.GetType());
                Assert.Equal(typeof(LProjectDependencies), map[37].Handler.GetType());
            }
        }
    }
}
