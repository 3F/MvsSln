/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    public interface IConfPlatformPrj: IConfPlatform
    {
        /// <summary>
        /// Project Guid.
        /// </summary>
        string PGuid { get; }

        /// <summary>
        /// Existence of `.Build.0` to activate project for build:
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU
        /// </summary>
        bool IncludeInBuild { get; }

        /// <summary>
        /// Existence of `.Deploy.0` to activate project for deployment:
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Deploy.0 = Release|Any CPU
        /// </summary>
        bool IncludeInDeploy { get; }

        /// <summary>
        /// Link to solution configuration.
        /// </summary>
        IConfPlatform Sln { get; }
    }
}