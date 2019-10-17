using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Exceptions;
using Xunit;

namespace MvsSlnTest.Core
{
    public class SlnWriterTest
    {
        private SlnItems defaultSlnItems = SlnItems.Projects
                                            | SlnItems.SolutionConfPlatforms
                                            | SlnItems.ProjectConfPlatforms
                                            | SlnItems.ProjectDependencies
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
                    Assert.Equal(expectedCount, sln.Result.Map.Count);
                }
            }
        }
    }
}
