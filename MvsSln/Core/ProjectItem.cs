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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Properties of project in solution file
    /// </summary>
    [DebuggerDisplay("{name} [{pGuid}] = {path}")]
    public struct ProjectItem
    {
        /// <summary>
        /// Project GUID
        /// </summary>
        public string pGuid;

        /// <summary>
        /// Project type GUID
        /// </summary>
        public string pType;

        /// <summary>
        /// Project name
        /// </summary>
        public string name;

        /// <summary>
        /// Relative path to project
        /// </summary>
        public string path;

        /// <summary>
        /// Full path to project 
        /// </summary>
        public string fullPath;

        /// <param name="line">Initialize data from raw line.</param>
        /// <param name="solutionDir">Path to solution directory.</param>
        public ProjectItem(string line, string solutionDir)
            : this()
        {
            Match m = RPatterns.ProjectLine.Match(line);
            if(!m.Success) {
                LSender.Send(this, $"ProjectItem: incorrect line :: '{line}'", Message.Level.Warn);
                return;
            }

            pType   = m.Groups["TypeGuid"].Value.Trim();
            name    = m.Groups["Name"].Value.Trim();
            path    = m.Groups["Path"].Value.Trim();
            pGuid   = m.Groups["Guid"].Value.Trim();

            if(Path.IsPathRooted(path)) {
                fullPath = path;
            }
            else {
                fullPath = (!String.IsNullOrEmpty(path))? Path.Combine(solutionDir, path) : path;
            }

            LSender.Send(this, $"ProjectItem ->['{pGuid}'; '{name}'; '{path}'; '{fullPath}'; '{pType}' ]", Message.Level.Trace);
        }
    }
}