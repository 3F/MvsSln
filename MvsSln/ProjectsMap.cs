/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.SBEScripts.Components.Build
{
    /// <summary>
    /// Detects the first / last project of build-order in solution of Visual Studio.
    /// Based on https://gist.github.com/3F/a77129e3978841241927
    /// And represents final box-solution from Sample 1 - http://vssbe.r-eg.net/doc/Examples/Demo/#sample-1
    /// </summary>
    public class ProjectsMap
    {
        /// <summary>
        /// Guid of Solution Folder.
        /// </summary>
        public const string GUID_SLN_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        /// <summary>
        /// Map of projects in direct order.
        /// </summary>
        protected List<string> order = new List<string>();

        /// <summary>
        /// Map of projects by Guid.
        /// </summary>
        protected Dictionary<string, Project> projects = new Dictionary<string, Project>();

        /// <summary>
        /// Pattern of 'Project(' line - based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProject = new Regex("^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$");

        /// <summary>
        /// Pattern of 'ProjectSection(ProjectDependencies)' lines - based on crackPropertyLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProperty = new Regex("^(?<PName>[^=]*)\\s*=\\s*(?<PValue>[^=]*)$");

        /// <summary>
        /// Container of project data.
        /// </summary>
        public struct Project
        {
            /// <summary>
            /// The name of project.
            /// </summary>
            public string name;

            /// <summary>
            /// Path to project.
            /// </summary>
            public string path;

            /// <summary>
            /// Type of project.
            /// </summary>
            public string type;

            /// <summary>
            /// Guid of project.
            /// </summary>
            public string guid;
        }

        /// <summary>
        /// Get list of project Guids.
        /// In direct order of definition.
        /// </summary>
        public List<string> GuidList
        {
            get {
                return order;
            }
        }

        /// <summary>
        /// Get first project from defined list.
        /// Ignores used Build type.
        /// </summary>
        public Project First
        {
            get
            {
                if(order.Count < 1) {
                    return new Project() { name = "The First project is Undefined", path = "?" };
                }
                return projects[order[0]];
            }
        }

        /// <summary>
        /// Get last project from defined list.
        /// Ignores used Build type.
        /// </summary>
        public Project Last
        {
            get
            {
                if(order.Count < 1) {
                    return new Project() { name = "The Last project is Undefined", path = "?" };
                }
                return projects[order[order.Count - 1]];
            }
        }

        /// <summary>
        /// Get first project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Project FirstBy(BuildType type)
        {
            switch(type)
            {
                case BuildType.Clean:
                case BuildType.CleanCtx:
                case BuildType.CleanOnlyProject:
                case BuildType.CleanSelection: {
                    return Last; // it should be in reverse order for 'Clean' types
                }
            }
            return First;
        }

        /// <summary>
        /// Get last project in Project Build Order.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Project LastBy(BuildType type)
        {
            switch(type)
            {
                case BuildType.Clean:
                case BuildType.CleanCtx:
                case BuildType.CleanOnlyProject:
                case BuildType.CleanSelection: {
                    return First; // it should be in reverse order for 'Clean' types
                }
            }
            return Last;
        }

        /// <summary>
        /// Get project by Guid string.
        /// </summary>
        /// <param name="guid">Identifier of project.</param>
        /// <returns></returns>
        public Project getProjectBy(string guid)
        {
            return projects[guid];
        }

        /// <summary>
        /// Detect projects from solution file.
        /// </summary>
        /// <param name="sln">Full path to solution</param>
        /// <param name="flush">Resets prev. data if true.</param>
        public void detect(string sln, bool flush = false)
        {
            if(flush) {
                projects.Clear();
                order.Clear();
            }

            Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
            using(StreamReader reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    extract(reader, line.Trim(), ref map);
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
        public ProjectsMap(string sln)
        {
            detect(sln);
        }

        /// <summary>
        /// Only to initialize analyzer.
        /// </summary>
        public ProjectsMap()
        {

        }

        /// <param name="reader">Used reader.</param>
        /// <param name="line">Current line.</param>
        /// <param name="map">Container of projects.</param>
        protected void extract(StreamReader reader, string line, ref Dictionary<string, List<string>> map)
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

            projects[pGuid] = new Project()
            { 
                name = m.Groups["Name"].Value,
                path = m.Groups["Path"].Value,
                type = m.Groups["TypeGuid"].Value,
                guid = pGuid
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