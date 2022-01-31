/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    // TODO: review. { Items ; Groups } -> [ Condition ] => Actions
    [DebuggerDisplay("{DbgDisplay}")]
    public class XProject: IXProject
    {
        private const string ITEM_REF   = "Reference";
        private const string PKG_REF    = "PackageReference";
        private const string PRJ_REF    = "ProjectReference";

        /// <summary>
        /// Access to project instance of Microsoft.Build.Evaluation.
        /// </summary>
        public Project Project
        {
            get;
            protected set;
        }

        /// <summary>
        /// ProjectItem and its configurations.
        /// </summary>
        public ProjectItemCfg ProjectItem
        {
            get;
            protected set;
        }

        /// <summary>
        /// Access to solution data if this was initialized with its context.
        /// </summary>
        public ISlnResult Sln
        {
            get;
            protected set;
        }

        /// <summary>
        /// Provides unique identifier for project (not instance).
        /// </summary>
        public Guid PId
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Guid of this project.
        /// </summary>
        public string ProjectGuid => GetProjectGuid(Project);

        /// <summary>
        /// The ProjectName of this project.
        /// </summary>
        public string ProjectName => GetProjectName(Project); 

        /// <summary>
        /// Gets the root directory for this project.
        /// </summary>
        public string ProjectPath => Project.DirectoryPath;

        /// <summary>
        /// Gets the full path to the project source file.
        /// </summary>
        public string ProjectFullPath => Project.FullPath;

        /// <summary>
        /// Access to global properties of project.
        /// </summary>
        public IDictionary<string, string> GlobalProperties => Project.GlobalProperties;

        /// <summary>
        /// The base path for MakeRelativePath() functions etc.
        /// </summary>
        protected string RootPath => ProjectPath;

        /// <summary>
        /// Saves the project to the file system, if modified.
        /// //TODO: ~"... has been modified outside the environment."
        /// </summary>
        public void Save() => Project.Save();

        /// <summary>
        /// Saves the project to the file system, if modified or if the path to the project
        /// source code changes, using the given character encoding.
        /// </summary>
        /// <param name="path">Destination path of the the project source code.</param>
        /// <param name="enc"></param>
        public void Save(string path, Encoding enc) => Project.Save(path, enc);

        /// <summary>
        /// To add 'Import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="checking">To check existence of target via 'Condition' attr.</param>
        /// <param name="label">Optional 'Label' attr.</param>
        /// <returns>true value if target has been added.</returns>
        public bool AddImport(string target, bool checking, string label = null)
        {
            return AddImport(target, checking ? $"Exists('{target}')" : null, label);
        }

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="condition">Use 'Condition' attr. Can be null to avoid this attr.</param>
        /// <param name="label">Optional 'Label' attr.</param>
        /// <returns>true value if target has been added.</returns>
        public bool AddImport(string target, string condition, string label = null)
        {
            if(string.IsNullOrWhiteSpace(target)) {
                return false;
            }
            return AddImport(Project.Xml.AddImport(target), condition, label);
        }

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="element">Specified 'Import' element to add.</param>
        /// <returns>true value if it has been added.</returns>
        public bool AddImport(ImportElement element)
        {
            return AddImport(element.project, element.condition, element.label);
        }

        /// <summary>
        /// To add 'import' elements inside ImportGroup.
        /// Will stop the adding if some of this cannot be added.
        /// </summary>
        /// <param name="elements">List of specified 'Import' elements to add.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        /// <param name="label">Optional 'Label' attr for group.</param>
        /// <returns>true value only if all 'import' elements has been successfully added. False if one of this is failed.</returns>
        public bool AddImport(IEnumerable<ImportElement> elements, string condition = null, string label = null)
        {
            var group = Project.Xml.AddImportGroup();
            foreach(var elem in elements)
            {
                if(AddImport(group.AddImport(elem.project), elem.condition, elem.label)) {
                    continue;
                }

                Log.LSender.Send(
                    this,
                    $"One of the 'Import' elements cannot be added: '{elem.project}'",
                    Log.Message.Level.Debug
                );
                return false;
            }

            if(condition != null) {
                group.Condition = condition;
            }

            if(label != null) {
                group.Label = label;
            }

            return true;
        }

        /// <summary>
        /// To remove the first found 'Import' element.
        /// </summary>
        /// <param name="project">Target project.</param>
        /// <returns>true value if it has been found and removed.</returns>
        public bool RemoveImport(string project) => RemoveImport(GetImport(project));

        /// <summary>
        /// To remove 'Import' element.
        /// </summary>
        /// <param name="element">Specified 'Import' element to remove.</param>
        /// <param name="holdEmptyGroup">Holds empty group if it was inside.</param>
        /// <returns>true value if it has been removed.</returns>
        public bool RemoveImport(ImportElement element, bool holdEmptyGroup = false)
        {
            if(element.parentElement == null) {
                return false;
            }

            // https://github.com/3F/DllExport/issues/77
            // 'The node is not parented by this object' if an `Import` element is already placed inside `ImportGroup`.
            //Project.Xml.RemoveChild(element.parentElement);

            var imp = element.parentElement;
            if(imp.Parent is ProjectImportGroupElement container)
            {
                if(container.Imports.Count > 1) {
                    container.RemoveChild(imp);
                    return true;
                }

                if(holdEmptyGroup) {
                    container.RemoveChild(imp); // leave as an ~ <ImportGroup />
                }
                else {
                    Project.Xml.RemoveChild(container);
                }
                return true;
            }

            Project.Xml.RemoveChild(imp);
            return true;
        }

        /// <summary>
        /// Retrieve the first found 'Import' element if it exists.
        /// </summary>
        /// <param name="project">Optional filter by the Project attribute.</param>
        /// <returns></returns>
        public ImportElement GetImport(string project = null) => GetImports(project).FirstOrDefault();

        /// <summary>
        /// Retrieve the first found 'Import' element if it exists.
        /// </summary>
        /// <param name="project">Filter by the Project attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="label">Filter by the Label attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="eq">Equals() if true or EndsWith() function for comparing Project attribute.</param>
        /// <returns></returns>
        public ImportElement GetImport(string project, string label, bool eq = false)
            => GetImports(project, label, eq).FirstOrDefault();

        /// <summary>
        /// Retrieve the all found 'Import' elements.
        /// </summary>
        /// <param name="project">Optional filter by the Project attribute.</param>
        /// <returns></returns>
        public IEnumerable<ImportElement> GetImports(string project = null)
        {
            if(string.IsNullOrWhiteSpace(project)) {
                return Project.Xml.Imports.Select(i => GetImportElement(i));
            }

            return Project.Xml.Imports.Where(i => i.Project == project)
                                        .Select(i => GetImportElement(i));
        }

        /// <summary>
        /// Retrieve the all found 'Import' elements.
        /// </summary>
        /// <param name="project">Filter by the Project attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="label">Filter by the Label attribute. Case-insensitive variant. Can be null to skip comparing.</param>
        /// <param name="eq">Equals() if true or EndsWith() function for comparing Project attribute.</param>
        /// <returns></returns>
        public IEnumerable<ImportElement> GetImports(string project, string label, bool eq = false)
        {
            var ret = GetImports();
            var cmp = StringComparison.InvariantCultureIgnoreCase;

            if(project != null) {
                if(eq) {
                    ret = ret.Where(i => project.Equals(i.project, cmp));
                }
                else {
                    ret = ret.Where(i => i.project != null && i.project.EndsWith(project, cmp));
                }
            }

            if(label != null) {
                ret = ret.Where(i => label.Equals(i.label, cmp));
            }

            return ret;
        }

        /// <summary>
        /// The property in this project that has the specified name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="localScope">If true, will return default value for any special and imported properties type.</param>
        /// <returns>null if no property of that name and scope exists.</returns>
        public PropertyItem GetProperty(string name, bool localScope = true)
        {
            PropertyItem defvalue = default;
            if(string.IsNullOrWhiteSpace(name)) {
                return defvalue;
            }

            var ret = GetProperty(Project.GetProperty(name));
            if(!localScope) {
                return ret;
            }

            if(ret.isImported || ret.isEnvironmentProperty || ret.isReservedProperty || ret.isGlobalProperty) 
            {
                defvalue.parentProject = ret.parentProject;
                return defvalue;
            }
            return ret;
        }

        /// <summary>
        /// Sets or adds a property with the given name and unevaluated value to the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluated">The new unevaluated value of the property.</param>
        /// <returns></returns>
        public PropertyItem SetProperty(string name, string unevaluated) => SetProperty(name, unevaluated, null);

        /// <summary>
        /// Sets or adds a property with the given name and unevaluated value to the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluated">The new unevaluated value of the property.</param>
        /// <param name="condition">Use 'Condition' attr.</param>
        /// <returns></returns>
        public PropertyItem SetProperty(string name, string unevaluated, string condition)
        {
            var eProperty = Project.SetProperty(name, unevaluated);

            if(condition != null

                // Some cases such global property etc may produce empty xml nodes. 
                // This usually accompanied by throwing InvalidOperationException when using related SetProperty() like for 'Platform' global property,
                // but just to be sure when other unknown case.
                && eProperty?.Xml != null) 
            {
                eProperty.Xml.Condition = condition;
            }

            return GetProperty(eProperty);
        }

        /// <summary>
        /// Sets or adds properties inside group.
        /// To remove group, just delete all properties inside.
        /// </summary>
        /// <param name="properties">List of properties name=unevaluatedValue.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        public void SetProperties(IEnumerable<KeyValuePair<string, string>> properties, string condition = null)
        {
            SetProperties(properties.Select(p => new PropertyItem(p.Key, p.Value)), condition);
        }

        /// <summary>
        /// Sets or adds properties inside group.
        /// To remove group, just delete all properties inside.
        /// </summary>
        /// <param name="properties">List of properties via PropertyItem.</param>
        /// <param name="condition">Optional 'Condition' attr for group.</param>
        public void SetProperties(IEnumerable<PropertyItem> properties, string condition = null)
        {
            var group = Project.Xml.AddPropertyGroup();
            if(condition != null) {
                group.Condition = condition;
            }

            foreach(var prop in properties)
            {
                var ret = group.SetProperty(prop.name, prop.unevaluatedValue);
                if(prop.condition != null) {
                    ret.Condition = prop.condition;
                }
            }
        }

        /// <summary>
        /// Removes an property from the project. Local Scope only.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="revalue">if true, will reevaluate data of project after removing.</param>
        /// <returns></returns>
        public bool RemoveProperty(string name, bool revalue = false) => RemoveProperty(GetProperty(name), revalue);

        /// <summary>
        /// Removes an property from the project.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="revalue">if true, will reevaluate data of project after removing</param>
        /// <returns></returns>
        public bool RemoveProperty(PropertyItem property, bool revalue = false)
        {
            if(property.parentProperty == null) {
                return false;
            }

            try {
                return Project.RemoveProperty(property.parentProperty);
            }
            catch(Exception ex)
            {
                Log.LSender.Send(
                    this, 
                    String.Format(
                        "Property '{0}' cannot be removed. [{1};{2};{3};{4};{5}]: '{6}'",
                        property.name,
                        property.isImported,
                        property.isGlobalProperty,
                        property.isReservedProperty,
                        property.isEnvironmentProperty,
                        property.isUserDef,
                        ex.Message
                    ),
                    Log.Message.Level.Warn
                );
                return false;
            }
            finally
            {
                if(revalue) {
                    Reevaluate();
                }
            }
        }

        /// <summary>
        /// All properties in this project.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyItem> GetProperties()
        {
            foreach(var p in Project.Properties) {
                yield return GetProperty(p);
            }
        }

        /// <summary>
        /// Reevaluates data of project if necessary.
        /// For example, if project contains 2 or more same properties by name:
        /// * After RemoveProperty(...) the second property still will be unavailable for GetProperty(...) 
        ///  because its node does not contain this at all. Use this to update nodes.
        /// </summary>
        public void Reevaluate() => Project?.ReevaluateIfNecessary();

        /// <summary>
        /// Makes relative path from this project.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string GetRelativePath(string path) => RootPath.MakeRelativePath(path);

        /// <summary>
        /// Makes full path using path to this project as the base.
        /// </summary>
        /// <param name="relative">any not null path relative to the current project.</param>
        /// <returns></returns>
        public string GetFullPath(string relative) => Path.GetFullPath(Path.Combine(RootPath, relative));

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <returns></returns>
        public bool AddReference(string inc) => AddItem(ITEM_REF, inc);

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="asm">Assembly for adding.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        public bool AddReference(Assembly asm, bool local, bool? embed = null, bool? spec = null)
        {
            return AddReference(asm.ToString(), GetRelativePath(asm.Location), local, embed, spec);
        }

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="fullpath">Full path to binary file.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        public bool AddReference(string fullpath, bool local, bool? embed = null, bool? spec = null)
        {
            //TODO: fullpath may contain unevaluated properties, e.g.: metalib\$(namespace)\$(libname)
            string inc = AssemblyName.GetAssemblyName(fullpath).FullName;
            return AddReference(inc, GetRelativePath(fullpath), local, embed, spec);
        }

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <param name="path">Meta 'HintPath'.</param>
        /// <param name="local">Meta 'Private' - i.e. Copy Local.</param>
        /// <param name="embed">Meta 'EmbedInteropTypes'.</param>
        /// <param name="spec">Meta 'SpecificVersion'.</param>
        /// <returns></returns>
        public bool AddReference(string inc, string path, bool local, bool? embed = null, bool? spec = null)
        {
            var meta = new Dictionary<string, string>() {
                { "HintPath", path },
                { "Private", local.ToString() }
            };

            if(embed != null) {
                meta["EmbedInteropTypes"] = embed.ToString();
            }

            if(spec != null) {
                meta["SpecificVersion"] = spec.ToString();
            }

            return AddItem(ITEM_REF, inc, meta);
        }

        /// <summary>
        /// Adds 'ProjectReference' item.
        /// </summary>
        /// <param name="project">Information about project.</param>
        /// <returns></returns>
        public bool AddProjectReference(ProjectItem project)
        {
            var path = GetRelativePath(project.fullPath) ?? project.path;
            return AddProjectReference(path, project.pGuid, project.name, false);
        }

        /// <summary>
        /// Adds 'ProjectReference' item.
        /// </summary>
        /// <param name="path">Path to project file.</param>
        /// <param name="guid">The Guid of project.</param>
        /// <param name="name">The name of project.</param>
        /// <param name="makeRelative">Make relative path.</param>
        /// <returns></returns>
        public bool AddProjectReference(string path, string guid, string name, bool makeRelative = false)
        {
            var meta = new Dictionary<string, string>() {
                { "Project", guid },
                { "Name", name }
            };

            if(makeRelative) {
                path = GetRelativePath(path);
            }
            return AddItem(PRJ_REF, path, meta);
        }

        /// <summary>
        /// Adds 'PackageReference' item.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <param name="version">Package version: 2.5; 1.6.0-beta3; ...</param>
        /// <param name="meta">Optional metadata, eg. ExcludeAssets="runtime" etc.</param>
        /// <returns></returns>
        public bool AddPackageReference(string id, string version, IEnumerable<KeyValuePair<string, string>> meta = null) => AddItem
        (
            PKG_REF, 
            id, 
            new Dictionary<string, string>()
            {
                { "Version", version }
            }
            .AddOrUpdate(meta)
        );

        /// <summary>
        /// Adds an item to the project.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The Include attribute of this item.</param>
        /// <param name="meta">Optional metadata list.</param>
        /// <returns>true if item has been added.</returns>
        public bool AddItem(string type, string inc, IEnumerable<KeyValuePair<string, string>> meta = null)
        {
            var items = Project.AddItem(type, inc, meta);
            return items?.Count > 0;
        }

        /// <summary>
        /// Retrieve all available items from projects.
        /// </summary>
        /// <param name="type">The item type or null value to get all.</param>
        /// <param name="inc">The unevaluated value of the Include attribute or null value to get all.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetItems(string type = null, string inc = null)
        {
            bool hasType    = !string.IsNullOrWhiteSpace(type);
            bool hasInc     = !string.IsNullOrWhiteSpace(inc);

            if(!hasType && !hasInc) {
                return Project.Items.Select(i => GetItem(i));
            }

            if(hasType && !hasInc) {
                return Project.Items.Where(i => i.ItemType == type).Select(i => GetItem(i));
            }

            if(!hasType && hasInc) {
                return Project.Items.Where(i => i.UnevaluatedInclude == inc).Select(i => GetItem(i));
            }

            return Project.Items.Where(i => i.ItemType == type && i.UnevaluatedInclude == inc)
                                .Select(i => GetItem(i));
        }

        /// <summary>
        /// Retrieve first item by type.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public Item GetItem(string type, string inc) => GetItems(type, inc).FirstOrDefault();

        /// <summary>
        /// Get all available 'Reference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetReferences(string inc = null) => GetItems(ITEM_REF, inc);

        /// <summary>
        /// Get all available 'ProjectReference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetProjectReferences(string inc = null) => GetItems(PRJ_REF, inc);

        public IEnumerable<Item> GetPackageReferences() => GetItems(PKG_REF);

        /// <summary>
        /// Get first available 'Reference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        public Item GetFirstReference(string inc) => GetReferences(inc).FirstOrDefault();

        /// <summary>
        /// Get first available 'ProjectReference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        public Item GetFirstProjectReference(string inc) => GetProjectReferences(inc).FirstOrDefault();

        /// <summary>
        /// Get first available 'PackageReference' item.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <returns></returns>
        public Item GetFirstPackageReference(string id) => GetItems(PKG_REF, id).FirstOrDefault();

        /// <summary>
        /// Remove first item from project by type.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveItem(string type, string inc) => RemoveItem(GetItem(type, inc));

        /// <summary>
        /// Remove selected item from project.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(Item item)
        {
            if(item.parentItem == null) {
                return false;
            }
            return Project.RemoveItem(item.parentItem);
        }

        /// <summary>
        /// Remove 'Reference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveReference(string inc) => RemoveItem(GetFirstReference(inc));

        /// <summary>
        /// Remove 'ProjectReference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveProjectReference(string inc) => RemoveItem(GetFirstProjectReference(inc));

        /// <summary>
        /// Remove 'PackageReference' item from project.
        /// </summary>
        /// <param name="id">Package id: `MvsSln`; `Conari`; ...</param>
        /// <returns></returns>
        public bool RemovePackageReference(string id) => RemoveItem(GetFirstPackageReference(id));

        public XProject()
            : this(new Project())
        {

        }

        public XProject(string file)
            : this(new Project(file))
        {

        }

        public XProject(string file, IDictionary<string, string> properties)
            : this(new Project(file, properties, null))
        {

        }

        public XProject(Project prj)
            : this(new ProjectItemCfg(), prj)
        {

        }

        public XProject(ProjectItemCfg pItem, Project prj)
            : this(null, pItem, prj)
        {

        }

        public XProject(ISlnResult data, ProjectItemCfg pItem, Project prj)
        {
            Sln         = data;
            ProjectItem = pItem;
            Project     = prj ?? throw new ArgumentNullException(nameof(prj), MsgResource.ValueNoEmptyOrNull);
            PId         = CalculatePId(prj);
        }

        protected Guid CalculatePId(Project prj)
        {
            if(Project == null) {
                return Guid.Empty;
            }

            return (
                FindGuid(prj)
                    + ProjectItem.projectConfig 
                    + ProjectItem.solutionConfig
            )
            .Guid();
        }

        protected virtual string GetProjectGuid(Project eProject)
        {
            //eProject.GetProjectGuid(); - null by default for all SDK-based projects
            return FindGuid(eProject);
        }

        protected virtual string GetProjectName(Project eProject)
        {
            return eProject.GetProjectName().NullIfEmpty() ?? ProjectItem.project.name;
        }

        protected PropertyItem GetProperty(ProjectProperty eProperty)
        {
            return (eProperty == null) ? default : new PropertyItem(eProperty) { parentProject = this };
        }

        protected Item GetItem(Microsoft.Build.Evaluation.ProjectItem eItem)
        {
            return (eItem == null) ? default : new Item(eItem) { parentProject = this };
        }

        protected ImportElement GetImportElement(ProjectImportElement element)
        {
            return (element == null) ? default : new ImportElement(element) { parentProject = this };
        }

        protected bool AddImport(ProjectImportElement element, string condition, string label)
        {
            if(element == null) {
                return false;
            }

            if(condition != null) {
                element.Condition = condition;
            }

            if(label != null) {
                element.Label = label;
            }

            return true;
        }

        private string FindGuid(Project eProject) => eProject?.GetProjectGuid().NullIfEmpty() ?? ProjectItem.project.pGuid;

        #region DebuggerDisplay

        private string DbgDisplay => $"{ProjectName}: [{ProjectItem.projectConfig}] {ProjectGuid}";

        #endregion
    }
}