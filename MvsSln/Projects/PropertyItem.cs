/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Diagnostics;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{DbgDisplay}")]
    public struct PropertyItem
    {
        public static readonly PropertyItem None;

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string name;

        /// <summary>
        /// The evaluated property value.
        /// </summary>
        public string evaluated;

        /// <summary>
        /// The unevaluated property value.
        /// </summary>
        public string unevaluated;

        /// <summary>
        /// 'Condition' attr if defined.
        /// </summary>
        public string condition;

        /// <summary>
        /// True if the property originated from an environment variable.
        /// </summary>
        public bool isEnvironmentProperty;

        /// <summary>
        /// True if the property is a global property.
        /// </summary>
        public bool isGlobalProperty;

        /// <summary>
        /// True if the property is a reserved property, for example 'MSBuildProjectFile'.
        /// </summary>
        public bool isReservedProperty;

        /// <summary>
        /// True if the property originates from an imported file 
        /// and not from an environment variable, a global property, or a reserved property.
        /// </summary>
        public bool isImported;

        /// <summary>
        /// True if the property has been defined locally by user via available constructor.
        /// </summary>
        public bool isUserDef;

        /// <summary>
        /// Link to Microsoft.Build.Evaluation.ProjectProperty.
        /// </summary>
        public ProjectProperty parentProperty;

        /// <summary>
        /// Link to parent container.
        /// </summary>
        public IXProject parentProject;

        /// <summary>
        /// Check an <see cref="unevaluated"/> for not null.
        /// </summary>
        public readonly bool HasValue => unevaluated != null;

        /// <summary>
        /// Check an <see cref="unevaluated"/> for null or empty or whitespace.
        /// </summary>
        public readonly bool HasNothing => string.IsNullOrWhiteSpace(unevaluated);

        public static bool operator ==(PropertyItem a, PropertyItem b) => a.Equals(b);

        public static bool operator !=(PropertyItem a, PropertyItem b) => !(a == b);

        public override readonly bool Equals(object obj)
        {
            if(obj is null || obj is not PropertyItem b) return false;

            return name == b.name
                && evaluated == b.evaluated
                && unevaluated == b.unevaluated
                && condition == b.condition
                && isEnvironmentProperty == b.isEnvironmentProperty
                && isGlobalProperty == b.isGlobalProperty
                && isReservedProperty == b.isReservedProperty
                && isImported == b.isImported
                && isUserDef == b.isUserDef;
        }

        public override readonly int GetHashCode() => 0.CalculateHashCode
        (
            name,
            evaluated,
            unevaluated,
            condition,
            isEnvironmentProperty,
            isGlobalProperty,
            isReservedProperty,
            isImported,
            isUserDef,
            parentProperty,
            parentProject
        );

        /// <param name="name">The name of property.</param>
        /// <param name="value">Unevaluated value.</param>
        /// <param name="condition">Optional 'Condition' attr.</param>
        public PropertyItem(string name, string value, string condition = null)
            : this()
        {
            this.name       = name;
            this.condition  = condition;
            unevaluated     = value;
            isUserDef       = true;

            // TODO: `evaluated`. Actually we need expose some optional evaluator, 
            // like in Varhead project: https://github.com/3F/Varhead/blob/master/Varhead/EvaluatorBlank.cs
            // evaluated = unevaluated;
        }

        public PropertyItem(ProjectProperty eProperty, IXProject parentProject)
            : this(eProperty)
        {
            this.parentProject = parentProject;
        }

        public PropertyItem(ProjectProperty eProperty)
            : this()
        {
            if(eProperty == null) throw new ArgumentNullException(nameof(eProperty));

            name                    = eProperty.Name;
            unevaluated             = eProperty.UnevaluatedValue;
            condition               = eProperty.Xml?.Condition;
            isEnvironmentProperty   = eProperty.IsEnvironmentProperty;
            isGlobalProperty        = eProperty.IsGlobalProperty;
            isReservedProperty      = eProperty.IsReservedProperty;
            isImported              = eProperty.IsImported;
            parentProperty          = eProperty;

            //NOTE: MS describes this as 'the evaluated property value, which is never null'
            //      But, this is not true ! >(  .NETFramework\v4.0\Microsoft.Build.dll - Version=4.0.0.0, PublicKeyToken=b03f5f7f11d50a3a
            evaluated = eProperty.EvaluatedValue ?? string.Empty;
        }

        internal PropertyItem(IXProject parentProject)
            : this()
        {
            this.parentProject = parentProject;
        }

        #region Obsolete fields
        #pragma warning disable IDE1006

        [Obsolete("Renamed as " + nameof(evaluated))]
        public readonly string evaluatedValue => evaluated;

        [Obsolete("Renamed as " + nameof(unevaluated))]
        public readonly string unevaluatedValue => unevaluated;

        #pragma warning restore IDE1006
        #endregion

        #region DebuggerDisplay

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string DbgDisplay => $"{name} = {evaluated} [{unevaluated}]";

        #endregion
    }
}