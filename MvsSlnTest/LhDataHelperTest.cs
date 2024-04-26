using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest
{
    public class LhDataHelperTest
    {
        [Fact]
        public void DataTest1()
        {
            using Sln sln = new(TestData.GetPathTo(@"SlnWriter\L-13\src_d.sln"), SlnItems.AllNoLoad);

            ConfigSln[] slnConf = [new("Debug", "x64")];

            IConfPlatformPrj[] prjConfs =
            [
                new ConfigPrj("Debug", "x64", "{8F92E183-0B6A-406D-8ABB-77930F0F494D}", build: true, slnConf[0])
            ];

            LhDataHelper hdata = new();
            hdata.SetHeader(new SlnHeader("12.0"))
                    .SetProjectConfigs(prjConfs)
                    .SetSolutionConfigs(slnConf);

            string expected =
@"Microsoft Visual Studio Solution File, Format Version 12.00
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|x64 = Debug|x64
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{8F92E183-0B6A-406D-8ABB-77930F0F494D}.Debug|x64.ActiveCfg = Debug|x64
		{8F92E183-0B6A-406D-8ABB-77930F0F494D}.Debug|x64.Build.0 = Debug|x64
	EndGlobalSection
EndGlobal
";
            using SlnWriter w = new(DefaultHandlers.MakeFrom(hdata));
            Assert.Equal(expected, w.WriteAsString());

            using SlnWriter w2 = new(hdata);
            Assert.Equal(expected, w2.WriteAsString());
        }
    }
}
