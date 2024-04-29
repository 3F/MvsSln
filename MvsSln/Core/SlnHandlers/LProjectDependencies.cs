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
    using static net.r_eg.MvsSln.Core.Keywords;

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
        protected IList<string> order = [];

        /// <summary>
        /// Collected map of projects.
        /// </summary>
        protected IDictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>();

        public IList<string> GuidList => order;

        public IDictionary<string, HashSet<string>> Dependencies => map;

        public IDictionary<string, ProjectItem> Projects { get; protected set; } = new Dictionary<string, ProjectItem>();

        public ProjectItem FirstProject => (order.Count < 1) ? default : GetProjectBy(order[0]);

        public ProjectItem LastProject => (order.Count < 1) ? default : GetProjectBy(order[order.Count - 1]);

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

        public ProjectItem GetProjectBy(string guid)
        {
            if(string.IsNullOrWhiteSpace(guid) || !Projects.ContainsKey(guid)) {
                return default;
            }
            return Projects[guid];
        }

        public override LineAct LineControl => LineAct.Ignore;

        public override bool IsActivated(ISvc svc)
        {
            return (svc.Sln.ResultType & SlnItems.ProjectDependencies) == SlnItems.ProjectDependencies;
        }

        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith(Project_, StringComparison.Ordinal);
        }

        public override void PreProcessing(ISvc svc)
        {
            Projects.Clear();
            order.Clear();
            map.Clear();
        }

        public override bool Positioned(ISvc svc, RawText line)
        {
            ProjectItem pItem = GetProjectItem(line.trimmed, svc.Sln.SolutionDir);
            if(pItem.pGuid == null) return false;

            Projects[pItem.pGuid]   = pItem;
            map[pItem.pGuid]        = [];

            while((line = svc.ReadLine(this)) != null && (line != EndProject))
            {
                if(!line.trimmed.StartsWith(ProjectDependencies, StringComparison.Ordinal))
                {
                    continue;
                }

                for(line = svc.ReadLine(this); line != null; line = svc.ReadLine(this))
                {
                    if(line.trimmed.StartsWith(EndProjectSection, StringComparison.Ordinal))
                    {
                        break;
                    }

                    map[pItem.pGuid].Add
                    (
                        FormatGuid(RPatterns.PropertyLine.Match(line).Groups["PName"].Value)
                    );
                }
            }

            svc.Sln.SetProjectDependencies(this);
            return true;
        }

        public override void PostProcessing(ISvc svc)
        {
            try
            {
                BuildOrder();
            }
            catch(KeyNotFoundException)
            {
                LSender.Send
                (
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
                if(!order.Contains(id)) order.Add(id);
                return true;
            };

            foreach(KeyValuePair<string, HashSet<string>> project in map)
            {
                h(project.Key);
                if(!order.Contains(project.Key)) order.Add(project.Key);
            }
        }

        protected string FormatGuid(string guid) => guid.ReformatSlnGuid();
    }
}