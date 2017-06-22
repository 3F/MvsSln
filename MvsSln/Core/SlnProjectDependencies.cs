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
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Project Build Order from .sln file.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// Today it just re-licensed and separated into new project 'as is'. Thus:
    /// TODO: after import - abstraction layer, and scalable items 
    /// </summary>
    public class SlnProjectDependencies: ISlnProjectDependencies
    {
        /// <summary>
        /// Guid of Solution Folder.
        /// </summary>
        public const string GUID_SLN_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        /// <summary>
        /// Direct order of identifiers.
        /// </summary>
        protected List<string> order = new List<string>();

        /// <summary>
        /// Map of projects.
        /// </summary>
        public Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();

        /// <summary>
        /// Pattern of 'Project(' line - based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProject = new Regex("^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$");

        /// <summary>
        /// Pattern of 'ProjectSection(ProjectDependencies)' lines - based on crackPropertyLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProperty = new Regex("^(?<PName>[^=]*)\\s*=\\s*(?<PValue>[^=]*)$");

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
        /// Detect projects from solution file.
        /// </summary>
        /// <param name="sln">Full path to solution</param>
        /// <param name="flush">Resets prev. data if true.</param>
        public void Detect(string sln, bool flush = false)
        {
            if(flush) {
                Projects.Clear();
                order.Clear();
            }

            map.Clear();
            using(StreamReader reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    Extract(reader, line.Trim(), ref map);
                }
            }

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

        /// <param name="sln">Full path to solution</param>
        public SlnProjectDependencies(string sln)
        {
            Detect(sln);
        }

        /// <summary>
        /// To initialize analyzer only.
        /// </summary>
        public SlnProjectDependencies()
        {

        }

        /// <param name="reader">Used reader.</param>
        /// <param name="line">Current line.</param>
        /// <param name="map">Container of projects.</param>
        protected void Extract(StreamReader reader, string line, ref Dictionary<string, List<string>> map)
        {
            if(!line.StartsWith("Project(", StringComparison.Ordinal)) {
                return;
            }

            Match m = rProject.Match(line);
            if(!m.Success) {
                throw new Exception("incorrect line");
            }

            if(String.Equals(GUID_SLN_FOLDER, m.Groups["TypeGuid"].Value.Trim(), StringComparison.OrdinalIgnoreCase)) {
                return; //SolutionFolder
            }

            string pGuid    = m.Groups["Guid"].Value.Trim();
            map[pGuid]      = new List<string>();

            Projects[pGuid] = new ProjectItem()
            { 
                name    = m.Groups["Name"].Value,
                path    = m.Groups["Path"].Value,
                type    = m.Groups["TypeGuid"].Value,
                pGuid   = pGuid
            };

            while((line = reader.ReadLine()) != null && (line != "EndProject"))
            {
                line = line.Trim();
                if(!line.StartsWith("ProjectSection(ProjectDependencies)", StringComparison.Ordinal)) {
                    continue;
                }

                for(line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
                    line = line.Trim();
                    if(line.StartsWith("EndProjectSection", StringComparison.Ordinal)) {
                        break;
                    }
                    map[pGuid].Add(rProperty.Match(line).Groups["PName"].Value.Trim());
                }
            }
        }
    }
}