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
using System.Diagnostics;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{type} = {evaluatedInclude} [{unevaluatedInclude}]")]
    public struct Item
    {
        /// <summary>
        /// The item type.
        /// </summary>
        public string type;

        /// <summary>
        /// The unevaluated value of the Include attribute.
        /// </summary>
        public string unevaluatedInclude;

        /// <summary>
        /// The evaluated value of the Include attribute.
        /// </summary>
        public string evaluatedInclude;

        /// <summary>
        /// True if this item originates from an imported file.
        /// </summary>
        public bool isImported;

        /// <summary>
        /// All the metadata for this item by name.
        /// </summary>
        public RoProperties<string, Metadata> meta;

        public struct Metadata
        {
            /// <summary>
            /// The name of the metadata.
            /// </summary>
            public string name;

            /// <summary>
            /// The evaluated metadata value.
            /// </summary>
            public string evaluated;

            /// <summary>
            /// The unevaluated metadata value.
            /// </summary>
            public string unevaluated;
        }

        /// <summary>
        /// Link to parent Microsoft.Build.Evaluation.ProjectItem.
        /// </summary>
        public Microsoft.Build.Evaluation.ProjectItem parentItem;

        /// <summary>
        /// Link to parent container.
        /// </summary>
        public IXProject parentProject;
    }
}