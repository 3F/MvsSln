/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Basic item of the configuration and its platform.
    /// </summary>
    public class ConfigItem: IConfPlatform
    {
        protected const char DELIM = '|';

        private string _fmt;

        public IRuleOfConfig Rule { get; protected set; }

        public bool SensitivityComparing { get; set; } = true;

        public string Configuration { get; protected set; }

        public string ConfigurationByRule
        {
            get => Rule?.Configuration(Configuration) ?? Configuration;
        }

        public string ConfigurationByRuleICase => Sensitivity(ConfigurationByRule);

        public string Platform { get; protected set; }

        public string PlatformByRule => Rule?.Platform(Platform) ?? Platform;

        public string PlatformByRuleICase => Sensitivity(PlatformByRule);

        public string Formatted => _fmt ??= Format(ConfigurationByRule, PlatformByRule);

        public bool IsEqualByRule(string config, string platform, bool icase = false)
        {
            var cmp = icase ? StringComparison.InvariantCultureIgnoreCase 
                            : StringComparison.InvariantCulture;

            return string.Equals(ConfigurationByRule, Rule?.Configuration(config), cmp)
                && string.Equals(PlatformByRule, Rule?.Platform(platform), cmp);
        }

        public static bool operator ==(ConfigItem a, ConfigItem b)
        {
            return a is null ? b is null : a.Equals(b);
        }

        public static bool operator !=(ConfigItem a, ConfigItem b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is ConfigItem)) {
                return false;
            }

            var b = (ConfigItem)obj;

            // NOTE: {SensitivityComparing} will control an `Sensitivity` logic, 
            //       thus we need only `...ByRuleICase` properties:
            return ConfigurationByRuleICase == b.ConfigurationByRuleICase 
                    && PlatformByRuleICase == b.PlatformByRuleICase;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                Configuration,
                Platform,
                Rule,
                SensitivityComparing
            );
        }

        public override string ToString() => Format(Configuration, Platform);

        /// <summary>
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        public static string Format(string configuration, string platform)
        {
            return $"{configuration}{DELIM}{platform}";
        }

        [Obsolete("Use `ToString()` and `Formatted` instead.")]
        public string Format() => ToString();

        /// <summary>
        /// Initialize using custom rule.
        /// </summary>
        /// <param name="rule">Custom rule. Use null to disable it.</param>
        /// <param name="configuration">Configuration name.</param>
        /// <param name="platform">Platform name.</param>
        public ConfigItem(IRuleOfConfig rule, string configuration, string platform)
        {
            Rule            = rule;
            Configuration   = configuration;
            Platform        = platform;
        }

        /// <summary>
        /// Initialize using rule <see cref="RuleOfConfig"/> by default.
        /// </summary>
        /// <inheritdoc cref="ConfigItem(IRuleOfConfig, string, string)"/>
        public ConfigItem(string configuration, string platform)
            : this(new RuleOfConfig(), configuration, platform)
        {

        }

        /// <summary>
        /// Initialize using rule <see cref="RuleOfConfig"/> by default.
        /// </summary>
        /// <inheritdoc cref="ConfigItem(IRuleOfConfig, string)"/>
        public ConfigItem(string formatted)
            : this(new RuleOfConfig(), formatted)
        {

        }

        /// <summary>
        /// Initialize using custom rule.
        /// </summary>
        /// <param name="rule">Custom rule. Use null to disable it.</param>
        /// <param name="formatted">Raw formatted string.</param>
        public ConfigItem(IRuleOfConfig rule, string formatted)
            : this
            (
                rule, 
                ExtractName(formatted, out int delimiter), 
                ExtractPlatform(formatted, delimiter)
            )
        {

        }

        protected virtual string Sensitivity(string name)
        {
            if(!SensitivityComparing) {
                return name;
            }
            return name?.ToLowerInvariant();
        }

        private static string ExtractName(string raw, out int delimiter)
        {
            if(raw == null)
            {
                delimiter = -1;
                return null;
            }

            delimiter = raw.IndexOf(DELIM);
            if(delimiter == -1) return raw;

            return raw.Substring(0, delimiter);
        }

        private static string ExtractPlatform(string raw, int delimiter)
        {
            if(raw == null) return null;
            if(delimiter == -1) return string.Empty;

            return raw.Substring(delimiter + 1);
        }
    }
}
