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
        /// All supported data.
        /// </summary>
        All = Projects 
                | Header 
                | SolutionConfPlatforms 
                | ProjectConfPlatforms 
                | ProjectDependencies
                | SolutionItems
                | ExtItems
                | Env
                | LoadDefaultData
                | Map
                | ProjectDependenciesXml,

        /// <summary>
        /// All found projects from solution.
        /// </summary>
        Projects = 0x0001,

        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        SolutionConfPlatforms = 0x0002,

        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        ProjectConfPlatforms = 0x0004,

        /// <summary>
        /// Project Build Order from .sln file.
        /// </summary>
        ProjectDependencies = 0x0008,

        /// <summary>
        /// To prepare environment without loading projects.
        /// </summary>
        Env = 0x0010 | Projects | SolutionConfPlatforms | ProjectConfPlatforms,

        /// <summary>
        /// To load all possible default data.
        /// </summary>
        LoadDefaultData = 0x0020,

        /// <summary>
        /// To load only minimal default data.
        /// For example, the only one configuration for each project.
        /// </summary>
        LoadMinimalDefaultData = 0x0040,

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
        Map = 0x0080,

        /// <summary>
        /// ProjectSection - SolutionItems
        /// +NestedProjects dependencies
        /// </summary>
        SolutionItems = 0x0100,

        /// <summary>
        /// Header information.
        /// </summary>
        Header = 0x0200,

        /// <summary>
        /// Includes ExtensibilityGlobals
        /// </summary>
        ExtItems = 0x0400,

        /// <summary>
        /// Covers ProjectDependencies (SLN) logic using data from project files (XML).
        /// Helps eliminate miscellaneous units between VS and msbuild world:
        /// https://github.com/3F/MvsSln/issues/25#issuecomment-617956253
        /// 
        /// Requires Env with loaded projects (LoadMinimalDefaultData or LoadDefaultData).
        /// </summary>
        ProjectDependenciesXml = 0x0800 | ProjectDependencies | Env,
    }
}
