/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnPDManager: ISlnProjectDependencies
    {
        /// <summary>
        /// Get first project from defined list.
        /// </summary>
        ProjectItem FirstProject { get; }

        /// <summary>
        /// Get last project from defined list.
        /// </summary>
        ProjectItem LastProject { get; }

        /// <summary>
        /// Get first project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ProjectItem FirstBy(BuildType type);

        /// <summary>
        /// Get last project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ProjectItem LastBy(BuildType type);

        /// <summary>
        /// Get project by Guid string.
        /// </summary>
        /// <param name="guid">Identifier of project.</param>
        /// <returns></returns>
        ProjectItem GetProjectBy(string guid);
    }
}