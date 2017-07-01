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
        /// <param name="unevaluatedValue">The new unevaluated value of the property.</param>
        /// <returns></returns>
        PropertyItem SetProperty(string name, string unevaluatedValue);

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
    }
}