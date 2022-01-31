/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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
using net.r_eg.MvsSln.Types;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// An XProject Environment.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class XProjectEnv: IXProjectEnv
    {
        [Obsolete("Use " + nameof(PropertyNames), false)]
        public const string PROP_VALUE_DEFAULT = PropertyNames.UNDEFINED;

        /// <summary>
        /// Solution properties.
        /// </summary>
        protected IDictionary<string, string> slnProperties;

        /// <summary>
        /// Dictionary of raw xml projects by Guid.
        /// Will be used if projects cannot be accessed from filesystem.
        /// </summary>
        protected IDictionary<string, RawText> rawXmlProjects;

        protected List<IXProject> xpProjects = new List<IXProject>();

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
            get => xpProjects;
        }

        /// <summary>
        /// List of evaluated projects that was filtered by Guid.
        /// </summary>
        public IEnumerable<IXProject> UniqueByGuidProjects
        {
            get => Projects?.GroupBy(p => p.ProjectItem.project.pGuid).Select(p => p.First());
        }

        /// <summary>
        /// Access to global Microsoft.Build.Evaluation.ProjectCollection.
        /// Only if you know what you're doing.
        /// </summary>
        public ProjectCollection PrjCollection
        {
            get => ProjectCollection.GlobalProjectCollection;
        }

        /// <summary>
        /// List of valid projects from {PrjCollection}.
        /// Such as something except `.user`,`.metaproj` but contains FirstChild / LastChild XML node.
        /// Only if you know what you're doing.
        /// </summary>
        public IEnumerable<Project> ValidProjects
        {
            get => PrjCollection.LoadedProjects
                    .Where(p => p.Xml.FirstChild != null && p.Xml.LastChild != null)
                    .Where(p => !p.FullPath.EndsWith(".user", StringComparison.InvariantCultureIgnoreCase))
                    .Where(p => !p.FullPath.EndsWith(".metaproj", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Find project by Guid.
        /// </summary>
        /// <param name="guid">Guid of project.</param>
        /// <param name="cfg">Specific configuration.</param>
        /// <returns></returns>
        public IXProject XProjectByGuid(string guid, IConfPlatform cfg)
        {
            return Projects?.FirstOrDefault(p => 
                Eq(p.ProjectItem.projectConfig, cfg) && (p.ProjectGuid == guid)
            );
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
        /// Find project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <param name="cfg">Specified configuration.</param>
        /// <param name="tryLoad">Try to load if not found in current project collection.</param>
        /// <returns></returns>
        public IXProject XProjectByFile(string file, IConfPlatform cfg, bool tryLoad = false)
        {
            return XProjectByFile(file, cfg, tryLoad, null);
        }

        /// <summary>
        /// Find or load project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <param name="cfg">Specified configuration.</param>
        /// <param name="props">Optional properties when loading or null.</param>
        /// <returns></returns>
        public IXProject XProjectByFile(string file, IConfPlatform cfg, IDictionary<string, string> props)
        {
            return XProjectByFile(file, cfg, true, props);
        }

        /// <summary>
        /// Find project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <returns></returns>
        public IEnumerable<IXProject> XProjectsByFile(string file)
        {
            return Projects?.Where(p => p.ProjectFullPath == file);
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
        /// Get or load project using global collection.
        /// Uses default configuration.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem)
        {
            return GetOrLoadProject(pItem, GetProjectProperties(pItem, slnProperties));
        }

        /// <summary>
        /// Get or load project using global collection.
        /// </summary>
        /// <param name="pItem">Specified project.</param>
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
        /// Get or load project using global collection.
        /// </summary>
        /// <param name="pItem">Specified project.</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Project GetOrLoadProject(ProjectItem pItem, IDictionary<string, string> properties)
        {
            if(String.IsNullOrWhiteSpace(pItem.pGuid)) {
                throw new ArgumentException($"GUID of project is empty or null ['{pItem.name}', '{pItem.fullPath}']");
            }

            if(!properties.ContainsKey(PropertyNames.CONFIG) || !properties.ContainsKey(PropertyNames.PLATFORM)) {
                throw new ArgumentException($"Properties does not contain {PropertyNames.CONFIG} or {PropertyNames.PLATFORM} key.");
            }

            ConfigItem conf = new(properties[PropertyNames.CONFIG], properties[PropertyNames.PLATFORM]);
            foreach(var eProject in ValidProjects)
            {
                if(IsEqual(eProject, pItem, conf)) {
                    //LSender.Send(this, $"Found project from collection {pItem.pGuid}: {propCfg} == {eCfg}", Message.Level.Trace);
                    return eProject;
                }
            }

            LSender.Send(this, $"Load project {pItem.pGuid}:{properties[PropertyNames.CONFIG]}|{properties[PropertyNames.PLATFORM]} :: '{pItem.name}' ('{pItem.fullPath}')", Message.Level.Info);
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
            if(!slnProps.ContainsKey(PropertyNames.CONFIG) || !slnProps.ContainsKey(PropertyNames.PLATFORM)) {
                throw new ArgumentException("Solution Configuration or Platform is not defined in used properties.");
            }

            var cfg = Sln.ProjectConfigs
                    .FirstOrDefault(c => 
                        c.PGuid == pItem.pGuid 
                        && c.Sln.IsEqualByRule
                        (
                            slnProps[PropertyNames.CONFIG], 
                            slnProps[PropertyNames.PLATFORM], 
                            true
                        )
                    );

            if(cfg?.Configuration == null || cfg.Platform == null)
            {
                LSender.Send(
                    this, 
                    String.Format(
                        "Project configuration is not found <- sln [{0}|{1}]",
                        slnProps[PropertyNames.CONFIG],
                        slnProps[PropertyNames.PLATFORM]
                    ), 
                    Message.Level.Warn
                );
                return slnProps;
            }

            string config   = cfg.ConfigurationByRule;
            string platform = cfg.PlatformByRule;
            LSender.Send(this, $"-> prj['{config}'; '{platform}']", Message.Level.Info);

            return new Dictionary<string, string>(slnProps) {
                [PropertyNames.CONFIG]      = config,
                [PropertyNames.PLATFORM]    = platform
            };
        }

        /// <summary>
        /// Load available projects via configurations.
        /// It will be added without unloading of previous.
        /// </summary>
        /// <param name="pItems">Specified list or null value to load all available.</param>
        /// <returns>Loaded projects.</returns>
        public virtual IEnumerable<IXProject> LoadProjects(IEnumerable<ProjectItemCfg> pItems = null)
        {
            var loaded = Load(pItems ?? Sln.ProjectItemsConfigs);

            if(loaded == null) {
                return Projects;
            }

            xpProjects?.AddRange(loaded);
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

        /// <summary>
        /// Assign an existing `Microsoft.Build.Evaluation.Project` instances for local collection.
        /// </summary>
        /// <param name="projects">Will use {ValidProjects} if null.</param>
        /// <returns></returns>
        public IEnumerable<IXProject> Assign(IEnumerable<Project> projects = null)
        {
            return (projects ?? ValidProjects).Select(p => AddOrGet(p)).ToArray();
        }

        /// <summary>
        /// Adds `Microsoft.Build.Evaluation.Project` instance into IXProject collection if it does not exist.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IXProject AddOrGet(Project project)
        {
            if(project == null) {
                return null;
            }

            foreach(var prj in Projects)
            {
                if(prj.Project.IsEqual(project)) {
                    return prj;
                }
            }

            var xp = new XProject
            (
                Sln,
                ExtractItemCfg(project),
                project
            );

            xpProjects.Add(xp);
            return xp;
        }

        /// <summary>
        /// Prepares data from `Microsoft.Build.Evaluation.Project` instance.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ProjectItemCfg ExtractItemCfg(Project project)
        {
            if(project == null) throw new ArgumentNullException(nameof(project));

            foreach(ProjectItemCfg cfg in Sln.ProjectItemsConfigs)
            {
                if(IsEqual(project, cfg.project, cfg.projectConfig)) return cfg;
            }

            ProjectItem pItem = new
            (
                project.GetProjectGuid(), 
                project.GetProjectName(), 
                FileExt.GetProjectTypeByFile(project.FullPath), 
                project.GetProjectRelativePath()
            );

            ConfigSln slnCfg = null; // Sln.SolutionConfigs?.FirstOrDefault() ?? Sln.DefaultConfig;

            ConfigPrj prjCfg = new
            (
                project.GetConfig(), 
                project.GetPlatform(),
                pItem.pGuid, 
                false, 
                slnCfg
            );

            return new(pItem, slnCfg, prjCfg);
        }

        /// <summary>
        /// Unloads all evaluated projects at current time.
        /// Decreases `IXProjectEnv.Projects` collection.
        /// </summary>
        /// <param name="throwIfErr">When true, may throw exception if some project cannot be unloaded by some reason.</param>
        public void UnloadAll(bool throwIfErr = true)
        {
            LSender.Send(this, $"Release all loaded projects for current environment (total: {PrjCollection.LoadedProjects.Count})", Message.Level.Debug);

            if(PrjCollection.LoadedProjects.Count < 1) { // nothing to release, thus we need only to reset xp collection
                xpProjects.Clear();
                return;
            }

            foreach(var xp in Projects.ToArray())
            {
                if(!Unload(xp) && throwIfErr) {
                    throw new UnloadException<IXProject>($"Failed unload: '{xp.ProjectGuid}:{xp.ProjectItem.projectConfig}'", xp);
                }
            }

            LSender.Send(this, $"Collection now contains '{PrjCollection.LoadedProjects.Count}' loaded projects.", Message.Level.Debug);
        }

        /// <summary>
        /// Unloads specified project.
        /// Decreases `IXProjectEnv.Projects` collection.
        /// </summary>
        /// <param name="xp"></param>
        /// <returns>False if project was not unloaded by some reason. Otherwise true.</returns>
        public bool Unload(IXProject xp)
        {
            if(xp?.Project == null) {
                return false;
            }

            LSender.Send(this, $"Trying to release loaded project {xp.ProjectGuid}:{xp.ProjectItem.projectConfig}", Message.Level.Debug);
            try
            {
                if(xp.Project.FullPath != null) {
                    PrjCollection.UnloadProject(xp.Project);
                }
                else if(xp.Project.Xml != null) {
                    PrjCollection.TryUnloadProject(xp.Project.Xml);
                }

                return true;
            }
            catch(Exception ex)
            {
                LSender.Send(this, $"Project '{xp.ProjectGuid}:{xp.ProjectItem.projectConfig}':'{xp.ProjectFullPath}' is already unloaded or cannot be unloaded due to error: '{ex.Message}'", Message.Level.Debug);
                return false;
            }
            finally
            {
                xpProjects?.Remove(xp, (a, b) => a.IsLimEqual(b));
            }
        }

        /// <param name="data">Prepared data from solution parser.</param>
        /// <param name="properties">Specified sln properties.</param>
        /// <param name="raw">Optional dictionary of raw xml projects by Guid.</param>
        public XProjectEnv(ISlnResult data, IDictionary<string, string> properties, IDictionary<string, RawText> raw = null)
        {
            Sln             = data ?? throw new ArgumentNullException(nameof(data));
            rawXmlProjects  = raw;

            slnProperties = DefProperties(
                Sln.DefaultConfig,
                Sln.Properties.ExtractDictionary
            )
            .AddOrUpdate(properties);
        }

        /// <param name="data">Prepared data from solution parser.</param>
        /// <param name="raw">Optional dictionary of raw xml projects by Guid.</param>
        public XProjectEnv(ISlnResult data, IDictionary<string, RawText> raw = null)
            : this(data, null, raw)
        {

        }

        /// <param name="pItems"></param>
        /// <returns>List of loaded.</returns>
        protected virtual IEnumerable<IXProject> Load(IEnumerable<ProjectItemCfg> pItems)
        {
            if(pItems == null) {
                return null;
            }

            var xprojects = new List<IXProject>();
            foreach(var pItem in GetUniqPrjCfgs(pItems))
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
            if(rawXmlProjects != null && rawXmlProjects.ContainsKey(pItem.pGuid)) {
                return Load(rawXmlProjects[pItem.pGuid], properties);
            }
            return Load(pItem.fullPath, properties);
        }

        protected virtual Project Load(RawText raw, IDictionary<string, string> properties)
        {
            using(var reader = XmlReader.Create(new StreamReader(raw.data.GetStream(raw.encoding), raw.encoding))) {
                return new Project(reader, properties, null, PrjCollection);
            }
        }

        protected virtual Project Load(string path, IDictionary<string, string> properties)
        {
            return new Project(path, properties, null, PrjCollection);
        }

        protected IEnumerable<ProjectItemCfg> GetUniqPrjCfgs(IEnumerable<ProjectItemCfg> pItems)
        {
            // each sln cfg may refer to the same prj cfg more than once
            return pItems.GroupBy(p => new{ p.project.pGuid, p.projectConfig }).Select(g => g.First());
        }

        protected bool IsEqual(Project prj, ProjectItem pItem, IConfPlatform conf)
        {
            if(prj == null || conf == null) return false;

            // TODO: https://github.com/3F/vsSolutionBuildEvent/issues/40
            //       https://github.com/3F/DllExport/issues/36#issuecomment-320794498

            //if(eProject.GetProjectGuid() != pItem.pGuid) {
            //    continue;
            //}

            if(prj.FullPath != pItem.fullPath) return false;

            return conf.IsEqualByRule(prj.GetConfig(), prj.GetPlatform());
        }

        protected IXProject XProjectByFile(string file, IConfPlatform cfg, bool tryLoad, IDictionary<string, string> props)
        {
            var found = Projects?.FirstOrDefault(p =>
                Eq(p.ProjectItem.projectConfig, cfg) && (p.ProjectFullPath == file)
            );

            if(found != null || !tryLoad || string.IsNullOrWhiteSpace(file)) {
                return found;
            }

            return AddOrGet(GetOrLoadProject
            (
                GetProjectItem(file, props),
                DefProperties(cfg, slnProperties.ToDictionary(k => k.Key, v => v.Value).AddOrUpdate(props))
            ));
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
                    ret[k] = PropertyNames.UNDEFINED;
                }
            };

            _set(PropertyNames.CONFIG, conf?.ConfigurationByRule);
            _set(PropertyNames.PLATFORM, conf?.PlatformByRule);

            return ret;
        }

        protected bool Eq(IConfPlatformPrj a, IConfPlatform b)
        {
            return (ConfigItem)a == (ConfigItem)b;
        }

        private ProjectItem GetProjectItem(string file, IDictionary<string, string> props)
        {
            var ret = new ProjectItem()
            {
                fullPath    = file,
                pGuid       = props.GetOrDefault(PropertyNames.PRJ_GUID) ?? Guid.Empty.ToString(),
                EpType      = FileExt.GetProjectTypeByFile(file),
                name        = Path.GetFileNameWithoutExtension(file),
            };
            ret.pType = Guids.GuidBy(ret.EpType);

            if(Sln == null)
            {
                return ret;
            }

            var found = Sln.ProjectItems.FirstOrDefault(p => p.fullPath == file);
            if(found.path != null)
            {
                return found;
            }

            ret.path = Sln.SolutionDir.MakeRelativePath(file);
            return ret;
        }
    }
}