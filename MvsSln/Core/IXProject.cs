/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    public interface IXProject
    {
        /// <summary>
        /// Access to project instance of Microsoft.Build.Evaluation.
        /// </summary>
        Project Project { get; }

        /// <summary>
        /// ProjectItem and its configurations.
        /// </summary>
        ProjectItemCfg ProjectItem { get; }

        /// <summary>
        /// Access to solution data if this was initialized with its context.
        /// </summary>
        ISlnResult Sln { get; }

        /// <summary>
        /// Provides unique identifier for project (not instance).
        /// </summary>
        Guid PId { get; }

        /// <summary>
        /// The Guid of this project.
        /// </summary>
        string ProjectGuid { get; }

        /// <summary>
        /// The ProjectName of this project.
        /// </summary>
        string ProjectName { get; }

        /// <summary>
        /// Gets the root directory for this project.
        /// </summary>
        string ProjectPath { get; }

        /// <summary>
        /// Gets the full path to the project source file.
        /// </summary>
        string ProjectFullPath { get; }

        /// <summary>
        /// Access to global properties of project.
        /// </summary>
        IDictionary<string, string> GlobalProperties { get; }

        /// <summary>
        /// Saves the project to the file system, if modified.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves the project to the file system, if modified or if the path to the project
        /// source code changes, using the given character encoding.
        /// </summary>
        /// <param name="path">Destination path of the the project source code.</param>
        /// <param name="enc"></param>
        void Save(string path, Encoding enc);

        /// <summary>
        /// To add 'Import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="checking">To check existence of target via 'Condition' attr.</param>
        /// <param name="label">Optional 'Label' attr.</param>
        /// <returns>true value if target has been added.</returns>
        bool AddImport(string target, bool checking, string label = null);

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="condition">Use 'Condition' attr. Can be null to avoid this attr.</param>
        /// <param name="label">Optional 'Label' attr.</param>
        /// <returns>true value if target has been added.</returns>
        bool AddImport(string target, string condition, string label = null);

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="element">Specified 'Import' element to add.</param>
        /// <returns>true value if it has been added.</returns>
        bool AddImport(ImportElement element);

        /// <summary>
        /// To add 'import' elements inside ImportGroup.
        /// Will stop the adding if some of this cannot be added.
        /// </summary>
        /// <param name="elements">List of specified 'Import' elements to add.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        /// <param name="label">Optional 'Label' attr for group.</param>
        /// <returns>true value only if all 'import' elements has been successfully added. False if one of this is failed.</returns>
        bool AddImport(IEnumerable<ImportElement> elements, string condition = null, string label = null);

        /// <summary>
        /// To remove the first found 'Import' element.
        /// </summary>
        /// <param name="project">Target project.</param>
        /// <returns>true value if it has been found and removed.</returns>
        bool RemoveImport(string project);

        /// <summary>
        /// To remove 'Import' element.
        /// </summary>
        /// <param name="element">Specified 'Import' element to remove.</param>
        /// <param name="holdEmptyGroup">Holds empty group if it was inside.</param>
        /// <returns>true value if it has been removed.</returns>
        bool RemoveImport(ImportElement element, bool holdEmptyGroup = false);

        /// <summary>
        /// Retrieve the first found 'Import' element if it exists.
        /// </summary>
        /// <param name="project">Optional filter by the Project attribute.</param>
        /// <returns></returns>
        ImportElement GetImport(string project = null);

        /// <summary>
        /// Retrieve the first found 'Import' element if it exists.
        /// </summary>
        /// <param name="project">Filter by the Project attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="label">Filter by the Label attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="eq">Equals() if true or EndsWith() function for comparing Project attribute.</param>
        /// <returns></returns>
        ImportElement GetImport(string project, string label, bool eq = false);

        /// <summary>
        /// Retrieve the all found 'Import' elements.
        /// </summary>
        /// <param name="project">Optional filter by the Project attribute.</param>
        /// <returns></returns>
        IEnumerable<ImportElement> GetImports(string project = null);

        /// <summary>
        /// Retrieve the all found 'Import' elements.
        /// </summary>
        /// <param name="project">Filter by the Project attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="label">Filter by the Label attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="eq">Equals() if true or EndsWith() function for comparing Project attribute.</param>
        /// <returns></returns>
        IEnumerable<ImportElement> GetImports(string project, string label, bool eq = false);

        /// <summary>
        /// Get a property in this project with the specified name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="localScope">If true, will return default value for any special and imported properties type.</param>
        /// <returns>Found property or <see cref="PropertyItem.None"/> if does not exist.</returns>
        PropertyItem GetProperty(string name, bool localScope = true);

        /// <summary>
        /// Sets or adds a property with the given name and unevaluated value to the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluated">The new unevaluated value of the property.</param>
        /// <returns></returns>
        PropertyItem SetProperty(string name, string unevaluated);

        /// <summary>
        /// Sets or adds a property with the given name and unevaluated value to the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluated">The new unevaluated value of the property.</param>
        /// <param name="condition">Use 'Condition' attr.</param>
        /// <returns></returns>
        PropertyItem SetProperty(string name, string unevaluated, string condition);

        /// <summary>
        /// Sets or adds properties inside group.
        /// To remove group, just delete all properties inside.
        /// </summary>
        /// <param name="properties">List of properties name=unevaluatedValue.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        void SetProperties(IEnumerable<KeyValuePair<string, string>> properties, string condition = null);

        /// <summary>
        /// Sets or adds properties inside group.
        /// To remove group, just delete all properties inside.
        /// </summary>
        /// <param name="properties">List of properties via PropertyItem.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        void SetProperties(IEnumerable<PropertyItem> properties, string condition = null);

        /// <summary>
        /// Removes an property from the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="revalue">if true, will reevaluate data of project after removing.</param>
        /// <returns></returns>
        bool RemoveProperty(string name, bool revalue = false);

        /// <summary>
        /// Removes an property from the project.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="revalue">if true, will reevaluate data of project after removing.</param>
        /// <returns></returns>
        bool RemoveProperty(PropertyItem property, bool revalue = false);

        /// <summary>
        /// All properties in this project.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyItem> GetProperties();

        /// <summary>
        /// Reevaluates data of project if necessary.
        /// For example, if project contains 2 or more same properties by name:
        /// * After RemoveProperty(...) the second property still will be unavailable for GetProperty(...) 
        ///  because its node does not contain this at all. Use this to update nodes.
        /// </summary>
        void Reevaluate();

        /// <summary>
        /// Makes relative path from this project.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetRelativePath(string path);

        /// <summary>
        /// Makes full path using path to this project as the base.
        /// </summary>
        /// <param name="relative">any path relative to the current project.</param>
        /// <returns></returns>
        string GetFullPath(string relative);

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <returns></returns>
        bool AddReference(string inc);

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="asm">Assembly for adding.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        bool AddReference(Assembly asm, bool local, bool? embed = null, bool? spec = null);

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="fullpath">Full path to binary file.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        bool AddReference(string fullpath, bool local, bool? embed = null, bool? spec = null);

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <param name="path">Meta 'HintPath'.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        bool AddReference(string inc, string path, bool local, bool? embed = null, bool? spec = null);

        /// <summary>
        /// Adds 'ProjectReference' item.
        /// </summary>
        /// <param name="project">Information about project.</param>
        /// <returns></returns>
        bool AddProjectReference(ProjectItem project);

        /// <summary>
        /// Adds 'ProjectReference' item.
        /// </summary>
        /// <param name="path">Path to project file.</param>
        /// <param name="guid">The Guid of project.</param>
        /// <param name="name">The name of project.</param>
        /// <param name="makeRelative">Make relative path.</param>
        /// <returns></returns>
        bool AddProjectReference(string path, string guid, string name, bool makeRelative = false);

        /// <summary>
        /// Adds 'PackageReference' item.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <param name="version">Package version: 2.5; 1.6.0-beta3; ...</param>
        /// <param name="meta">Optional metadata, eg. ExcludeAssets="runtime" etc.</param>
        /// <returns></returns>
        bool AddPackageReference(string id, string version, IEnumerable<KeyValuePair<string, string>> meta = null);

        /// <summary>
        /// Adds an item to the project.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The Include attribute of this item.</param>
        /// <param name="meta">Optional metadata list.</param>
        /// <returns>true if item has been added.</returns>
        bool AddItem(string type, string inc, IEnumerable<KeyValuePair<string, string>> meta = null);

        /// <summary>
        /// Retrieve all available items from projects.
        /// </summary>
        /// <param name="type">The item type or null value to get all.</param>
        /// <param name="inc">The unevaluated value of the Include attribute or null value to get all.</param>
        /// <returns></returns>
        IEnumerable<Item> GetItems(string type = null, string inc = null);

        /// <summary>
        /// Retrieve first item by type.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        Item GetItem(string type, string inc);

        /// <summary>
        /// Get all available 'Reference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        IEnumerable<Item> GetReferences(string inc = null);

        /// <summary>
        /// Get all available 'ProjectReference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        IEnumerable<Item> GetProjectReferences(string inc = null);

        /// <summary>
        /// Get all available 'PackageReference' items.
        /// </summary>
        IEnumerable<Item> GetPackageReferences();

        /// <summary>
        /// Get first available 'Reference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        Item GetFirstReference(string inc);

        /// <summary>
        /// Get first available 'ProjectReference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        Item GetFirstProjectReference(string inc);

        /// <summary>
        /// Get first available 'PackageReference' item.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <returns></returns>
        Item GetFirstPackageReference(string id);

        /// <summary>
        /// Remove first item from project by type.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        bool RemoveItem(string type, string inc);

        /// <summary>
        /// Remove selected item from project.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool RemoveItem(Item item);

        /// <summary>
        /// Remove 'Reference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        bool RemoveReference(string inc);

        /// <summary>
        /// Remove 'ProjectReference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        bool RemoveProjectReference(string inc);

        /// <summary>
        /// Remove 'PackageReference' item from project.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <returns></returns>
        bool RemovePackageReference(string id);
    }
}