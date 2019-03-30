/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Linq;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{DbgDisplay}")]
    public struct SolutionFolder
    {
        /// <summary>
        /// Information about folder section.
        /// </summary>
        public ProjectItem header;

        /// <summary>
        /// Available items for this folder.
        /// </summary>
        public IEnumerable<RawText> items;

        /// <param name="pItem">Information about folder.</param>
        /// <param name="def">List of items for this folder.</param>
        public SolutionFolder(ProjectItem pItem, IEnumerable<RawText> def)
        {
            header  = pItem;
            items   = def;
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{header.name} [{header.pGuid}] = {items?.Count()}";
        }

        #endregion
    }
}