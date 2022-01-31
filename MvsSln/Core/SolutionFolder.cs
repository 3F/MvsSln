/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using net.r_eg.MvsSln.Extensions;

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

        public static bool operator ==(SolutionFolder a, SolutionFolder b) => a.Equals(b);

        public static bool operator !=(SolutionFolder a, SolutionFolder b) => !(a == b);

        /// <summary>
        /// Elements will not compared.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is SolutionFolder)) {
                return false;
            }

            return header == ((SolutionFolder)obj).header;
        }

        public override int GetHashCode()
        {
            return header.GetHashCode();
        }

        /// <param name="fGuid">Not null Folder GUID.</param>
        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string fGuid, string name, IEnumerable<RawText> items)
            : this(fGuid, name, null, items)
        {

        }

        /// <param name="fGuid">Not null Folder GUID.</param>
        /// <param name="name">Not null Solution folder name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(Guid fGuid, string name)
            : this(fGuid.SlnFormat(), name, null, null)
        {

        }

        /// <param name="fGuid">Not null Folder GUID.</param>
        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string fGuid, string name, SolutionFolder parent, params RawText[] items)
            : this(fGuid, name, parent, items?.AsEnumerable())
        {

        }

        /// <param name="fGuid">Not null Folder GUID.</param>
        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string fGuid, string name, SolutionFolder? parent, IEnumerable<RawText> items)
            : this
            (   new ProjectItem
                (
                    fGuid ?? throw new ArgumentNullException(), 
                    name ?? throw new ArgumentNullException(), 
                    ProjectType.SlnFolder, 
                    parent
                ),
                items
            )
        {

        }

        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string name, params RawText[] items)
            : this(name, items?.AsEnumerable())
        {

        }

        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string name, IEnumerable<RawText> items)
            : this
            (   new ProjectItem
                (
                    name ?? throw new ArgumentNullException(), 
                    ProjectType.SlnFolder
                ), 
                items
            )
        {

        }

        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string name, SolutionFolder parent, params RawText[] items)
            : this(name, parent, items?.AsEnumerable())
        {

        }

        /// <param name="name">Not null Solution folder name.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="items">Optional items inside.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SolutionFolder(string name, SolutionFolder parent, IEnumerable<RawText> items)
            : this
            (   new ProjectItem
                (
                    name ?? throw new ArgumentNullException(), 
                    ProjectType.SlnFolder, 
                    parent
                ),
                items
            )
        {

        }

        /// <param name="pItem">Information about folder.</param>
        /// <param name="def">List of items for this folder.</param>
        public SolutionFolder(ProjectItem pItem, params RawText[] def)
            : this(pItem, def?.AsEnumerable())
        {

        }

        /// <param name="pItem">Information about folder.</param>
        /// <param name="def">List of items for this folder.</param>
        public SolutionFolder(ProjectItem pItem, IEnumerable<RawText> def)
            : this()
        {
            header  = pItem;
            items   = def ?? new List<RawText>();
        }

        /// <param name="folder">Initialize data from other folder.</param>
        public SolutionFolder(SolutionFolder folder)
        {
            header  = folder.header;
            items   = folder.items;
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{header.name} [^{header.parent?.Value?.header.name}] = {items?.Count()} [{header.pGuid}]";
        }

        #endregion
    }
}