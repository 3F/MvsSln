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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Properties of project in solution file
    /// </summary>
    [DebuggerDisplay("{DbgDisplay}")]
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

        /// <summary>
        /// Contains parent item or null if it's a root element.
        /// </summary>
        public RefType<SolutionFolder?> parent;

        /// <summary>
        /// Evaluated project type.
        /// </summary>
        public ProjectType EpType;

        /// <summary>
        /// Evaluate project type via Guid.
        /// </summary>
        /// <param name="guid">Project type Guid.</param>
        /// <returns></returns>
        public static ProjectType ProjectTypeBy(string guid)
        {
            switch(guid)
            {
                case Guids.PROJECT_CS: {
                    return ProjectType.Cs;
                }
                case Guids.PROJECT_DB: {
                    return ProjectType.Db;
                }
                case Guids.PROJECT_FS: {
                    return ProjectType.Fs;
                }
                case Guids.PROJECT_VB: {
                    return ProjectType.Vb;
                }
                case Guids.PROJECT_VC: {
                    return ProjectType.Vc;
                }
                case Guids.PROJECT_VJ: {
                    return ProjectType.Vj;
                }
                case Guids.PROJECT_WD: {
                    return ProjectType.Wd;
                }
                case Guids.PROJECT_WEB: {
                    return ProjectType.Web;
                }
                case Guids.SLN_FOLDER: {
                    return ProjectType.SlnFolder;
                }
            }
            return ProjectType.Unknown;
        }

        /// <param name="line">Initialize data from raw line.</param>
        /// <param name="solutionDir">Path to solution directory.</param>
        public ProjectItem(string line, string solutionDir)
            : this()
        {
            Match m = RPatterns.ProjectLine.Match(line ?? String.Empty);
            if(!m.Success) {
                LSender.Send(this, $"ProjectItem: incorrect line :: '{line}'", Message.Level.Warn);
                return;
            }

            pType   = m.Groups["TypeGuid"].Value.Trim();
            name    = m.Groups["Name"].Value.Trim();
            path    = m.Groups["Path"].Value.Trim();
            pGuid   = m.Groups["Guid"].Value.Trim();
            EpType  = ProjectTypeBy(pType);

            if(Path.IsPathRooted(path)) {
                fullPath = path;
            }
            else {
                fullPath = (solutionDir != null && path != null) ? Path.Combine(solutionDir, path) : path;
            }
            fullPath = Path.GetFullPath(fullPath); // D:\a\b\c\..\..\MvsSlnTest.csproj -> D:\a\MvsSlnTest.csproj

            LSender.Send(this, $"ProjectItem ->['{pGuid}'; '{name}'; '{path}'; '{fullPath}'; '{pType}' ]", Message.Level.Trace);
            parent = new RefType<SolutionFolder?>();
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{name} [^{parent?.Value?.header.name}] [{pGuid}] = {path}";
        }

        #endregion
    }
}