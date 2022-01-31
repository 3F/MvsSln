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

        /// <param name="element"></param>
        public ImportElement(ProjectImportElement element)
            : this()
        {
            parentElement   = element ?? throw new ArgumentNullException(nameof(element));

            project         = element.Project;
            condition       = element.Condition;
            label           = element.Label;
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{project} - {label} [{condition}]";
        }

        #endregion
    }
}