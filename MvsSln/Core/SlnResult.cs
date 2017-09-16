﻿/*
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
    public sealed class SlnResult: ISlnResult, ISlnResultSvc
    {
        /// <summary>
        /// Full path to root solution directory.
        /// </summary>
        public string SolutionDir
        {
            get;
            set;
        }

        /// <summary>
        /// Processed type for result.
        /// </summary>
        public SlnItems ResultType
        {
            get;
            set;
        }

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatform> SolutionConfigs
        {
            get => SolutionConfigList;
        }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatformPrj> ProjectConfigs
        {
            get => ProjectConfigList;
        }

        /// <summary>
        /// Alias of the relation of solution configuration to project configurations.
        /// </summary>
        public RoProperties<IConfPlatform, IConfPlatformPrj[]> ProjectConfigurationPlatforms
        {
            get;
            set;
        }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        public IEnumerable<ProjectItem> ProjectItems
        {
            get => ProjectItemList;
        }

        /// <summary>
        /// Alias for ProjectItems and its configurations.
        /// </summary>
        public IEnumerable<ProjectItemCfg> ProjectItemsConfigs
        {
            get;
            set;
        }

        /// <summary>
        /// Default Configuration and Platform for current solution.
        /// </summary>
        public IConfPlatform DefaultConfig
        {
            get;
            set;
        }

        /// <summary>
        /// All available global properties for solution.
        /// </summary>
        public RoProperties Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Solution Project Dependencies.
        /// See also `ProjectReferences` class if you need additional work with project references.
        /// </summary>
        public ISlnProjectDependencies ProjectDependencies
        {
            get;
            set;
        }

        /// <summary>
        /// Environment for current data.
        /// </summary>
        public IEnvironment Env
        {
            get;
            set;
        }

        /// <summary>
        /// Contains map of all found (known/unknown) solution data.
        /// This value is never null.
        /// </summary>
        public List<ISection> Map
        {
            get;
            private set;
        } = new List<ISection>();

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        public List<IConfPlatform> SolutionConfigList
        {
            get;
            set;
        }

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public List<IConfPlatformPrj> ProjectConfigList
        {
            get;
            set;
        }

        /// <summary>
        /// All found projects in solution.
        /// </summary>
        public List<ProjectItem> ProjectItemList
        {
            get;
            set;
        }

        /// <summary>
        /// Updates instance of the Solution Project Dependencies.
        /// </summary>
        /// <param name="dep"></param>
        public void SetProjectDependencies(ISlnProjectDependencies dep)
        {
            ProjectDependencies = dep;
        }
    }
}