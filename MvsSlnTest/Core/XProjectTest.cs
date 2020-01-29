using System;
using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class XProjectTest
    {
        [Fact]
        public void PropertiesTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                Assert.Null(project.GetProperty("NOT_REAL_PROPERTY").name);
                Assert.Equal("MyProperty1", project.SetProperty("MyProperty1", "Value1").name);
                Assert.Equal("Value1", project.GetProperty("MyProperty1").evaluatedValue);

                Assert.False(project.RemoveProperty("NOT_REAL_PROPERTY_2"));
                Assert.True(project.RemoveProperty("MyProperty1"));
                Assert.Null(project.GetProperty("MyProperty1").name);
            }
        }

        [Fact]
        public void PropertiesTest2()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                // 'Platform' is global and not XML property
                var prop = project.GetProperty("Platform", false);

                Assert.Equal("Platform", prop.name);
                Assert.Equal("x86", prop.unevaluatedValue);
                Assert.Equal("x86", prop.evaluatedValue);
                Assert.NotNull(prop.parentProperty);
                Assert.Null(prop.parentProperty.Xml);
                Assert.NotNull(prop.parentProject);
            }
        }

        [Fact]
        public void PropertiesTest3()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                var prop = project.GetProperty("Platform", true);

                Assert.Null(prop.name);
                Assert.Null(prop.unevaluatedValue);
                Assert.Null(prop.evaluatedValue);
                Assert.Null(prop.parentProperty);
                Assert.NotNull(prop.parentProject);
            }
        }

        [Fact]
        public void PropertiesTest4()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    project.SetProperty("Platform", "x64");
                });

                Assert.Throws<InvalidOperationException>(() =>
                {
                    project.SetProperty("Configuration", "Release");
                });
            }
        }

        [Fact]
        public void ItemsTest1()
        {
            var projects = new Dictionary<string, RawText>() {
                ["{12B25935-229F-4128-B66B-7561A77ABC54}"] = new RawText(PrjSamplesResource.snet)
            };

            using(var sln = new Sln(SlnItems.EnvWithProjects, new RawText(SlnSamplesResource.regXwild), projects))
            {
                IXProject project = sln.Result.Env.Projects.FirstOrDefault();

                Assert.Null(project.GetItem("Reference", "NOT_REAL_INC").evaluatedInclude);
                Assert.True(project.AddItem("Reference", "MyInclude"));
                Assert.Equal("MyInclude", project.GetItem("Reference", "MyInclude").evaluatedInclude);

                Assert.False(project.RemoveItem("Reference", "NOT_REAL_INC"));
                Assert.True(project.RemoveItem("Reference", "MyInclude"));
                Assert.Null(project.GetItem("Reference", "MyInclude").evaluatedInclude);
            }
        }
    }
}
