/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using net.r_eg.MvsSln.Extensions;
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
        /// Project GUID.
        /// </summary>
        public string pGuid;

        /// <summary>
        /// Project type GUID.
        /// </summary>
        public string pType;

        /// <summary>
        /// Project name.
        /// </summary>
        public string name;

        /// <summary>
        /// Relative path to project.
        /// </summary>
        public string path;

        /// <summary>
        /// Evaluated full path to project.
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
        /// Evaluate project type via GUID.
        /// </summary>
        /// <param name="guid">Project type GUID.</param>
        /// <returns></returns>
        [Obsolete("Use `Guids.ProjectTypeBy(string guid)` instead.", false)]
        public static ProjectType ProjectTypeBy(string guid)
        {
            return Guids.ProjectTypeBy(guid);
        }

        public static bool operator ==(ProjectItem a, ProjectItem b) => a.Equals(b);

        public static bool operator !=(ProjectItem a, ProjectItem b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is ProjectItem)) {
                return false;
            }

            var b = (ProjectItem)obj;

            return pGuid == b.pGuid
                    && pType == b.pType
                    && name == b.name
                    && path == b.path
                    && fullPath == b.fullPath
                    && EpType == b.EpType
                    && parent == b.parent;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                pGuid,
                pType,
                name,
                path,
                fullPath,
                EpType,
                parent
            );
        }

        /// <param name="name">Project name.</param>
        /// <param name="pType">Project type GUID.</param>
        /// <param name="parent">Parent folder.</param>
        public ProjectItem(string name, ProjectType pType, SolutionFolder? parent = null)
            : this(name, pType, name, parent)
        {

        }

        /// <param name="name">Project name.</param>
        /// <param name="pType">Project type GUID.</param>
        /// <param name="path"></param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="slnDir">To evaluate `fullPath` define path to solution directory.</param>
        public ProjectItem(string name, ProjectType pType, string path, SolutionFolder? parent = null, string slnDir = null)
            : this(Guid.NewGuid().SlnFormat(), name, pType, path, parent, slnDir)
        {

        }

        /// <param name="pGuid">Project GUID.</param>
        /// <param name="name">Project name.</param>
        /// <param name="pType">Project type GUID.</param>
        /// <param name="parent">Parent folder.</param>
        public ProjectItem(string pGuid, string name, ProjectType pType, SolutionFolder? parent = null)
            : this(pGuid, name, pType, name, parent)
        {

        }

        /// <param name="pGuid">Project GUID.</param>
        /// <param name="name">Project name.</param>
        /// <param name="pType">Project type GUID.</param>
        /// <param name="path">Relative path to project.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="slnDir">To evaluate `fullPath` define path to solution directory.</param>
        public ProjectItem(string pGuid, string name, ProjectType pType, string path, SolutionFolder? parent = null, string slnDir = null)
            : this()
        {
            Init(pGuid, name, path, pType, parent, slnDir);
        }

        /// <param name="pGuid">Project GUID.</param>
        /// <param name="name">Project name.</param>
        /// <param name="path">Relative path to project.</param>
        /// <param name="pType">Project type GUID.</param>
        /// <param name="slnDir">To evaluate `fullPath` define path to solution directory.</param>
        public ProjectItem(string pGuid, string name, string path, string pType, string slnDir = null)
            : this()
        {
            Init(pGuid, name, path, pType, slnDir);
        }

        /// <param name="prj">Initialize data from other project.</param>
        public ProjectItem(ProjectItem prj)
        {
            pGuid       = prj.pGuid.ReformatSlnGuid();
            pType       = prj.pType.ReformatSlnGuid();
            name        = prj.name;
            path        = prj.path;
            fullPath    = prj.fullPath;
            EpType      = prj.EpType;
            parent      = prj.parent;
        }

        /// <param name="raw">Initialize data from raw line.</param>
        /// <param name="solutionDir">To evaluate `fullPath` define path to solution directory.</param>
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

        private void Init(string pGuid, string name, string path, ProjectType pType, SolutionFolder? parent, string slnDir)
        {
            SetProjectType(pType);
            SetFields(pGuid, name, path, slnDir, parent);
        }

        private void Init(string pGuid, string name, string path, string pType, string slnDir)
        {
            SetProjectType(pType);
            SetFields(pGuid, name, path, slnDir);
        }

        private void SetFields(string pGuid, string name, string path, string slnDir, SolutionFolder? parent = null)
        {
            this.name   = name?.Trim();
            this.path   = path?.Trim();
            this.pGuid  = pGuid.ReformatSlnGuid();

            SetFullPath(slnDir);
            LSender.Send(this, $"ProjectItem ->['{pGuid}'; '{name}'; '{path}'; '{fullPath}'; '{pType}' ]", Message.Level.Trace);

            this.parent = new RefType<SolutionFolder?>(parent);
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
            this.pType  = pType.ReformatSlnGuid();
            EpType      = Guids.ProjectTypeBy(pType);
        }

        private void SetFullPath(string slnDir)
        {
            if(string.IsNullOrWhiteSpace(path)) return;

            if(Path.IsPathRooted(path)) {
                fullPath = path;
            }
            else {
                fullPath = (slnDir != null) ? Path.Combine(slnDir, path) : path;
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