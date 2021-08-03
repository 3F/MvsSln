/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Basic item of configuration and platform.
    /// </summary>
    [DebuggerDisplay("{DbgDisplay}")]
    public class ConfigItem: IConfPlatform
    {
        /// <summary>
        /// The custom rule of the Configuration and Platform names.
        /// </summary>
        public IRuleOfConfig Rule
        {
            get;
            set;
        } = new RuleOfConfig();

        /// <summary>
        /// To use an `Sensitivity` logic when comparing {IConfPlatform}
        /// together with `==` , `!=`.
        /// </summary>
        public bool SensitivityComparing
        {
            get;
            set;
        } = true;

        public string Configuration
        {
            get;
            protected set;
        }

        public string ConfigurationByRule
        {
            get => Rule?.Configuration(Configuration);
        }

        /// <summary>
        /// {ConfigurationByRule} with optional case insensitive logic.
        /// Uses {SensitivityComparing} flag.
        /// </summary>
        public string ConfigurationByRuleICase
        {
            get => Sensitivity(ConfigurationByRule);
        }

        public string Platform
        {
            get;
            protected set;
        }

        public string PlatformByRule
        {
            get => Rule?.Platform(Platform);
        }

        /// <summary>
        /// {PlatformByRule} with optional case insensitive logic.
        /// Uses {SensitivityComparing} flag.
        /// </summary>
        public string PlatformByRuleICase
        {
            get => Sensitivity(PlatformByRule);
        }

        /// <summary>
        /// Checking an config/platform by using {Rule} instance.
        /// </summary>
        /// <param name="config">Configuration name.</param>
        /// <param name="platform">Platform name.</param>
        /// <param name="icase">Case insensitive flag.</param>
        /// <returns></returns>
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
                Platform
            );
        }

        public override string ToString()
        {
            return Format();
        }

        /// <summary>
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        public static string Format(string configuration, string platform)
        {
            return $"{configuration}|{platform}";
        }

        public string Format()
        {
            return Format(Configuration, Platform);
        }

        public ConfigItem(string configuration, string platform)
        {
            Configuration   = configuration;
            Platform        = platform;
        }

        public ConfigItem(string formatted)
        {
            if(formatted == null) {
                return;
            }

            string[] cfg = formatted.Split('|');

            Configuration = cfg[0];

            // < 2 https://github.com/3F/MvsSln/issues/19
            Platform = cfg.Length < 2 ? string.Empty : cfg[1];
        }

        protected virtual string Sensitivity(string name)
        {
            if(!SensitivityComparing) {
                return name;
            }
            return name.ToLowerInvariant();
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => Format();
        }

        #endregion
    }
}
