/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;

namespace net.r_eg.MvsSln.Core
{
    // TODO: typo, add Get+ prefix for all XProjectBy...(), ie. GetXProjectBy...()
    [Guid("1BED7620-25A0-4FC3-BD44-A284782CA68A")]
    public interface IXProjectEnv
    {
        /// <summary>
        /// Access to Solution data.
        /// </summary>
        ISlnResult Sln { get; }

        /// <summary>
        /// List of all evaluated projects at current time 
        /// with unique configuration for each instance.
        /// </summary>
        IEnumerable<IXProject> Projects { get; }

        /// <summary>
        /// List of evaluated projects that was filtered by Guid.
        /// </summary>
        IEnumerable<IXProject> UniqueByGuidProjects { get; }

        /// <summary>
        /// Access to global Microsoft.Build.Evaluation.ProjectCollection.
        /// Only if you know what you're doing.
        /// </summary>
        ProjectCollection PrjCollection { get; }

        /// <summary>
        /// List of valid projects from {PrjCollection}.
        /// Such as something except `.user`,`.metaproj` but contains FirstChild / LastChild XML node.
        /// Only if you know what you're doing.
        /// </summary>
        IEnumerable<Project> ValidProjects { get; }

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
        /// Find project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <param name="cfg">Specified configuration.</param>
        /// <param name="tryLoad">Try to load if not found in current project collection.</param>
        /// <returns></returns>
        IXProject XProjectByFile(string file, IConfPlatform cfg, bool tryLoad = false);

        /// <summary>
        /// Find or load project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <param name="cfg">Specified configuration.</param>
        /// <param name="props">Optional properties when loading or null.</param>
        /// <returns></returns>
        IXProject XProjectByFile(string file, IConfPlatform cfg, IDictionary<string, string> props);

        /// <summary>
        /// Find project by full path to file.
        /// </summary>
        /// <param name="file">Full path to file.</param>
        /// <returns></returns>
        IEnumerable<IXProject> XProjectsByFile(string file);

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
        /// Get or load project using global collection.
        /// Uses default configuration.
        /// </summary>
        /// <param name="pItem">Specific project.</param>
        /// <returns></returns>
        Project GetOrLoadProject(ProjectItem pItem);

        /// <summary>
        /// Get or load project using global collection.
        /// </summary>
        /// <param name="pItem">Specified project.</param>
        /// <param name="cfg">Configuration of project to load.</param>
        /// <returns></returns>
        Project GetOrLoadProject(ProjectItem pItem, IConfPlatform cfg);

        /// <summary>
        /// Get or load project using global collection.
        /// </summary>
        /// <param name="pItem">Specified project.</param>
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
        /// It will be added without unloading of previous.
        /// </summary>
        /// <param name="pItems">Specified list or null value to load all available.</param>
        /// <returns>Loaded projects.</returns>
        IEnumerable<IXProject> LoadProjects(IEnumerable<ProjectItemCfg> pItems = null);

        /// <summary>
        /// Load the only one configuration for each available project.
        /// </summary>
        /// <returns>Loaded projects.</returns>
        IEnumerable<IXProject> LoadMinimalProjects();

        /// <summary>
        /// Assign an existing `Microsoft.Build.Evaluation.Project` instances for local collection.
        /// </summary>
        /// <param name="projects">Will use {ValidProjects} if null.</param>
        /// <returns></returns>
        IEnumerable<IXProject> Assign(IEnumerable<Project> projects = null);

        /// <summary>
        /// Adds `Microsoft.Build.Evaluation.Project` instance into IXProject collection if it does not exist.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        IXProject AddOrGet(Project project);

        /// <summary>
        /// Prepares data from `Microsoft.Build.Evaluation.Project` instance.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        ProjectItemCfg ExtractItemCfg(Project project);

        /// <summary>
        /// Unloads all evaluated projects at current time.
        /// Decreases `IXProjectEnv.Projects` collection.
        /// </summary>
        /// <param name="throwIfErr">When true, may throw exception if some project cannot be unloaded by some reason.</param>
        void UnloadAll(bool throwIfErr = true);

        /// <summary>
        /// Unloads specified project.
        /// Decreases `IXProjectEnv.Projects` collection.
        /// </summary>
        /// <param name="xp"></param>
        /// <returns>False if project was not unloaded by some reason. Otherwise true.</returns>
        bool Unload(IXProject xp);
    }
}