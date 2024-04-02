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
        /// Value of <see cref="evaluatedInclude"/> if not null, otherwise <see cref="unevaluatedInclude"/>
        /// </summary>
        public readonly string Include => evaluatedInclude ?? unevaluatedInclude;

        /// <summary>
        /// Try to extract assembly information, e.g.:
        /// Include="DllExport, Version=1.5.1.35977, Culture=neutral, PublicKeyToken=8337224c9ad9e356, processorArchitecture=MSIL"
        /// Include="System.Core"
        /// ...
        /// </summary>
        public readonly AsmData Assembly => MakeAssemblyInfo(Include);

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
                && unevaluatedInclude == b.unevaluatedInclude
                && evaluatedInclude == b.evaluatedInclude
                && isImported == b.isImported
                && meta == b.meta;
        }

        public override readonly int GetHashCode() => 0.CalculateHashCode
        (
            type,
            unevaluatedInclude,
            evaluatedInclude,
            isImported,
            meta,
            parentItem,
            parentProject
        );

        public Item(Microsoft.Build.Evaluation.ProjectItem eItem)
            : this()
        {
            if(eItem == null) throw new ArgumentNullException(nameof(eItem));

            type                = eItem.ItemType;
            unevaluatedInclude  = eItem.UnevaluatedInclude;
            evaluatedInclude    = eItem.EvaluatedInclude;
            isImported          = eItem.IsImported;
            parentItem          = eItem;

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

        #region DebuggerDisplay

        private readonly string DbgDisplay => $"{type} = {evaluatedInclude} [{unevaluatedInclude}]";

        #endregion
    }
}