/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Extensions
{
    public static class XProjectExtension
    {
        /// <summary>
        /// Checking of equality by limited project attributes like full path and its configuration.
        /// IXProject does not override Equals() and GetHashCode() 
        /// And this can help to compare projects by minimal information for Unload() methods etc.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="prj"></param>
        /// <returns></returns>
        public static bool IsLimEqual(this IXProject x, IXProject prj)
        {
            if(x == null) {
                return x == prj;
            }

            return x.ProjectFullPath == prj.ProjectFullPath
                && x.ProjectItem.project == prj.ProjectItem.project
                && (ConfigItem)x.ProjectItem.solutionConfig == (ConfigItem)prj.ProjectItem.solutionConfig
                && (ConfigItem)x.ProjectItem.projectConfig == (ConfigItem)prj.ProjectItem.projectConfig;
        }
    }
}