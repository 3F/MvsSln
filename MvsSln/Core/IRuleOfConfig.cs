/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    public interface IRuleOfConfig
    {
        /// <summary>
        /// Rules of platform names.
        /// details: https://github.com/3F/vsSolutionBuildEvent/issues/14
        ///        + MS Connect Issue #503935
        /// </summary>
        /// <param name="name">Platform name.</param>
        /// <returns></returns>
        string Platform(string name);

        /// <summary>
        /// Rules of configuration names.
        /// </summary>
        /// <param name="name">Configuration name.</param>
        /// <returns></returns>
        string Configuration(string name);
    }
}
