/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    /// <summary>
    /// Project Build Order from .sln file.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class LProjectDependencies: LAbstract, ISlnHandler, ISlnPDManager
    {
        /// <summary>
        /// Direct order of identifiers.
        /// </summary>
        protected IList<string> order = new List<string>();

        /// <summary>
        /// Map of projects.
        /// </summary>
        protected IDictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// List of project Guids.
        /// In direct order of definitions with considering of ProjectDependencies.
        /// </summary>
        public IList<string> GuidList => order;

        /// <summary>
        /// Projects and their dependencies.
        /// </summary>
        public IDictionary<string, HashSet<string>> Dependencies => map;

        /// <summary>
        /// List of projects by Guid.
        /// </summary>
        public IDictionary<string, ProjectItem> Projects
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
                    return default(ProjectItem);
                }
                return GetProjectBy(order[0]);
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
                    return default(ProjectItem);
                }
                return GetProjectBy(order[order.Count - 1]);
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
            if(String.IsNullOrWhiteSpace(guid) || !Projects.ContainsKey(guid)) {
                return default(ProjectItem);
            }
            return Projects[guid];
        }

        /// <summary>
        /// Action with incoming line.
        /// </summary>
        public override LineAct LineControl
        {
            get => LineAct.Ignore;
        }

        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.ProjectDependencies) == SlnItems.ProjectDependencies);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("Project(", StringComparison.Ordinal);
        }

        /// <summary>
        /// The logic before processing file.
        /// </summary>
        /// <param name="svc"></param>
        public override void PreProcessing(ISvc svc)
        {
            Projects.Clear();
            order.Clear();
            map.Clear();
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            var pItem = GetProjectItem(line.trimmed, svc.Sln.SolutionDir);
            if(pItem.pGuid == null) {
                return false;
            }

            Projects[pItem.pGuid]   = pItem;
            map[pItem.pGuid]        = new HashSet<string>();

            while((line = svc.ReadLine(this)) != null && (line != "EndProject"))
            {
                if(!line.trimmed.StartsWith("ProjectSection(ProjectDependencies)", StringComparison.Ordinal)) {
                    continue;
                }

                for(line = svc.ReadLine(this); line != null; line = svc.ReadLine(this))
                {
                    if(line.trimmed.StartsWith("EndProjectSection", StringComparison.Ordinal)) {
                        break;
                    }

                    map[pItem.pGuid].Add(
                        FormatGuid(RPatterns.PropertyLine.Match(line).Groups["PName"].Value)
                    );
                }
            }

            svc.Sln.SetProjectDependencies(this);
            return true;
        }

        /// <summary>
        /// The logic after processing file.
        /// </summary>
        /// <param name="svc"></param>
        public override void PostProcessing(ISvc svc)
        {
            try {
                BuildOrder();
            }
            catch(KeyNotFoundException) {
                LSender.Send(
                    this, 
                    "We can't build dependencies, some of this is incorrect. Please check data.", 
                    Message.Level.Warn
                );
            }
        }

        protected void BuildOrder()
        {
            bool h(string id)
            {
                map[id].ForEach(dep => h(dep));
                if(!order.Contains(id)) {
                    order.Add(id);
                }
                return true;
            };

            foreach(KeyValuePair<string, HashSet<string>> project in map)
            {
                h(project.Key);
                if(!order.Contains(project.Key)) {
                    order.Add(project.Key);
                }
            }
        }

        protected string FormatGuid(string guid) => guid.ReformatSlnGuid();
    }
}