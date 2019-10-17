using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using Xunit;

namespace MvsSlnTest.Core.ObjHandlers
{
    public class WNestedProjectsTest
    {
        protected List<SolutionFolder> folders = new List<SolutionFolder>()
        {
                new SolutionFolder("{8B05CE1C-999E-460D-80FF-E44DFEC019E7}", "dir1", null),
                new SolutionFolder("{BEB7D6F5-5065-4427-9797-2AD7511862D8}", "dir2", null),
                new SolutionFolder("{1257C186-1EC7-41C6-9344-14B2341BE6F6}", "dir3", null),
                new SolutionFolder("{80AEF083-D86A-416E-88B8-300DF20399AF}", "dir4", null),
        };

        protected List<ProjectItem> projects = new List<ProjectItem>()
        {
            new ProjectItem("{A2D23001-08A8-49AF-A975-64ADA41EB08A}", "Project1", ProjectType.Cs),
            new ProjectItem("{470DA54D-A309-4A6B-8670-33F017213113}", "Project2", ProjectType.Cs),
            new ProjectItem("{C4C66B30-66CD-4D68-9A95-372CA4A13611}", "Project3", ProjectType.Vc),
            new ProjectItem("{C1A3BCC3-F120-471D-A31E-4B835A06F42D}", "Project4", ProjectType.Vc),
        };

        [Fact]
        public void ExtractTest1()
        {
            IEnumerable<SolutionFolder> sf = null;
            var target = (new WNestedProjects(sf)).Extract(null);

            Assert.Equal(String.Empty, target);
        }

        [Fact]
        public void ExtractTest2()
        {
            ResetData();

            folders[1].header.parent.Value = folders[0];
            folders[2].header.parent.Value = folders[1];
            folders[3].header.parent.Value = folders[0];

            var target = (new WNestedProjects(folders)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_WNestedProjects_Test2, target);
        }

        [Fact]
        public void ExtractTest3()
        {
            ResetData();

            projects[0].parent.Value = folders[2];
            projects[2].parent.Value = folders[3];
            projects[3].parent.Value = folders[1];

            var target = (new WNestedProjects(projects)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_WNestedProjects_Test3, target);
        }

        [Fact]
        public void ExtractTest4()
        {
            ResetData();

            folders[1].header.parent.Value = folders[0];
            folders[2].header.parent.Value = folders[1];
            folders[3].header.parent.Value = folders[0];

            projects[0].parent.Value = folders[2];
            projects[2].parent.Value = folders[3];
            projects[3].parent.Value = folders[1];

            var target = (new WNestedProjects(folders, projects)).Extract(null);

            Assert.Equal(SlnSamplesResource.Section_WNestedProjects_Test4, target);
        }

        protected void ResetData()
        {
            folders.ForEach(f => f.header.parent.Value = null);
            projects.ForEach(p => p.parent.Value = null);
        }
    }
}
