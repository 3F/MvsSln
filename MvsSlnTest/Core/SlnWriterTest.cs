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
        public async Task WriteTest1()
        {
            using Sln sln = new(defaultSlnItems, SlnSamplesResource.vsSolutionBuildEvent);

            Dictionary<Type, HandlerValue> handlers = new()
            {
                [typeof(LVisualStudioVersion)] = new(new WVisualStudioVersion(SlnHeader.MakeDefault())),
                [typeof(LSolutionConfigurationPlatforms)] = new(),
                [typeof(LProject)] = new(new WProject(sln.Result.ProjectItems, sln.Result.ProjectDependencies)),
                [typeof(LProjectConfigurationPlatforms)] = new(new WProjectConfigurationPlatforms(sln.Result.ProjectConfigs)),
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
        }
    }
}
