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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;

namespace net.r_eg.MvsSln.Core
{
    [Guid("818233C6-5BFE-47D5-929D-668C70EA25D5")]
    public interface IEnvironment: IDisposable
    {
        /// <summary>
        /// List of evaluated projects.
        /// </summary>
        IEnumerable<IXProject> Projects { get; }

        /// <summary>
        /// Access to GlobalProjectCollection
        /// </summary>
        ProjectCollection PrjCollection { get; }

        /// <summary>
        /// Find project by Guid.
        /// </summary>
        /// <param name="guid">Guid of project.</param>
        /// <param name="cfg">Specific configuration.</param>
        /// <returns></returns>
        IXProject XProjectByGuid(string guid, IConfPlatform cfg);

        /// <summary>
        /// Find project by Guid.
        /// </summary>
        /// <param name="guid">Guid of project.</param>
        /// <returns></returns>
        IXProject[] XProjectsByGuid(string guid);

        /// <summary>
        /// Find projects by name.
        /// </summary>
        /// <param name="name">ProjectName.</param>
        /// <param name="cfg">Specific configuration.</param>
        /// <returns></returns>
        IXProject[] XProjectsByName(string name, IConfPlatform cfg);

        /// <summary>
        /// Find projects by name.
        /// </summary>
        /// <param name="name">ProjectName.</param>
        /// <returns></returns>
        IXProject[] XProjectsByName(string name);

        /// <summary>
        /// Get or firstly load into collection the project. 
        /// Use default configuration.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <returns></returns>
        Project GetOrLoadProject(ProjectItem pItem);

        /// <summary>
        /// Get or firstly load into collection the project.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <param name="conf">Configuration of project to load.</param>
        /// <returns></returns>
        Project GetOrLoadProject(ProjectItem pItem, IConfPlatform conf);

        /// <summary>
        /// Get or firstly load into collection the project.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Project GetOrLoadProject(ProjectItem pItem, IDictionary<string, string> properties);

        /// <summary>
        /// Get project properties from solution properties.
        /// </summary>
        /// <param name="pItem"></param>
        /// <param name="slnProps">Solution properties.</param>
        /// <returns></returns>
        IDictionary<string, string> GetProjectProperties(ProjectItem pItem, IDictionary<string, string> slnProps);

        /// <summary>
        /// Load available projects via configurations.
        /// </summary>
        /// <param name="pItems">Specific list or null value to load all available.</param>
        void LoadProjects(IEnumerable<ProjectItemCfg> pItems = null);
    }
}