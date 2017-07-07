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

using System;

namespace net.r_eg.MvsSln
{
    public enum SlnItems: UInt32
    {
        None,

        /// <summary>
        /// All supported data.
        /// </summary>
        All = Projects 
                | SolutionConfPlatforms 
                | ProjectConfPlatforms 
                | ProjectDependencies
                | Env
                | LoadDefaultData,

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
        Env = 0x0010 | Projects | SolutionConfPlatforms,

        /// <summary>
        /// To load default data.
        /// </summary>
        LoadDefaultData = 0x0020,

        /// <summary>
        /// To prepare environment with loaded projects by default.
        /// </summary>
        EnvWithProjects = Env | ProjectConfPlatforms | LoadDefaultData,
    }
}
