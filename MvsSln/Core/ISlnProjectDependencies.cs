/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnProjectDependencies
    {
        /// <summary>
        /// List of project Guids.
        /// In direct order of definitions with considering of ProjectDependencies.
        /// </summary>
        IList<string> GuidList { get; }

        /// <summary>
        /// List of projects by Guid.
        /// </summary>
        IDictionary<string, ProjectItem> Projects { get; }

        /// <summary>
        /// Projects and their dependencies.
        /// </summary>
        IDictionary<string, HashSet<string>> Dependencies { get; }
    }
}