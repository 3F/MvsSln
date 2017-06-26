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
using System.IO;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    /// <summary>
    /// Project Build Order from .sln file.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class LProjectDependencies: LAbstract, ISlnHandler, ISlnProjectDependencies
    {
        /// <summary>
        /// Direct order of identifiers.
        /// </summary>
        protected List<string> order = new List<string>();

        /// <summary>
        /// Map of projects.
        /// </summary>
        protected Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();

        /// <summary>
        /// List of project Guids.
        /// In direct order of definitions with considering of ProjectDependencies.
        /// </summary>
        public List<string> GuidList
        {
            get {
                return order;
            }
        }

        /// <summary>
        /// Projects and their dependencies.
        /// </summary>
        public Dictionary<string, List<string>> ProjectDependencies
        {
            get {
                return map;
            }
        }

        /// <summary>
        /// List of projects by Guid.
        /// </summary>
        public Dictionary<string, ProjectItem> Projects
        {
            get;
            protected set;
        } = new Dictionary<string, ProjectItem>();

        /// <summary>
        /// Get first project from defined list.
        /// </summary>
        public ProjectItem FirstProject
        {
            get
            {
                if(order.Count < 1) {
                    return new ProjectItem() { name = "The First project is Undefined", path = "?" };
                }
                return Projects[order[0]];
            }
        }

        /// <summary>
        /// Get last project from defined list.
        /// </summary>
        public ProjectItem LastProject
        {
            get
            {
                if(order.Count < 1) {
                    return new ProjectItem() { name = "The Last project is Undefined", path = "?" };
                }
                return Projects[order[order.Count - 1]];
            }
        }

        /// <summary>
        /// Get first project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ProjectItem FirstBy(BuildType type)
        {
            switch(type)
            {
                case BuildType.Clean:
                case BuildType.CleanCtx:
                case BuildType.CleanOnlyProject:
                case BuildType.CleanSelection: {
                    return LastProject; // it should be in reverse order for 'Clean' types
                }
            }
            return FirstProject;
        }

        /// <summary>
        /// Get last project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ProjectItem LastBy(BuildType type)
        {
            switch(type)
            {
                case BuildType.Clean:
                case BuildType.CleanCtx:
                case BuildType.CleanOnlyProject:
                case BuildType.CleanSelection: {
                    return FirstProject; // it should be in reverse order for 'Clean' types
                }
            }
            return LastProject;
        }

        /// <summary>
        /// Get project by Guid string.
        /// </summary>
        /// <param name="guid">Identifier of project.</param>
        /// <returns></returns>
        public ProjectItem GetProjectBy(string guid)
        {
            return Projects[guid];
        }

        /// <summary>
        /// The logic before processing file.
        /// </summary>
        /// <param name="stream">Used stream.</param>
        /// <param name="rsln">Handled solution data.</param>
        public override void PreProcessing(StreamReader stream, SlnResult rsln)
        {
            //if(flush) {
            //    Projects.Clear();
            //    order.Clear();
            //}

            map.Clear();
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="stream">Used stream.</param>
        /// <param name="line">Received line.</param>
        /// <param name="rsln">Handled solution data.</param>
        public override void Positioned(StreamReader stream, string line, SlnResult rsln)
        {
            if((rsln.type & SlnItems.ProjectDependencies) == 0) {
                return;
            }

            if(!line.StartsWith("Project(", StringComparison.Ordinal)) {
                return;
            }

            var pItem = new ProjectItem(line, rsln.solutionDir);
            if(pItem.pGuid == null) {
                //throw new Exception();
                LSender.Send(this, $"The Guid is null empty for line :: '{line}'", Message.Level.Error);
                return;
            }

            if(String.Equals(Guids.SLN_FOLDER, pItem.pType, StringComparison.OrdinalIgnoreCase)) {
                LSender.Send(this, $"{pItem.name} has been ignored as solution-folder :: '{line}'", Message.Level.Debug);
                return;
            }

            Projects[pItem.pGuid]   = pItem;
            map[pItem.pGuid]        = new List<string>();

            while((line = stream.ReadLine()) != null && (line != "EndProject"))
            {
                line = line.Trim();
                if(!line.StartsWith("ProjectSection(ProjectDependencies)", StringComparison.Ordinal)) {
                    continue;
                }

                for(line = stream.ReadLine(); line != null; line = stream.ReadLine())
                {
                    line = line.Trim();
                    if(line.StartsWith("EndProjectSection", StringComparison.Ordinal)) {
                        break;
                    }
                    map[pItem.pGuid].Add(RPatterns.PropertyLine.Match(line).Groups["PName"].Value.Trim());
                }
            }

            rsln.projectDependencies = this;
        }

        /// <summary>
        /// The logic after processing file.
        /// </summary>
        /// <param name="stream">Used stream.</param>
        /// <param name="rsln">Handled solution data.</param>
        public override void PostProcessing(StreamReader stream, SlnResult rsln)
        {
            // Build order

            Func<string, bool> h = null;
            h = delegate(string id)
            {
                map[id].ForEach(dep => h(dep));
                if(!order.Contains(id)) {
                    order.Add(id);
                }
                return true;
            };

            foreach(KeyValuePair<string, List<string>> project in map)
            {
                h(project.Key);
                if(!order.Contains(project.Key)) {
                    order.Add(project.Key);
                }
            }
        }
    }
}