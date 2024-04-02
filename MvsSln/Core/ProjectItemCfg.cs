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
    public readonly struct ProjectItemCfg(ProjectItem item, IConfPlatform sln, IConfPlatformPrj prj)
    {
        public readonly ProjectItem project = item;

        public readonly IConfPlatform solutionConfig = sln;

        public readonly IConfPlatformPrj projectConfig = prj;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DbgDisplay => $"{project.name} ({projectConfig}) [{project.pGuid}]";
    }
}