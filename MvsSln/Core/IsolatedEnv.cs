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
using System.Linq;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Exceptions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Isolated environment.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class IsolatedEnv: IEnvironment
    {
        /// <summary>
        /// Default value for all undefined properties.
        /// </summary>
        public const string PROP_VALUE_DEFAULT = "*Undefined*";

        protected const string P_CONFIG     = "Configuration";
        protected const string P_PLATFORM   = "Platform";

        /// <summary>
        /// Solution properties.
        /// </summary>
        protected IDictionary<string, string> slnProperties;

        /// <summary>
        /// Solution data
        /// </summary>
        protected ISlnResult sln;

        /// <summary>
        /// List of evaluated projects.
        /// </summary>
        public IEnumerable<XProject> Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Access to GlobalProjectCollection
        /// </summary>
        public ProjectCollection PrjCollection
        {
            get => ProjectCollection.GlobalProjectCollection;
        }

        /// <summary>
        /// Get or firstly load into collection the project. 
        /// Use default configuration.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem)
        {
            return GetOrLoadProject(pItem, GetProjectProperties(pItem, slnProperties));
        }

        /// <summary>
        /// Get or firstly load into collection the project.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <param name="conf">Configuration of project to load.</param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem, IConfPlatform conf)
        {
            return GetOrLoadProject(pItem, DefProperties(conf, slnProperties));
        }

        /// <summary>
        /// Get or firstly load into collection the project.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem, IDictionary<string, string> properties)
        {
            if(String.IsNullOrWhiteSpace(pItem.pGuid)) {
                throw new ArgumentException($"GUID of project is empty or null ['{pItem.name}', '{pItem.fullPath}']");
            }

            if(!properties.ContainsKey(P_CONFIG) || !properties.ContainsKey(P_PLATFORM)) {
                throw new ArgumentException($"Properties does not contain {P_CONFIG} or {P_PLATFORM} key.");
            }

            foreach(var eProject in PrjCollection.LoadedProjects)
            {
                LSender.Send(this, $"Find in projects collection: `{pItem.pGuid}`", Message.Level.Trace);

                if(GetProjectGuid(eProject) != pItem.pGuid) {
                    continue;
                }

                var eCfg    = new ConfigItem(eProject.GetPropertyValue(P_CONFIG), eProject.GetPropertyValue(P_PLATFORM));
                var propCfg = new ConfigItem(properties[P_CONFIG], properties[P_PLATFORM]);

                LSender.Send(this, $" ? {propCfg} == {eCfg}", Message.Level.Trace);

                if(eCfg == propCfg) {
                    return eProject;
                }
            }

            LSender.Send(this, $"trying to load project :: '{pItem.name}' ('{pItem.fullPath}')", Message.Level.Trace);
            if(String.IsNullOrWhiteSpace(pItem.fullPath)) {
                throw new NotFoundException($"Path is empty to project ['{pItem.name}', '{pItem.pGuid}']");
            }

            return new Project(pItem.fullPath, properties, null, PrjCollection);
        }

        /// <summary>
        /// Get project properties from solution properties.
        /// </summary>
        /// <param name="pItem"></param>
        /// <param name="slnProps">Solution properties.</param>
        /// <returns></returns>
        public IDictionary<string, string> GetProjectProperties(ProjectItem pItem, IDictionary<string, string> slnProps)
        {
            if(!slnProps.ContainsKey(P_CONFIG) || !slnProps.ContainsKey(P_PLATFORM)) {
                throw new ArgumentException("Solution Configuration or Platform is not defined in used properties.");
            }

            var cfg = sln
                        .ProjectConfigs
                        .Where(c => 
                            c.PGuid == pItem.pGuid 
                                && c.Sln.Configuration == slnProps[P_CONFIG]
                                && c.Sln.Platform == slnProps[P_PLATFORM]
                        )
                        .FirstOrDefault();

            if(cfg.Configuration == null || cfg.Platform == null)
            {
                LSender.Send(
                    this, 
                    String.Format(
                        "Something went wrong with project configuration `{0}|{1}` <- sln [{2}|{3}]",
                        cfg.Configuration,
                        cfg.Platform,
                        slnProps[P_CONFIG],
                        slnProps[P_PLATFORM]
                    ), 
                    Message.Level.Warn
                );
                return slnProps;
            }

            string platform = PlatformName(cfg.Platform);
            LSender.Send(this, $"-> prj['{cfg.Configuration}'; '{platform}']", Message.Level.Debug);

            var ret = new Dictionary<string, string>(slnProps);

            ret[P_CONFIG]   = cfg.Configuration;
            ret[P_PLATFORM] = platform;

            return ret;
        }

        public IsolatedEnv(ISlnResult data)
        {
            sln = data;
            slnProperties = DefProperties(
                sln.DefaultConfig,
                sln.Properties.ExtractDictionary
            );

            foreach(var property in slnProperties) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(property.Key, property.Value);
            }

            Projects = Load();
        }

        protected IEnumerable<XProject> Load()
        {
            var xprojects = new List<XProject>();
            foreach(var pItem in sln.ProjectItemsConfigs)
            {
                Project eProject = GetOrLoadProject(pItem.project, pItem.projectConfig);
                xprojects.Add(new XProject(pItem, eProject));
            }
            return xprojects;
        }

        protected virtual string GetProjectGuid(Project eProject)
        {
            return eProject.GetPropertyValue("ProjectGuid");
        }

        /// <summary>
        /// Defines required properties for project via IConfPlatform.
        /// </summary>
        /// <param name="conf">Specific configuration.</param>
        /// <param name="properties">Common properties.</param>
        /// <returns></returns>
        protected IDictionary<string, string> DefProperties(IConfPlatform conf, IDictionary<string, string> properties)
        {
            var ret = new Dictionary<string, string>(properties);

            void _set(string k, string v)
            {
                if(v != null) {
                    ret[k] = v;
                }

                if(!ret.ContainsKey(k)) {
                    ret[k] = PROP_VALUE_DEFAULT;
                }
            };

            _set(P_CONFIG, conf?.Configuration);
            _set(P_PLATFORM, PlatformName(conf?.Platform));

            return ret;
        }

        /// <summary>
        /// Rules of platform names, for example: 'Any CPU' to 'AnyCPU'
        /// see MS Connect Issue #503935 + https://github.com/3F/vsSolutionBuildEvent/issues/14
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        protected virtual string PlatformName(string platform)
        {
            if(platform == null) {
                return null;
            }

            if(String.Compare(platform, "Any CPU", StringComparison.OrdinalIgnoreCase) == 0) {
                return "AnyCPU";
            }
            return platform;
        }
    }
}