/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Aggregates links to ProjectItem and its configurations.
    /// </summary>
    [DebuggerDisplay("{DbgDisplay}")]
    public struct ProjectItemCfg
    {
        public ProjectItem project;

        public IConfPlatform solutionConfig;

        public IConfPlatformPrj projectConfig;

        public ProjectItemCfg(ProjectItem item, IConfPlatform sln, IConfPlatformPrj prj)
        {
            project         = item;
            solutionConfig  = sln;
            projectConfig   = prj;
        }
        
        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{project.name} ({projectConfig}) [{project.pGuid}]";
        }

        #endregion
    }
}