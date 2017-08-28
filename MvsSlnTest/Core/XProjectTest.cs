using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class XProjectTest
    {
        [TestMethod]
        public void PropertiesTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                Assert.AreEqual(null, project.GetProperty("NOT_REAL_PROPERTY").name);
                Assert.AreEqual("MyProperty1", project.SetProperty("MyProperty1", "Value1").name);
                Assert.AreEqual("Value1", project.GetProperty("MyProperty1").evaluatedValue);

                Assert.AreEqual(false, project.RemoveProperty("NOT_REAL_PROPERTY_2"));
                Assert.AreEqual(true, project.RemoveProperty("MyProperty1"));
                Assert.AreEqual(null, project.GetProperty("MyProperty1").name);
            }
        }

        [TestMethod]
        public void ItemsTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                Assert.AreEqual(null, project.GetItem("Reference", "NOT_REAL_INC").evaluatedInclude);
                Assert.AreEqual(true, project.AddItem("Reference", "MyInclude"));
                Assert.AreEqual("MyInclude", project.GetItem("Reference", "MyInclude").evaluatedInclude);

                Assert.AreEqual(false, project.RemoveItem("Reference", "NOT_REAL_INC"));
                Assert.AreEqual(true, project.RemoveItem("Reference", "MyInclude"));
                Assert.AreEqual(null, project.GetItem("Reference", "MyInclude").evaluatedInclude);
            }
        }
    }
}
