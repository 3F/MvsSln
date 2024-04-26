/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln
{
    public sealed class LhDataHelper: ISlnWhData
    {
        public SlnHeader Header { get; set; }

        public IEnumerable<ProjectItem> ProjectItems { get; set; }

        public ISlnPDManager ProjectDependencies { get; set; }

        public IEnumerable<SolutionFolder> SolutionFolders { get; set; }

        public IEnumerable<IConfPlatform> SolutionConfigs { get; set; }

        public IEnumerable<IConfPlatformPrj> ProjectConfigs { get; set; }

        public IDictionary<string, string> ExtItems { get; set; }

        public LhDataHelper SetHeader(SlnHeader data)
        {
            Header = data;
            return this;
        }

        public LhDataHelper SetProjects(IEnumerable<ProjectItem> projects)
        {
            ProjectItems = projects;
            return this;
        }

        public LhDataHelper SetDependencies(ISlnPDManager dependencies)
        {
            ProjectDependencies = dependencies;
            return this;
        }

        public LhDataHelper SetFolders(IEnumerable<SolutionFolder> folders)
        {
            SolutionFolders = folders;
            return this;
        }

        public LhDataHelper SetSolutionConfigs(IEnumerable<IConfPlatform> configs)
        {
            SolutionConfigs = configs;
            return this;
        }

        public LhDataHelper SetProjectConfigs(IEnumerable<IConfPlatformPrj> configs)
        {
            ProjectConfigs = configs;
            return this;
        }

        public LhDataHelper SetExt(IDictionary<string, string> items)
        {
            ExtItems = items;
            return this;
        }
    }
}
