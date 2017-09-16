using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Exceptions;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class SlnWriterTest
    {
        private SlnItems defaultSlnItems = SlnItems.Projects
                                            | SlnItems.SolutionConfPlatforms
                                            | SlnItems.ProjectConfPlatforms
                                            | SlnItems.ProjectDependencies
                                            | SlnItems.Map;

        [TestMethod]
        [ExpectedException(typeof(CoHandlerRuleException))]
        public void ValidationTest1()
        {
            using(var sln = new Sln(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent))
            {
                var handlers = new Dictionary<Type, HandlerValue>() {
                    [typeof(LProject)] = new HandlerValue(),
                    [typeof(LProjectDependencies)] = new HandlerValue(),
                    [typeof(LProjectConfigurationPlatforms)] = new HandlerValue(),
                };

                using(var stream = new StreamWriter(new MemoryStream())) {
                    (new SlnWriter(stream, handlers)).Write(sln.Result.Map);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CoHandlerRuleException))]
        public void ValidationTest2()
        {
            using(var sln = new Sln(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent))
            {
                var handlers = new Dictionary<Type, HandlerValue>() {
                    [typeof(LProjectDependencies)] = new HandlerValue(),
                };

                using(var stream = new StreamWriter(new MemoryStream())) {
                    (new SlnWriter(stream, handlers)).Write(sln.Result.Map);
                }
            }
        }

        [TestMethod]
        public void MapRefTest1()
        {
            using(var sln = new Sln(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent))
            {
                int expectedCount   = sln.Result.Map.Count;
                var expectedMap     = sln.Result.Map.Select(s => s.Clone()).ToArray();

                var handlers = new Dictionary<Type, HandlerValue>() {
                    [typeof(LProject)] = new HandlerValue(),
                    [typeof(LProjectConfigurationPlatforms)] = new HandlerValue(),
                    [typeof(LSolutionConfigurationPlatforms)] = new HandlerValue(),
                };

                using(var stream = new StreamWriter(new MemoryStream()))
                {
                    (new SlnWriter(stream, handlers)).Write(sln.Result.Map);
                    Assert.AreEqual(expectedCount, sln.Result.Map.Count);
                }
            }
        }
    }
}
