/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
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

using System;

namespace net.r_eg.MvsSln
{
    [Flags]
    public enum SlnItems: UInt32
    {
        None,

        /// <summary>
        /// All supported sln items with loading only minimal default data for prepared environment.
        /// </summary>
        AllMinimal = AllNoLoad | LoadMinimalDefaultData | PackagesConfigSolution,

        /// <summary>
        /// All supported sln items with loading all possible default data for prepared environment.
        /// </summary>
        All = AllNoLoad | LoadDefaultData | PackagesConfig,

        /// <summary>
        /// All supported sln items.
        /// It does not include loading projects for prepared environment (<see cref="Env"/>).
        /// </summary>
        AllNoLoad = Projects 
                | Header 
                | SolutionConfPlatforms 
                | ProjectConfPlatforms 
                | ProjectDependencies
                | SolutionItems
                | ExtItems
                | Env
                | Map
                | ProjectDependenciesXml,

        /// <summary>
        /// All found projects from solution.
        /// </summary>
        Projects = 0x00001,

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        SolutionConfPlatforms = 0x00002,

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        ProjectConfPlatforms = 0x00004,

        /// <summary>
        /// Project Build Order from .sln file.
        /// </summary>
        ProjectDependencies = 0x00008,

        /// <summary>
        /// To prepare environment without loading projects.
        /// </summary>
        Env = 0x00010 | Projects | SolutionConfPlatforms | ProjectConfPlatforms,

        /// <summary>
        /// To load all possible default data.
        /// </summary>
        LoadDefaultData = 0x00020,

        /// <summary>
        /// To load only minimal default data.
        /// For example, the only one configuration for each project.
        /// </summary>
        LoadMinimalDefaultData = 0x00040,

        /// <summary>
        /// To prepare environment with loaded projects by default.
        /// </summary>
        EnvWithProjects = Env | LoadDefaultData,

        /// <summary>
        /// To prepare environment with minimal loaded projects.
        /// The only one configuration for each project. 
        /// </summary>
        EnvWithMinimalProjects = Env | LoadMinimalDefaultData,

        /// <summary>
        /// Creates map when processing sln data.
        /// </summary>
        Map = 0x00080,

        /// <summary>
        /// ProjectSection - SolutionItems
        /// +NestedProjects dependencies
        /// </summary>
        SolutionItems = 0x00100,

        /// <summary>
        /// Header information.
        /// </summary>
        Header = 0x00200,

        /// <summary>
        /// Includes ExtensibilityGlobals
        /// </summary>
        ExtItems = 0x00400,

        /// <summary>
        /// Covers ProjectDependencies (SLN) logic using data from project files (XML).
        /// Helps eliminate miscellaneous units between VS and msbuild world:
        /// https://github.com/3F/MvsSln/issues/25#issuecomment-617956253
        /// 
        /// Requires Env with loaded projects (LoadMinimalDefaultData or LoadDefaultData).
        /// </summary>
        ProjectDependenciesXml = 0x00800 | ProjectDependencies | Env,

        /// <summary>
        /// Find and Load packages.config files using paths defined in .sln (if defined <see cref="SolutionItems"/>) and solution directory.
        /// </summary>
        PackagesConfigSolution = 0x01000,

        /// <summary>
        /// Find and Load legacy packages.config files from all specified projects directories (activates <see cref="Projects"/>), 
        /// and root `packages` folder if exists.
        /// </summary>
        PackagesConfigLegacy = 0x02000 | Projects,

        /// <summary>
        /// Find and Load packages.config using <see cref="PackagesConfigSolution"/> and <see cref="PackagesConfigLegacy"/> flags.
        /// </summary>
        PackagesConfig = PackagesConfigSolution | PackagesConfigLegacy,
    }
}
