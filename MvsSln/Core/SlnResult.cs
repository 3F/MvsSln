/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    public sealed class SlnResult: ISlnResult, ISlnResultSvc
    {
        /// <summary>
        /// Full path to root solution directory.
        /// </summary>
        public string SolutionDir
        {
            get;
            set;
        }

        /// <summary>
        /// Full path to an solution file.
        /// </summary>
        public string SolutionFile
        {
            get => Properties?.GetOrDefault(PropertyNames.SLN_PATH);
        }

        /// <summary>
        /// Processed type for result.
        /// </summary>
        public SlnItems ResultType
        {
            get;
            set;
        }

        /// <summary>
        /// Header information.
        /// </summary>
        public SlnHeader Header
        {
            get;
            private set;
        }

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatform> SolutionConfigs
        {
            get => SolutionConfigList;
        }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatformPrj> ProjectConfigs
        {
            get => ProjectConfigList;
        }

        /// <summary>
        /// Alias of the relation of solution configuration to project configurations.
        /// </summary>
        public RoProperties<IConfPlatform, IConfPlatformPrj[]> ProjectConfigurationPlatforms
        {
            get;
            set;
        }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        public IEnumerable<ProjectItem> ProjectItems
        {
            get => ProjectItemList;
        }

        /// <summary>
        /// Alias for ProjectItems and its configurations.
        /// </summary>
        public IEnumerable<ProjectItemCfg> ProjectItemsConfigs
        {
            get;
            set;
        }

        /// <summary>
        /// List of solution folders.
        /// </summary>
        public IEnumerable<SolutionFolder> SolutionFolders
        {
            get => SolutionFolderList;
        }

        /// <summary>
        /// Default Configuration and Platform for current solution.
        /// </summary>
        public IConfPlatform DefaultConfig
        {
            get;
            set;
        }

        /// <summary>
        /// All available global properties for solution.
        /// Use optional {PropertyNames} to access to popular properties.
        /// </summary>
        public RoProperties Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Solution Project Dependencies.
        /// See also `ProjectReferences` class if you need additional work with project references.
        /// </summary>
        public ISlnPDManager ProjectDependencies
        {
            get;
            set;
        }

        /// <summary>
        /// Optional Key[-Value] records like `SolutionGuid` and so on
        /// that can be presented inside an ExtensibilityGlobals section.
        /// 
        /// ie. Flags/Key-only records are possible too (values will contain null).
        /// </summary>
        public IDictionary<string, string> ExtItems
        {
            get;
            set;
        }

        /// <summary>
        /// Environment for current data.
        /// </summary>
        public IEnvironment Env
        {
            get;
            set;
        }

        /// <summary>
        /// Contains map of all found (known/unknown) solution data.
        /// This value is never null.
        /// </summary>
        public IList<ISection> Map { get; private set; } = new List<ISection>();

        public IEnumerable<PackagesConfig> PackagesConfigs { get; set; }

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        public IList<IConfPlatform> SolutionConfigList
        {
            get;
            set;
        }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public IList<IConfPlatformPrj> ProjectConfigList
        {
            get;
            set;
        }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        public IList<ProjectItem> ProjectItemList
        {
            get;
            set;
        }

        /// <summary>
        /// List of solution folders.
        /// </summary>
        public IList<SolutionFolder> SolutionFolderList
        {
            get;
            set;
        }

        /// <summary>
        /// Updates instance of the Solution Project Dependencies.
        /// </summary>
        /// <param name="dep"></param>
        public void SetProjectDependencies(ISlnPDManager dep)
        {
            ProjectDependencies = dep;
        }

        /// <summary>
        /// Updates header info.
        /// </summary>
        /// <param name="info"></param>
        public void SetHeader(SlnHeader info)
        {
            Header = info;
        }
    }
}