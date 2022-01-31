/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.MvsSln
{
    [Guid("6445BCC6-B722-43C9-A230-003D8B4FFED2")]
    public enum BuildType
    {
        /// <summary>
        /// Common context - any type or type by default
        /// </summary>
        Common = Int32.MaxValue,

        /// <summary>
        /// 'build' action
        /// </summary>
        Build = 100,

        /// <summary>
        /// 'rebuild' action
        /// </summary>
        Rebuild = 101,

        /// <summary>
        /// 'clean' action
        /// </summary>
        Clean = 102,

        /// <summary>
        /// 'build' action for selection
        /// </summary>
        BuildSelection = 200,

        /// <summary>
        /// 'rebuild' action for selection
        /// </summary>
        RebuildSelection = 201,

        /// <summary>
        /// 'clean' action for selection
        /// </summary>
        CleanSelection = 202,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildOnlyProject = 205,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildOnlyProject = 206,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanOnlyProject = 207,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildCtx = 302,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildCtx = 303,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanCtx = 304,
    }
}
