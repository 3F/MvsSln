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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{ProjectName}: [{ProjectItem.projectConfig}] {ProjectGuid}")]
    public class XProject: IXProject
    {
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
        /// The Guid of this project.
        /// </summary>
        public string ProjectGuid
        {
            get => GetProjectGuid(Project);
        }

        /// <summary>
        /// The ProjectName of this project.
        /// </summary>
        public string ProjectName
        {
            get => GetProjectName(Project);
        }

        /// <summary>
        /// Access to global properties of project.
        /// </summary>
        public IDictionary<string, string> GlobalProperties
        {
            get => Project.GlobalProperties;
        }

        /// <summary>
        /// Saves the project to the file system, if modified.
        /// </summary>
        public void Save()
        {
            //TODO: EnvDTE because of ~"... has been modified outside the environment."
            Project.Save();
        }

        /// <summary>
        /// To add 'import' element.
        /// It will be added only if target does not exist.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="checking">To check existence of target via 'Condition' attr.</param>
        /// <returns>true value if target has been added.</returns>
        public bool AddImport(string target, bool checking)
        {
            return AddImport(target, checking ? $"Exists('{target}')" : null);
        }

        /// <summary>
        /// To add 'import' element.
        /// It will be added only if target does not exist.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="condition">Use 'Condition' attr.</param>
        /// <returns>true value if target has been added.</returns>
        public bool AddImport(string target, string condition)
        {
            if(String.IsNullOrWhiteSpace(target)) {
                return false;
            }

            var element = (GetImport(target) != null) ? null : Project.Xml.AddImport(target);
            if(element == null) {
                return false;
            }

            if(condition != null) {
                element.Condition = condition;
            }
            return true;
        }

        /// <summary>
        /// To remove selected 'import' element if exists.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <returns>true value if target has been found and removed.</returns>
        public bool RemoveImport(string target)
        {
            var element = GetImport(target);
            if(element == null) {
                return false;
            }

            Project.Xml.RemoveChild(element);
            return true;
        }

        /// <summary>
        /// Retrieve first selected target from 'import' tags if it exists.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ProjectImportElement GetImport(string target)
        {
            if(String.IsNullOrWhiteSpace(target)) {
                return null;
            }

            return Project.Xml.Imports.Where(i => i.Project == target).FirstOrDefault();
        }

        /// <summary>
        /// The property in this project that has the specified name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>null if no property of that name exists.</returns>
        public PropertyItem GetProperty(string name)
        {
            return GetProperty(Project.GetProperty(name));
        }

        /// <summary>
        /// Sets or adds a property with the given name and unevaluated value to the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluated">The new unevaluated value of the property.</param>
        /// <returns></returns>
        public PropertyItem SetProperty(string name, string unevaluated)
        {
            return SetProperty(name, unevaluated, null);
        }

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
            if(condition != null) {
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
        /// Removes an property from the project.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns></returns>
        public bool RemoveProperty(string name)
        {
            return RemoveProperty(GetProperty(name));
        }

        /// <summary>
        /// Removes an property from the project.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool RemoveProperty(PropertyItem property)
        {
            return Project.RemoveProperty(property.parentProperty);
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
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <returns></returns>
        public bool AddReference(string inc)
        {
            return AddItem("Reference", inc);
        }

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
            return AddReference(asm.ToString(), Sln.SolutionDir.MakeRelativePath(asm.Location), local, embed, spec);
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

            return AddItem("Reference", inc, meta);
        }

        /// <summary>
        /// Adds 'ProjectReference' item.
        /// </summary>
        /// <param name="project">Information about project.</param>
        /// <returns></returns>
        public bool AddProjectReference(ProjectItem project)
        {
            var path = Sln.SolutionDir.MakeRelativePath(project.fullPath) ?? project.path;
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
                path = Sln.SolutionDir.MakeRelativePath(path);
            }
            return AddItem("ProjectReference", path, meta);
        }

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
            bool hasType    = !String.IsNullOrWhiteSpace(type);
            bool hasInc     = !String.IsNullOrWhiteSpace(inc);

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
        public Item GetItem(string type, string inc)
        {
            return GetItems(type, inc).FirstOrDefault();
        }

        /// <summary>
        /// Get all available 'Reference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetReferences(string inc = null)
        {
            return GetItems("Reference", inc);
        }

        /// <summary>
        /// Get all available 'ProjectReference' items.
        /// </summary>
        /// <param name="inc">The Include attribute to be found or null value to get all.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetProjectReferences(string inc = null)
        {
            return GetItems("ProjectReference", inc);
        }

        /// <summary>
        /// Get first available 'Reference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        public Item GetFirstReference(string inc)
        {
            return GetReferences(inc).FirstOrDefault();
        }

        /// <summary>
        /// Get first available 'ProjectReference' item.
        /// </summary>
        /// <param name="inc">The Include attribute to be found.</param>
        /// <returns></returns>
        public Item GetFirstProjectReference(string inc)
        {
            return GetProjectReferences(inc).FirstOrDefault();
        }

        /// <summary>
        /// Remove first item from project by type.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveItem(string type, string inc)
        {
            return RemoveItem(GetItem(type, inc));
        }

        /// <summary>
        /// Remove selected item from project.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(Item item)
        {
            return Project.RemoveItem(item.parentItem);
        }

        /// <summary>
        /// Remove 'Reference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveReference(string inc)
        {
            return RemoveItem(GetFirstReference(inc));
        }

        /// <summary>
        /// Remove 'ProjectReference' item from project.
        /// </summary>
        /// <param name="inc">The unevaluated value of the Include attribute.</param>
        /// <returns></returns>
        public bool RemoveProjectReference(string inc)
        {
            return RemoveItem(GetFirstProjectReference(inc));
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
        }

        protected virtual string GetProjectGuid(Project eProject)
        {
            return eProject.GetProjectGuid();
        }

        protected virtual string GetProjectName(Project eProject)
        {
            return eProject.GetProjectName();
        }

        protected PropertyItem GetProperty(ProjectProperty eProperty)
        {
            if(eProperty == null) {
                return default(PropertyItem);
            }

            return new PropertyItem() {
                name                    = eProperty.Name,
                evaluatedValue          = eProperty.EvaluatedValue,
                unevaluatedValue        = eProperty.UnevaluatedValue,
                condition               = eProperty.Xml.Condition,
                isEnvironmentProperty   = eProperty.IsEnvironmentProperty,
                isGlobalProperty        = eProperty.IsGlobalProperty,
                isReservedProperty      = eProperty.IsReservedProperty,
                isImported              = eProperty.IsImported,
                parentProperty          = eProperty,
                parentProject           = this,
            };
        }

        protected Item GetItem(Microsoft.Build.Evaluation.ProjectItem eItem)
        {
            if(eItem == null) {
                return default(Item);
            }

            return new Item() {
                type                = eItem.ItemType,
                unevaluatedInclude  = eItem.UnevaluatedInclude,
                evaluatedInclude    = eItem.EvaluatedInclude,
                isImported          = eItem.IsImported,
                parentItem          = eItem,
                parentProject       = this,
                meta = eItem.DirectMetadata
                            .Select(m => 
                                new KeyValuePair<string, Item.Metadata>(
                                    m.Name,
                                    new Item.Metadata() {
                                        name = m.Name,
                                        unevaluated = m.UnevaluatedValue,
                                        evaluated = m.EvaluatedValue
                                    }
                                )
                             ).ToDictionary(m => m.Key, m => m.Value),
            };
        }
    }
}