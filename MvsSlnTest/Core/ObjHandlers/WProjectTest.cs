using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WProjectTest
    {
        [Fact]
        public void ExtractTest1()
        {
            var data    = new _WProjectTestData();
            var target  = (new WProject(data.ProjectItems, data)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_Project_Dep, target);

            string firstLine = target.Substring(0, target.IndexOfAny(new[] { '\r', '\n' }));
            Assert.Matches(RPatterns.ProjectLine, firstLine);
        }

        private class _WProjectTestData: ISlnProjectDependencies
        {
            public IList<ProjectItem> ProjectItems
            {
                get;
                protected set;
            }

            public IList<string> GuidList { get; set; }
            public IDictionary<string, ProjectItem> Projects { get; set; }
            public IDictionary<string, HashSet<string>> Dependencies { get; set; }

            public _WProjectTestData()
            {
                var pItems = new List<ProjectItem>()
                {
                    new ProjectItem()
                    {
                        pType   = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                        name    = "Bridge",
                        path    = "Bridge\\Bridge.csproj",
                        pGuid   = "{73919171-44B6-4536-B892-F1FCA653887C}"
                    },
                    new ProjectItem()
                    {
                        pType   = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                        name    = "CIMLib",
                        path    = "CIMLib\\CIMLib.csproj",
                        pGuid   = "{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}"
                    },
                    new ProjectItem()
                    {
                        pType   = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                        name    = "Provider",
                        path    = "Provider\\Provider.csproj",
                        pGuid   = "{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}"
                    }
                };

                ProjectItems = pItems;

                Projects = new Dictionary<string, ProjectItem>()
                {
                    [pItems[0].pGuid] = pItems[0],
                    [pItems[1].pGuid] = pItems[1],
                    [pItems[2].pGuid] = pItems[2]
                };

                Dependencies = new Dictionary<string, HashSet<string>>()
                {
                    [pItems[0].pGuid] = new HashSet<string>(),
                    [pItems[1].pGuid] = new HashSet<string>()
                    {
                            "{73919171-44B6-4536-B892-F1FCA653887C}",
                            "{4F8BB8CD-1116-4F07-9B8F-06D69FB8589B}"
                    },
                    [pItems[2].pGuid] = new HashSet<string>()
                    {
                            "{73919171-44B6-4536-B892-F1FCA653887C}"
                    }
                };
            }
        }
    }
}
