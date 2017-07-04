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

using System.Collections.Generic;
using Microsoft.Build.Construction;
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
        /// The Guid of this project.
        /// </summary>
        string ProjectGuid { get; }

        /// <summary>
        /// The ProjectName of this project.
        /// </summary>
        string ProjectName { get; }

        /// <summary>
        /// Saves the project to the file system, if modified.
        /// </summary>
        void Save();

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="checking">To check existence of target via 'Condition' attr.</param>
        /// <returns>true value if target has been added.</returns>
        bool AddImport(string target, bool checking);

        /// <summary>
        /// To add 'import' element.
        /// It will be added only if target does not exist.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="condition">Use 'Condition' attr.</param>
        /// <returns>true value if target has been added.</returns>
        bool AddImport(string target, string condition);

        /// <summary>
        /// To remove selected 'import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <returns>true value if target has been found and removed.</returns>
        bool RemoveImport(string target);

        /// <summary>
        /// Retrieve selected target from 'import' tags if it exists.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        ProjectImportElement GetImport(string target);

        /// <summary>
        /// The property in this project that has the specified name.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>null if no property of that name exists.</returns>
        PropertyItem GetProperty(string name);

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
        /// <returns></returns>
        bool RemoveProperty(string name);

        /// <summary>
        /// Removes an property from the project.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool RemoveProperty(PropertyItem property);

        /// <summary>
        /// All properties in this project.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyItem> GetProperties();

        /// <summary>
        /// Adds 'Reference' item.
        /// </summary>
        /// <param name="inc">Include attribute.</param>
        /// <returns></returns>
        bool AddReference(string inc);

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
        /// <param name="path">Path to project file.</param>
        /// <param name="guid">The Guid of project.</param>
        /// <param name="name">The name of project.</param>
        /// <returns></returns>
        bool AddProjectReference(string path, string guid, string name);

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
    }
}