/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Diagnostics;
using Microsoft.Build.Construction;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{DbgDisplay}")]
    public struct ImportElement
    {
        /// <summary>
        /// The Project attribute.
        /// </summary>
        public string project;

        /// <summary>
        /// The Condition attribute.
        /// </summary>
        public string condition;

        /// <summary>
        /// The Label value.
        /// </summary>
        public string label;

        /// <summary>
        /// Access to parent element.
        /// </summary>
        public ProjectImportElement parentElement;

        /// <summary>
        /// Link to parent container.
        /// </summary>
        public IXProject parentProject;

        public static bool operator ==(ImportElement a, ImportElement b) => a.Equals(b);

        public static bool operator !=(ImportElement a, ImportElement b) => !(a == b);

        public override readonly bool Equals(object obj)
        {
            if(obj is null || obj is not ImportElement b) return false;

            return project == b.project
                && condition == b.condition
                && label == b.label;
        }

        public override readonly int GetHashCode() => 0.CalculateHashCode
        (
            project,
            condition,
            label,
            parentElement,
            parentProject
        );

        public ImportElement(ProjectImportElement element, IXProject parentProject)
            : this(element)
        {
            this.parentProject = parentProject;
        }

        public ImportElement(ProjectImportElement element)
            : this()
        {
            parentElement   = element ?? throw new ArgumentNullException(nameof(element));

            project         = element.Project;
            condition       = element.Condition;
            label           = element.Label;
        }

        #region DebuggerDisplay

        private readonly string DbgDisplay => $"{project} - {label} [{condition}]";

        #endregion
    }
}