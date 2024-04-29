/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnResult: ISlnWhData
    {
        /// <summary>
        /// Full path to root solution directory.
        /// </summary>
        string SolutionDir { get; }

        /// <summary>
        /// Full path to an solution file.
        /// </summary>
        string SolutionFile { get; }

        /// <summary>
        /// Processed type for result.
        /// </summary>
        SlnItems ResultType { get; }

        /// <summary>
        /// Alias of the relation of solution configuration to project configurations.
        /// </summary>
        RoProperties<IConfPlatform, IConfPlatformPrj[]> ProjectConfigurationPlatforms { get; }

        /// <summary>
        /// Alias for ProjectItems and its configurations.
        /// </summary>
        IEnumerable<ProjectItemCfg> ProjectItemsConfigs { get; }

        /// <summary>
        /// Default Configuration and Platform for current solution.
        /// </summary>
        IConfPlatform DefaultConfig { get; }

        /// <summary>
        /// All available global properties for solution.
        /// Use optional {PropertyNames} to access to popular properties.
        /// </summary>
        RoProperties Properties { get; }

        /// <summary>
        /// Environment for current data.
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Contains map of all found (known/unknown) solution data.
        /// This value is never null.
        /// </summary>
        SMap Map { get; }

        /// <summary>
        /// According to <see cref="SlnItems.PackagesConfig"/> related flags, 
        /// all found and loaded packages.config.
        /// </summary>
        IEnumerable<PackagesConfig> PackagesConfigs { get; }
    }
}