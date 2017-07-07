using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class ConfigItemTest
    {
        [TestMethod]
        public void EqTest1()
        {
            ConfigItem target1 = new ConfigItem("Debug", "Any CPU");

            Assert.AreEqual("Debug", target1.Configuration);
            Assert.AreEqual("Debug", target1.ConfigurationByRule);
            Assert.AreEqual("Any CPU", target1.Platform);
            Assert.AreEqual("AnyCPU", target1.PlatformByRule);


            Assert.AreEqual(target1, new ConfigItem("Debug", "Any CPU"));
            Assert.AreEqual(target1, new ConfigItem("Debug", "AnyCPU"));
            Assert.AreEqual(target1, new ConfigItem("deBUG", "anycpu"));
            Assert.AreNotEqual(target1, new ConfigItem("Release", "Any CPU"));
            Assert.AreNotEqual(target1, new ConfigItem("Debug", "Any CP"));
        }

        [TestMethod]
        public void EqTest2()
        {
            var target1 = new ConfigItem("Config1|Platform1");

            Assert.AreEqual("Config1", target1.Configuration);
            Assert.AreEqual("Platform1", target1.Platform);

            var target2 = new ConfigItem(null);

            Assert.AreEqual(null, target2.Configuration);
            Assert.AreEqual(null, target2.ConfigurationByRule);
            Assert.AreEqual(null, target2.Platform);
            Assert.AreEqual(null, target2.PlatformByRule);

            var target3 = new ConfigItem("Config1", "Platform1");
            Assert.AreEqual("Config1|Platform1", target3.ToString());
            Assert.AreEqual("Config1|Platform1", target3.Format());
        }

        [TestMethod]
        public void EqRuleTest1()
        {
            var target = new ConfigItem("Debug", "Any CPU");

            Assert.AreEqual("Debug", target.Configuration);
            Assert.AreEqual("Debug", target.ConfigurationByRule);
            Assert.AreEqual("Any CPU", target.Platform);
            Assert.AreEqual("AnyCPU", target.PlatformByRule);
        }

        [TestMethod]
        public void EqRuleTest2()
        {
            Assert.AreEqual(new ConfigItem("Debug", "Platform1"), new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });
            Assert.AreEqual(new ConfigItem("Debug", "Platform1") { SensitivityComparing = true }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });

            Assert.AreNotEqual(new ConfigItem("Debug", "Platform1"), new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = false });
            Assert.AreNotEqual(new ConfigItem("Debug", "Platform1") { SensitivityComparing = false }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = false });
            Assert.AreNotEqual(new ConfigItem("Debug", "Platform1") { SensitivityComparing = false }, new ConfigItem("deBUG", "platFORM1") { SensitivityComparing = true });
        }
    }
}
