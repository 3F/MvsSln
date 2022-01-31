/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnResultSvc: ISlnResult
    {
        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        IList<IConfPlatform> SolutionConfigList { get; set; }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        IList<IConfPlatformPrj> ProjectConfigList { get; set; }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        IList<ProjectItem> ProjectItemList { get; set; }

        /// <summary>
        /// List of solution folders.
        /// </summary>
        IList<SolutionFolder> SolutionFolderList { get; set; }

        /// <summary>
        /// Updates instance of the Solution Project Dependencies.
        /// </summary>
        /// <param name="dep"></param>
        void SetProjectDependencies(ISlnPDManager dep);

        /// <summary>
        /// Updates header info.
        /// </summary>
        /// <param name="info"></param>
        void SetHeader(SlnHeader info);
    }
}