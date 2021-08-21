using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class ConfigItemTest
    {
        [Fact]
        public void EqTest1()
        {
            ConfigItem target1 = new ConfigItem("Debug", "Any CPU");

            Assert.Equal("Debug", target1.Configuration);
            Assert.Equal("Debug", target1.ConfigurationByRule);
            Assert.Equal("Any CPU", target1.Platform);
            Assert.Equal("AnyCPU", target1.PlatformByRule);


            Assert.Equal(target1, new ConfigItem("Debug", "Any CPU"));
            Assert.Equal(target1, new ConfigItem("Debug", "AnyCPU"));
            Assert.Equal(target1, new ConfigItem("deBUG", "anycpu"));
            Assert.NotEqual(target1, new ConfigItem("Release", "Any CPU"));
            Assert.NotEqual(target1, new ConfigItem("Debug", "Any CP"));
        }

        [Fact]
        public void EqTest2()
        {
            var target1 = new ConfigItem("Config1|Platform1");

            Assert.Equal("Config1", target1.Configuration);
            Assert.Equal("Platform1", target1.Platform);

            var target2 = new ConfigItem(null);

            Assert.Null(target2.Configuration);
            Assert.Null(target2.ConfigurationByRule);
            Assert.Null(target2.Platform);
            Assert.Null(target2.PlatformByRule);

            var target3 = new ConfigItem("Config1", "Platform1");
            Assert.Equal("Config1|Platform1", target3.ToString());

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Equal("Config1|Platform1", target3.Format());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void EqRuleTest1()
        {
            var target = new ConfigItem("Debug", "Any CPU");

            Assert.Equal("Debug", target.Configuration);
            Assert.Equal("Debug", target.ConfigurationByRule);
            Assert.Equal("Any CPU", target.Platform);
            Assert.Equal("AnyCPU", target.PlatformByRule);
        }

        [Fact]
        public void EqRuleTest2()
        {
            Assert.Equal(new ConfigItem("Debug", "Platform1"), new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });
            Assert.Equal(new ConfigItem("Debug", "Platform1") { SensitivityComparing = true }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });

            Assert.NotEqual(new ConfigItem("Debug", "Platform1"), new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = false });
            Assert.NotEqual(new ConfigItem("Debug", "Platform1") { SensitivityComparing = false }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = false });
            Assert.NotEqual(new ConfigItem("Debug", "Platform1") { SensitivityComparing = false }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });
        }

        [Fact]
        public void IsEqualByRuleTest1()
        {
            var target = new ConfigItem("Debug", "Any CPU");

            Assert.True(target.IsEqualByRule("Debug", "Any CPU", false));
            Assert.False(target.IsEqualByRule("debug", "Any CPU", false));

            Assert.True(target.IsEqualByRule("Debug", "AnyCPU", false));
            Assert.False(target.IsEqualByRule("Debug", "Anycpu", false));

            Assert.True(target.IsEqualByRule("Debug", "Anycpu", true));

            Assert.False(target.IsEqualByRule(null, null, false));
            Assert.False(target.IsEqualByRule(null, null, true));
        }

        [Fact]
        public void NoPlatformTest1()
        {
            var target = new ConfigItem("Config");

            Assert.Equal("Config", target.Configuration);
            Assert.Equal(string.Empty, target.Platform);

            target = new ConfigItem("Config|");

            Assert.Equal("Config", target.Configuration);
            Assert.Equal(string.Empty, target.Platform);

            target = new ConfigItem("Config| ");

            Assert.Equal("Config", target.Configuration);
            Assert.Equal(" ", target.Platform);
        }

        [Fact]
        public void CtorTest1()
        {
            var target = new ConfigItem(string.Empty);

            Assert.Equal(string.Empty, target.Configuration);
            Assert.Equal(string.Empty, target.Platform);

            target = new ConfigItem(" ");

            Assert.Equal(" ", target.Configuration);
            Assert.Equal(string.Empty, target.Platform);

            target = new ConfigItem(null);

            Assert.Null(target.Configuration);
            Assert.Null(target.Platform);
        }

        [Fact]
        public void FormatTest1()
        {
            string input   = "Debug|Any CPU";
            string rulemod = "Debug|AnyCPU";

            Assert.Equal(input, new ConfigItem(input).ToString());
            Assert.Equal(input, new ConfigItem((IRuleOfConfig)null, input).ToString());

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Equal(input, new ConfigItem(input).Format());
            Assert.Equal(input, new ConfigItem((IRuleOfConfig)null, input).Format());
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Equal(rulemod, new ConfigItem(input).Formatted);
            Assert.Equal(input, new ConfigItem((IRuleOfConfig)null, input).Formatted);
        }

        [Fact]
        public void FormatTest2()
        {
            string name     = "Debug";
            string platform = "Any CPU";

            string res1 = "Debug|Any CPU";
            string res2 = "Debug|AnyCPU";

            Assert.Equal(res1, new ConfigItem(name, platform).ToString());
            Assert.Equal(res1, new ConfigItem(null, name, platform).ToString());

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Equal(res1, new ConfigItem(name, platform).Format());
            Assert.Equal(res1, new ConfigItem(null, name, platform).Format());
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Equal(res2, new ConfigItem(name, platform).Formatted);
            Assert.Equal(res1, new ConfigItem(null, name, platform).Formatted);
        }
    }
}
