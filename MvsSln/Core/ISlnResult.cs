/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnResult
    {
        /// <summary>
        /// Full path to root solution directory.
        /// </summary>
        string SolutionDir { get; }

        /// <summary>
        /// Processed type for result.
        /// </summary>
        SlnItems ResultType { get; }

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        IEnumerable<IConfPlatform> SolutionConfigs { get; }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        IEnumerable<IConfPlatformPrj> ProjectConfigs { get; }

        /// <summary>
        /// Alias of the relation of solution configuration to project configurations.
        /// </summary>
        RoProperties<IConfPlatform, IConfPlatformPrj[]> ProjectConfigurationPlatforms { get; }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        IEnumerable<ProjectItem> ProjectItems { get; }

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
        /// </summary>
        RoProperties Properties { get; }

        /// <summary>
        /// Solution Project Dependencies.
        /// </summary>
        ISlnProjectDependencies ProjectDependencies { get; }

        /// <summary>
        /// Environment for current data.
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Contains map of all found (known/unknown) solution data.
        /// This value is never null.
        /// </summary>
        List<ISection> Map { get; }
    }
}