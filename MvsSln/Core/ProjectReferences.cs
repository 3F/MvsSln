/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2018  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Use it for additional work with project references and it's dependencies in ISlnPDManager manner.
    /// </summary>
    public class ProjectReferences: LProjectDependencies, ISlnPDManager
    {
        /// <summary>
        /// List of ProjectReferences by project Guid.
        /// </summary>
        public IDictionary<string, List<Item>> References
        {
            get;
            protected set;
        }

        /// <summary>
        /// Parent data of the solution ProjectDependencies that initialized this object.
        /// </summary>
        public ISlnPDManager Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Access to XProjects.
        /// </summary>
        public IEnumerable<IXProject> XProjects
        {
            get;
            protected set;
        }

        /// <summary>
        /// Get ProjectReferences by project Guid.
        /// </summary>
        /// <param name="guid">Identifier of project.</param>
        /// <returns>All found ProjectReferences.</returns>
        public IEnumerable<Item> GetReferences(string guid)
        {
            if(String.IsNullOrWhiteSpace(guid) || !References.ContainsKey(guid)) {
                return new List<Item>();
            }
            return References[guid];
        }

        /// <param name="slndep">Parent data.</param>
        /// <param name="xprojects">List of evaluated projects to consider of dependencies.</param>
        public ProjectReferences(ISlnPDManager slndep, IEnumerable<IXProject> xprojects)
        {
            Parent      = slndep;
            XProjects   = xprojects;

            map         = Parent.Dependencies;
            Projects    = Parent.Projects;

            InitReferences();
            InitMap();
        }

        protected void InitReferences()
        {
            References = new Dictionary<string, List<Item>>();
            foreach(var project in XProjects)
            {
                if(!References.ContainsKey(project.ProjectGuid)) {
                    References[project.ProjectGuid] = new List<Item>();
                }
                project.GetProjectReferences().ForEach(i => References[project.ProjectGuid].Add(i));
            }
        }

        protected void InitMap()
        {
            foreach(var r in References) {
                if(map.ContainsKey(r.Key)) {
                    map[r.Key] = new HashSet<string>();
                }
                r.Value.ForEach(i => map[r.Key].Add(FormatGuid(i.meta["Project"].evaluated)));
            }
            BuildOrder();
        }
    }
}