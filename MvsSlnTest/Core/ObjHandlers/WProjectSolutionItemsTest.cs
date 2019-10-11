using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WProjectSolutionItemsTest
    {
        [Fact]
        public void ExtractTest1()
        {
            var data = new List<SolutionFolder>()
            {
                new SolutionFolder
                (
                    new ProjectItem()
                    {
                        pType   = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}",
                        name    = ".gnt",
                        path    = ".gnt",
                        pGuid   = "{65FF5D56-E814-4956-89BD-7C53EC557BFE}"
                    },
                    new List<RawText>()
                    {
                        ".gnt\\gnt.core",
                        ".gnt\\packages.config"
                    }
                ),

                new SolutionFolder
                (
                    new ProjectItem()
                    {
                        pType   = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}",
                        name    = "tools",
                        path    = "tools",
                        pGuid   = "{849DD790-F856-493C-A19E-2560A21F6AF1}"
                    },
                    new List<RawText>(){
                        "tools\\gnt.bat"
                    }
                ),
            };

            var target = (new WProjectSolutionItems(data)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_Sln_Items, target);
        }
    }
}
