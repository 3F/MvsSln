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
using System.Linq;
using System.Xml;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Exceptions;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Isolated environment.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class IsolatedEnv: IEnvironment, IDisposable
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
        /// Dictionary of raw xml projects by Guid.
        /// Will be used if projects cannot be accessed from filesystem.
        /// </summary>
        protected IDictionary<string, RawText> rawXmlProjects;

        /// <summary>
        /// Access to Solution data.
        /// </summary>
        public ISlnResult Sln
        {
            get;
            protected set;
        }

        /// <summary>
        /// List of all evaluated projects at current time 
        /// with unique configuration for each instance.
        /// </summary>
        public IEnumerable<IXProject> Projects
        {
            get;
            set;
        }

        /// <summary>
        /// List of evaluated projects that filtered by Guid.
        /// </summary>
        public IEnumerable<IXProject> UniqueByGuidProjects
        {
            get => Projects?.GroupBy(p => p.ProjectItem.project.pGuid).Select(p => p.First());
        }

        /// <summary>
        /// Access to GlobalProjectCollection
        /// </summary>
        public ProjectCollection PrjCollection
        {
            get => ProjectCollection.GlobalProjectCollection;
        }

        /// <summary>
        /// Find project by Guid.
        /// </summary>
        /// <param name="guid">Guid of project.</param>
        /// <param name="cfg">Specific configuration.</param>
        /// <returns></returns>
        public IXProject XProjectByGuid(string guid, IConfPlatform cfg)
        {
            return Projects?.Where(p => 
                Eq(p.ProjectItem.projectConfig, cfg) && (p.ProjectGuid == guid)
            )
            .FirstOrDefault();
        }

        /// <summary>
        /// Find project by Guid.
        /// </summary>
        /// <param name="guid">Guid of project.</param>
        /// <returns></returns>
        public IXProject[] XProjectsByGuid(string guid)
        {
            return Projects?.Where(p => p.ProjectGuid == guid).ToArray();
        }

        /// <summary>
        /// Find projects by name.
        /// </summary>
        /// <param name="name">ProjectName.</param>
        /// <param name="cfg">Specific configuration.</param>
        /// <returns></returns>
        public IXProject[] XProjectsByName(string name, IConfPlatform cfg)
        {
            return Projects?.Where(
                p => Eq(p.ProjectItem.projectConfig, cfg) && p.ProjectName == name
            )
            .ToArray();
        }

        /// <summary>
        /// Find projects by name.
        /// </summary>
        /// <param name="name">ProjectName.</param>
        /// <returns></returns>
        public IXProject[] XProjectsByName(string name)
        {
            return Projects?.Where(p => p.ProjectName == name).ToArray();
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
        /// <param name="cfg">Configuration of project to load.</param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem, IConfPlatform cfg)
        {
            var xprj = XProjectByGuid(pItem.pGuid, cfg);
            if(xprj != null) {
                return xprj.Project;
            }

            return GetOrLoadProject(pItem, DefProperties(cfg, slnProperties));
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
                if(eProject.GetProjectGuid() != pItem.pGuid) {
                    continue;
                }

                var eCfg    = new ConfigItem(eProject.GetPropertyValue(P_CONFIG), eProject.GetPropertyValue(P_PLATFORM));
                var propCfg = new ConfigItem(properties[P_CONFIG], properties[P_PLATFORM]);

                if(eCfg == propCfg) {
                    //LSender.Send(this, $"Found project from collection {pItem.pGuid}: {propCfg} == {eCfg}", Message.Level.Trace);
                    return eProject;
                }
            }

            LSender.Send(this, $"Load project {pItem.pGuid}:{properties[P_CONFIG]}|{properties[P_PLATFORM]} :: '{pItem.name}' ('{pItem.fullPath}')", Message.Level.Info);
            if(String.IsNullOrWhiteSpace(pItem.fullPath)) {
                throw new NotFoundException($"Path is empty to project ['{pItem.name}', '{pItem.pGuid}']");
            }

            return Load(pItem, properties);
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

            var cfg = Sln
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

            string platform = cfg.PlatformByRule;
            LSender.Send(this, $"-> prj['{cfg.Configuration}'; '{platform}']", Message.Level.Info);

            return new Dictionary<string, string>(slnProps) {
                [P_CONFIG]   = cfg.Configuration,
                [P_PLATFORM] = platform
            };
        }

        /// <summary>
        /// Load available projects via configurations.
        /// It will be added without unloading of previous.
        /// </summary>
        /// <param name="pItems">Specific list or null value to load all available.</param>
        /// <returns>Loaded projects.</returns>
        public virtual IEnumerable<IXProject> LoadProjects(IEnumerable<ProjectItemCfg> pItems = null)
        {
            Projects = Load(pItems ?? Sln.ProjectItemsConfigs);
            return Projects;
        }

        /// <summary>
        /// Load the only one configuration for each available project.
        /// </summary>
        /// <returns>Loaded projects.</returns>
        public IEnumerable<IXProject> LoadMinimalProjects()
        {
            IConfPlatform slnCfg = Sln.SolutionConfigs?.FirstOrDefault();

            if(slnCfg == null) {
                return LoadProjects();
            }
            return LoadProjects(Sln.ProjectItemsConfigs?.Where(p => p.solutionConfig == slnCfg));
        }

        /// <param name="data">Prepared data from solution parser.</param>
        /// <param name="raw">Optional dictionary of raw xml projects by Guid.</param>
        public IsolatedEnv(ISlnResult data, IDictionary<string, RawText> raw = null)
        {
            Sln             = data;
            rawXmlProjects  = raw;

            slnProperties = DefProperties(
                Sln.DefaultConfig,
                Sln.Properties.ExtractDictionary
            );
        }

        /// <param name="pItems"></param>
        /// <returns>List of loaded.</returns>
        protected virtual IEnumerable<IXProject> Load(IEnumerable<ProjectItemCfg> pItems)
        {
            if(pItems == null) {
                return null;
            }

            var xprojects = new List<IXProject>();
            foreach(var pItem in pItems)
            {
                if(pItem.project.pGuid == null) {
                    LSender.Send(this, $"{pItem.solutionConfig} -> {pItem.projectConfig} does not have reference to project item.", Message.Level.Debug);
                    continue;
                }
                Project eProject = GetOrLoadProject(pItem.project, pItem.projectConfig);
                xprojects.Add(new XProject(Sln, pItem, eProject));
            }
            return xprojects;
        }

        protected Project Load(ProjectItem pItem, IDictionary<string, string> properties)
        {
            if(rawXmlProjects != null && rawXmlProjects.ContainsKey(pItem.pGuid))
            {
                var raw = rawXmlProjects[pItem.pGuid];

                using(var reader = XmlReader.Create(new StreamReader(raw.data.GetStream(raw.encoding), raw.encoding))) {
                    return new Project(reader, properties, null, PrjCollection);
                }
            }
            return new Project(pItem.fullPath, properties, null, PrjCollection);
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
            _set(P_PLATFORM, conf?.PlatformByRule);

            return ret;
        }

        protected bool Eq(IConfPlatformPrj a, IConfPlatform b)
        {
            return (ConfigItem)a == (ConfigItem)b;
        }

        //TODO: user option along with `LoadProjects` func
        protected bool Free()
        {
            if(Projects == null) {
                return true;
            }

            LSender.Send(this, $"Release loaded projects for current environment (total: {PrjCollection.LoadedProjects.Count})", Message.Level.Debug);
            foreach(var xp in Projects)
            {
                if(xp.Project == null) {
                    continue;
                }

                try
                {
                    if(xp.Project.FullPath != null) {
                        PrjCollection.UnloadProject(xp.Project);
                    }
                    else if(xp.Project.Xml != null) {
                        PrjCollection.TryUnloadProject(xp.Project.Xml);
                    }
                }
                catch(Exception ex) {
                    LSender.Send(this, $"Project '{xp.ProjectGuid}:{xp.ProjectItem.projectConfig}' was not unloaded: '{ex.Message}'", Message.Level.Trace);
                }
            }

            LSender.Send(this, $"Collection now contains '{PrjCollection.LoadedProjects.Count}' loaded projects.", Message.Level.Debug);
            return true;
        }

        #region IDisposable

        // To detect redundant calls
        private bool disposed = false;

        // To correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }
            disposed = true;

            //...
            Free();
        }

        #endregion
    }
}