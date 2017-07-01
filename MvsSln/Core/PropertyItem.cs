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

using Microsoft.Build.Evaluation;
using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{name} = {evaluatedValue} [{unevaluatedValue}]")]
    public struct PropertyItem
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string name;

        /// <summary>
        /// the evaluated property value, which is never null.
        /// </summary>
        public string evaluatedValue;

        /// <summary>
        /// The unevaluated property value.
        /// </summary>
        public string unevaluatedValue;

        /// <summary>
        /// True if the property originated from an environment variable.
        /// </summary>
        public bool isEnvironmentProperty;

        /// <summary>
        /// True if the property is a global property.
        /// </summary>
        public bool isGlobalProperty;

        /// <summary>
        /// True if the property is a reserved property, for example 'MSBuildProjectFile'.
        /// </summary>
        public bool isReservedProperty;

        /// <summary>
        /// True if the property originates from an imported file 
        /// and not from an environment variable, a global property, or a reserved property.
        /// </summary>
        public bool isImported;

        /// <summary>
        /// Link to Microsoft.Build.Evaluation.ProjectProperty.
        /// </summary>
        public ProjectProperty parentProperty;

        /// <summary>
        /// Link to parent container.
        /// </summary>
        public IXProject parentProject;
    }
}