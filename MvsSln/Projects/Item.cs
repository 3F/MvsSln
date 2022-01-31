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
using System.Reflection;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{DbgDisplay}")]
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

        /// <summary>
        /// Try to extract assembly information, e.g.:
        /// Include="DllExport, Version=1.5.1.35977, Culture=neutral, PublicKeyToken=8337224c9ad9e356, processorArchitecture=MSIL"
        /// Include="System.Core"
        /// ...
        /// </summary>
        public AsmData Assembly
        {
            get
            {
                string name = evaluatedInclude ?? unevaluatedInclude;
                if(String.IsNullOrWhiteSpace(name) || name.IndexOfAny(new[] { '\\', '/' }) != -1) {
                    return default(AsmData);
                }
                return new AsmData(new AssemblyName(name));
            }
        }

        public struct AsmData
        {
            public AssemblyName Info
            {
                get;
                private set;
            }

            public string PublicKeyToken
            {
                get;
                private set;
            }

            public AsmData(AssemblyName asm)
                : this()
            {
                Info = asm;

                byte[] data = asm?.GetPublicKeyToken();
                if(data == null || data.Length < 1) {
                    return;
                }

                PublicKeyToken = String.Empty;
                foreach(var b in data) {
                    PublicKeyToken += b.ToString("x");
                }
            }
        }

        /// <param name="eItem"></param>
        public Item(Microsoft.Build.Evaluation.ProjectItem eItem)
            : this()
        {
            if(eItem == null) {
                throw new ArgumentNullException(nameof(eItem));
            }

            type                = eItem.ItemType;
            unevaluatedInclude  = eItem.UnevaluatedInclude;
            evaluatedInclude    = eItem.EvaluatedInclude;
            isImported          = eItem.IsImported;
            parentItem          = eItem;

            meta = eItem.DirectMetadata.Select(m => new KeyValuePair<string, Metadata>
            (
                m.Name,
                new Metadata() 
                {
                    name = m.Name,
                    unevaluated = m.UnevaluatedValue,
                    evaluated = m.EvaluatedValue
                }
            )).ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{type} = {evaluatedInclude} [{unevaluatedInclude}]";
        }

        #endregion
    }
}