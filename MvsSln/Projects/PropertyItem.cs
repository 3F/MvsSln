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
        public string evaluatedValue;

        /// <summary>
        /// The unevaluated property value.
        /// </summary>
        public string unevaluatedValue;

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
        /// Check an <see cref="unevaluatedValue"/> for not null.
        /// </summary>
        public bool HasValue => unevaluatedValue != null;

        /// <summary>
        /// Check an <see cref="unevaluatedValue"/> for null or empty or whitespace.
        /// </summary>
        public bool HasNothing => string.IsNullOrWhiteSpace(unevaluatedValue);

        public static bool operator ==(PropertyItem a, PropertyItem b) => a.Equals(b);

        public static bool operator !=(PropertyItem a, PropertyItem b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is PropertyItem)) {
                return false;
            }

            var b = (PropertyItem)obj;

            return name == b.name
                && evaluatedValue == b.evaluatedValue
                && unevaluatedValue == b.unevaluatedValue
                && condition == b.condition
                && isEnvironmentProperty == b.isEnvironmentProperty
                && isGlobalProperty == b.isGlobalProperty
                && isReservedProperty == b.isReservedProperty
                && isImported == b.isImported
                && isUserDef == b.isUserDef
                && parentProperty == b.parentProperty
                && parentProject == b.parentProject;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                name,
                evaluatedValue,
                unevaluatedValue,
                condition,
                isEnvironmentProperty,
                isGlobalProperty,
                isReservedProperty,
                isImported,
                isUserDef,
                parentProperty,
                parentProject
            );
        }

        /// <param name="name">The name of property.</param>
        /// <param name="value">Unevaluated value.</param>
        /// <param name="condition">Optional 'Condition' attr.</param>
        public PropertyItem(string name, string value, string condition = null)
            : this()
        {
            this.name           = name;
            this.condition      = condition;
            unevaluatedValue    = value;
            isUserDef           = true;

            // TODO: `evaluatedValue`. Actually we need expose some optional evaluator, 
            // like in Varhead project: https://github.com/3F/Varhead/blob/master/Varhead/EvaluatorBlank.cs
            // evaluatedValue = unevaluatedValue;
        }

        /// <param name="eProperty"></param>
        public PropertyItem(ProjectProperty eProperty)
            : this()
        {
            if(eProperty == null) {
                throw new ArgumentNullException(nameof(eProperty));
            }

            name                    = eProperty.Name;
            unevaluatedValue        = eProperty.UnevaluatedValue;
            condition               = eProperty.Xml?.Condition;
            isEnvironmentProperty   = eProperty.IsEnvironmentProperty;
            isGlobalProperty        = eProperty.IsGlobalProperty;
            isReservedProperty      = eProperty.IsReservedProperty;
            isImported              = eProperty.IsImported;
            parentProperty          = eProperty;

            //NOTE: MS describes this as 'the evaluated property value, which is never null'
            //      But, this is not true ! >(  .NETFramework\v4.0\Microsoft.Build.dll - Version=4.0.0.0, PublicKeyToken=b03f5f7f11d50a3a
            evaluatedValue = eProperty.EvaluatedValue ?? string.Empty;
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{name} = {evaluatedValue} [{unevaluatedValue}]";
        }

        #endregion
    }
}