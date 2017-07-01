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
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Extensions;

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
        /// Saves the project to the file system, if modified.
        /// </summary>
        public void Save()
        {
            //TODO: EnvDTE because of ~"... has been modified outside the environment."
            Project.Save();
        }

        /// <summary>
        /// To add 'import' element.
        /// </summary>
        /// <param name="target">Target project.</param>
        /// <param name="checking">To check existence of target via 'Condition' attr.</param>
        /// <returns>true value if target has been added.</returns>
        public bool AddImport(string target, bool checking)
        {
            if(String.IsNullOrWhiteSpace(target)) {
                return false;
            }

            var element = (GetImport(target) != null) ? null : Project.Xml.AddImport(target);
            if(element == null) {
                return false;
            }

            if(checking) {
                element.Condition = $"Exists('{target}')";
            }
            return true;
        }

        /// <summary>
        /// To remove selected 'import' element.
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
        /// Retrieve selected target from 'import' tags if it exists.
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
        /// <param name="unevaluatedValue">The new unevaluated value of the property.</param>
        /// <returns></returns>
        public PropertyItem SetProperty(string name, string unevaluatedValue)
        {
            return GetProperty(Project.SetProperty(name, unevaluatedValue));
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

        public XProject(ProjectItemCfg pItem, Project prj)
        {
            ProjectItem = pItem;
            Project     = prj ?? throw new ArgumentNullException(nameof(prj), "Value cannot be null.");
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
                isEnvironmentProperty   = eProperty.IsEnvironmentProperty,
                isGlobalProperty        = eProperty.IsGlobalProperty,
                isReservedProperty      = eProperty.IsReservedProperty,
                isImported              = eProperty.IsImported,
                parentProperty          = eProperty,
                parentProject           = this,
            };
        }
    }
}