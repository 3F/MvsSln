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
using System.Diagnostics;
using System.IO;
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
        [Obsolete("Use `Guids.ProjectTypeBy(string guid)` instead.", false)]
        public static ProjectType ProjectTypeBy(string guid)
        {
            return Guids.ProjectTypeBy(guid);
        }

        public ProjectItem(string name, ProjectType pType)
            : this(name, name, pType)
        {

        }

        public ProjectItem(string pGuid, string name, ProjectType pType)
            : this(pGuid, name, name, pType)
        {

        }

        public ProjectItem(string name, string path, ProjectType pType, string slnDir = null)
            : this(Guid.NewGuid().ToString(), name, path, pType, slnDir)
        {

        }

        public ProjectItem(string pGuid, string name, string path, ProjectType pType, string slnDir = null)
            : this()
        {
            Init(pGuid, name, path, pType, slnDir);
        }

        public ProjectItem(string pGuid, string name, string path, string pType, string slnDir = null)
            : this()
        {
            Init(pGuid, name, path, pType, slnDir);
        }

        /// <param name="raw">Initialize data from raw line.</param>
        /// <param name="solutionDir">Path to solution directory.</param>
        public ProjectItem(string raw, string solutionDir)
            : this()
        {
            Match m = RPatterns.ProjectLine.Match(raw ?? String.Empty);
            if(!m.Success) {
                LSender.Send(this, $"ProjectItem: incorrect line :: '{raw}'", Message.Level.Warn);
                return;
            }

            Init
            (
                m.Groups["Guid"].Value,
                m.Groups["Name"].Value,
                m.Groups["Path"].Value,
                m.Groups["TypeGuid"].Value,
                solutionDir
            );
        }

        private void Init(string pGuid, string name, string path, ProjectType pType, string slnDir)
        {
            SetProjectType(pType);
            Init(pGuid, name, path, slnDir);
        }

        private void Init(string pGuid, string name, string path, string pType, string slnDir)
        {
            SetProjectType(pType);
            Init(pGuid, name, path, slnDir);
        }

        private void Init(string pGuid, string name, string path, string slnDir)
        {
            this.name   = name.Trim();
            this.path   = path.Trim();
            this.pGuid  = pGuid.Trim();

            SetFullPath(slnDir);

            LSender.Send(this, $"ProjectItem ->['{pGuid}'; '{name}'; '{path}'; '{fullPath}'; '{pType}' ]", Message.Level.Trace);
            parent = new RefType<SolutionFolder?>();
        }

        private void SetProjectType(ProjectType pType)
        {
            this.pType  = Guids.GuidBy(pType);
            EpType      = pType;
        }

        /// <summary>
        /// We reserved raw type for future new Guids before our updates.
        /// </summary>
        /// <param name="pType"></param>
        private void SetProjectType(string pType)
        {
            this.pType  = pType;
            EpType      = Guids.ProjectTypeBy(pType);
        }

        private void SetFullPath(string slnDir)
        {
            if(slnDir == null) {
                return;
            }

            if(Path.IsPathRooted(path)) {
                fullPath = path;
            }
            else {
                fullPath = (slnDir != null && path != null) ? Path.Combine(slnDir, path) : path;
            }

            fullPath = Path.GetFullPath(fullPath); // D:\a\b\c\..\..\MvsSlnTest.csproj -> D:\a\MvsSlnTest.csproj
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{name} [^{parent?.Value?.header.name}] [{pGuid}] = {path}";
        }

        #endregion
    }
}