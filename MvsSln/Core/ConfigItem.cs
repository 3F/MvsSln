/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
        /// To use virtual `Sensitivity` method in comparing objects.
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

        public string PlatformByRuleICase
        {
            get => Sensitivity(PlatformByRule);
        }

        public static bool operator ==(ConfigItem a, ConfigItem b)
        {
            return Object.ReferenceEquals(a, null) ? 
                    Object.ReferenceEquals(b, null) : a.Equals(b);
        }

        public static bool operator !=(ConfigItem a, ConfigItem b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if(Object.ReferenceEquals(obj, null) || !(obj is ConfigItem)) {
                return false;
            }

            var b = (ConfigItem)obj;

            return ConfigurationByRuleICase == b.ConfigurationByRuleICase 
                    && PlatformByRuleICase == b.PlatformByRuleICase;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                Configuration.GetHashCode(),
                Platform.GetHashCode()
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
            if(String.IsNullOrWhiteSpace(formatted)) {
                return;
            }

            string[] cfg = formatted.Split('|');
            if(cfg.Length < 2) {
                throw new ArgumentException($"The format `{formatted}` of configuration is not supported.");
            }

            Configuration   = cfg[0];
            Platform        = cfg[1];
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
