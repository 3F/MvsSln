/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnWhData
    {
        /// <summary>
        /// Header information.
        /// </summary>
        SlnHeader Header { get; }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        IEnumerable<ProjectItem> ProjectItems { get; }

        /// <summary>
        /// Solution Project Dependencies.
        /// </summary>
        ISlnPDManager ProjectDependencies { get; }

        /// <summary>
        /// List of solution folders.
        /// </summary>
        IEnumerable<SolutionFolder> SolutionFolders { get; }

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        IEnumerable<IConfPlatform> SolutionConfigs { get; }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        IEnumerable<IConfPlatformPrj> ProjectConfigs { get; }

        /// <summary>
        /// Optional Key[-Value] records like `SolutionGuid` and so on
        /// that can be presented inside an ExtensibilityGlobals section.
        /// 
        /// ie. Flags/Key-only records are possible too (values will contain null).
        /// </summary>
        IDictionary<string, string> ExtItems { get; set; }
    }
}