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
using net.r_eg.MvsSln.Extensions;

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
        public string unevaluated;

        /// <summary>
        /// The evaluated value of the Include attribute.
        /// </summary>
        public string evaluated;

        /// <summary>
        /// True if this item originates from an imported file.
        /// </summary>
        public bool isImported;

        /// <summary>
        /// All the metadata for this item by name.
        /// </summary>
        public RoProperties<string, Metadata> meta;

        public readonly struct Metadata(string name, string evaluated, string unevaluated)
        {
            /// <summary>
            /// The name of the metadata.
            /// </summary>
            public readonly string name = name;

            /// <summary>
            /// The evaluated metadata value.
            /// </summary>
            public readonly string evaluated = evaluated;

            /// <summary>
            /// The unevaluated metadata value.
            /// </summary>
            public readonly string unevaluated = unevaluated;

            public static bool operator ==(Metadata a, Metadata b) => a.Equals(b);

            public static bool operator !=(Metadata a, Metadata b) => !(a == b);

            public override readonly bool Equals(object obj)
            {
                if(obj is null || obj is not Metadata b) return false;

                return name == b.name
                    && evaluated == b.evaluated
                    && unevaluated == b.unevaluated;
            }

            public override readonly int GetHashCode() => 0.CalculateHashCode
            (
                name,
                evaluated,
                unevaluated
            );
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
        public readonly AsmData Assembly => MakeAssemblyInfo(evaluated ?? unevaluated);

        public sealed class AsmData(AssemblyName asm = null)
        {
            public AssemblyName Info { get; } = asm;

            public string PublicKeyToken { get; } = asm?.GetPublicKeyToken().ToHexString();
        }

        public static bool operator ==(Item a, Item b) => a.Equals(b);

        public static bool operator !=(Item a, Item b) => !(a == b);

        public override readonly bool Equals(object obj)
        {
            if(obj is null || obj is not Item b) return false;

            return type == b.type
                && unevaluated == b.unevaluated
                && evaluated == b.evaluated
                && isImported == b.isImported
                && meta == b.meta;
        }

        public override readonly int GetHashCode() => 0.CalculateHashCode
        (
            type,
            unevaluated,
            evaluated,
            isImported,
            meta,
            parentItem,
            parentProject
        );

        public Item(string evaluated, RoProperties<string, Metadata> meta = null, IXProject parentProject = null)
            : this(evaluated, evaluated, meta, parentProject)
        {

        }

        public Item(string unevaluated, string evaluated, string type, RoProperties<string, Metadata> meta = null, IXProject parentProject = null)
        {
            this.unevaluated = unevaluated;
            this.evaluated = evaluated;
            this.type = type;
            this.meta = meta;
            this.parentProject = parentProject;
        }

        public Item(string unevaluated, string evaluated, RoProperties<string, Metadata> meta = null, IXProject parentProject = null)
            : this(unevaluated, evaluated, type: null, meta, parentProject)
        {
            
        }

        public Item(Microsoft.Build.Evaluation.ProjectItem eItem, IXProject parentProject)
            : this(eItem)
        {
            this.parentProject = parentProject ?? throw new ArgumentNullException(nameof(parentProject));
        }

        public Item(Microsoft.Build.Evaluation.ProjectItem eItem)
            : this()
        {
            parentItem      = eItem ?? throw new ArgumentNullException(nameof(eItem));
            type            = eItem.ItemType;
            unevaluated     = eItem.UnevaluatedInclude;
            evaluated       = eItem.EvaluatedInclude;
            isImported      = eItem.IsImported;

            meta = eItem.DirectMetadata.Select(m => new KeyValuePair<string, Metadata>
            (
                m.Name,
                new Metadata(m.Name, m.EvaluatedValue, m.UnevaluatedValue) 
            ))
            .ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        private static AsmData MakeAssemblyInfo(string name)
        {
            if(string.IsNullOrWhiteSpace(name) || name.IndexOfAny(['\\', '/']) != -1)
            {
                return new();
            }
            return new AsmData(new AssemblyName(name));
        }

        #region Obsolete fields
        #pragma warning disable IDE1006

        [Obsolete("Renamed as " + nameof(unevaluated))]
        public readonly string unevaluatedValue => unevaluated;

        [Obsolete("Renamed as " + nameof(evaluated))]
        public readonly string evaluatedValue => evaluated;

        #pragma warning restore IDE1006
        #endregion

        #region DebuggerDisplay

        private readonly string DbgDisplay => $"{type} = {evaluated} [{unevaluated}]";

        #endregion
    }
}