/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Projects
{
    public interface IPackageInfo
    {
        /// <summary>
        /// Package id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Package version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// One-time parsed <see cref="Version"/> on the first access.
        /// </summary>
        Version VersionParsed { get; }

        /// <summary>
        /// Some related package meta information.
        /// Eg.:
        /// * targetFramework="net472"
        /// * output="vsSolutionBuildEvent"
        /// ...
        /// </summary>
        IDictionary<string, string> Meta { get; }

        /// <summary>
        /// Try get "targetFramework" value from <see cref="Meta"/> information.
        /// It must return <see langword="null"/> if attribute is not defined.
        /// </summary>
        /// <remarks>Alias to <see cref="Meta"/> accessing.</remarks>
        string MetaTFM { get; }

        /// <summary>
        /// Try get "output" value from <see cref="Meta"/> information.
        /// It must return <see langword="null"/> if attribute is not defined.
        /// </summary>
        /// <remarks>Alias to <see cref="Meta"/> accessing.</remarks>
        string MetaOutput { get; }

        /// <summary>
        /// Remove current package from storage.
        /// </summary>
        IPackagesConfig Remove();
    }
}