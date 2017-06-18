/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    /// <summary>
    /// Gets required properties from .sln files.
    /// 
    /// Basically, this reproduced logic of the internal SolutionParser from Microsoft.Build.Construction/BuildEngine.Shared.
    /// So we can also use reflection as variant...
    /// </summary>
    public class SolutionProperties
    {
        /// <summary>
        /// Configuration / Platform
        /// </summary>
        public struct SolutionCfg
        {
            public string Configuration { get; set; }
            public string Platform { get; set; }
        }

        /// <summary>
        /// Provides the result of work
        /// </summary>
        public sealed class Result
        {
            /// <summary>
            /// Configurations with platforms in solution file
            /// </summary>
            public List<SolutionCfg> configs;

            /// <summary>
            /// Default Configuration for current solution
            /// </summary>
            public string defaultConfiguration;

            /// <summary>
            /// Default Platform for current solution
            /// </summary>
            public string defaultPlatform;

            /// <summary>
            /// All available global properties.
            /// </summary>
            public Dictionary<string, string> properties;
        }

        /// <summary>
        /// Used logger
        /// </summary>
        internal ILog log;

        internal SolutionProperties(ILog logger)
        {
            log = logger;
        }

        public SolutionProperties(bool diagnostic = false)
        {
            log = new Log((diagnostic)? LoggerVerbosity.Diagnostic : LoggerVerbosity.Normal);
        }
        
        /// <summary>
        /// Parse of selected .sln file
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <returns></returns>
        public Result parse(string sln)
        {
            Result Data = new Result() {
                configs     = new List<SolutionCfg>(),
                properties  = new Dictionary<string, string>()
            };

            using(StreamReader reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if(line.StartsWith("GlobalSection(SolutionConfigurationPlatforms)", StringComparison.Ordinal)) {
                        hConfiguration(reader, ref Data.configs);
                    }
                }
            }

            Data.defaultConfiguration   = extractDefaultConfiguration(Data.configs);
            Data.defaultPlatform        = extractDefaultPlatform(Data.configs);
            Data.properties             = globalProperties(sln, Data.defaultConfiguration, Data.defaultPlatform);

            return Data;
        }


        /// <summary>
        /// SolutionConfigurationPlatforms section.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        protected void hConfiguration(StreamReader reader, ref List<SolutionCfg> configuration)
        {
            string line;
            while((line = reader.ReadLine()) != null && line.Trim() != "EndGlobalSection")
            {
                string left = line.Split('=')[0].Trim(); // Debug|Win32 = Debug|Win32
                if(string.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0) {
                    log.debug("SolutionProperties: Configuration has been ignored for line '{0}'", line);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) {
                    continue;
                }

                log.debug("SolutionProperties: Configuration ->['{0}' ; '{1}']", cfg[0], cfg[1]);
                configuration.Add(new SolutionCfg() {
                    Configuration   = cfg[0],
                    Platform        = cfg[1]
                });
            }
        }

        protected Dictionary<string, string> globalProperties(string sln, string configuration, string platform)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            string dir = Path.GetDirectoryName(sln);
            if(dir[dir.Length - 1] != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }

            ret["SolutionDir"]      = dir;
            ret["SolutionExt"]      = Path.GetExtension(sln);
            ret["SolutionFileName"] = Path.GetFileName(sln);
            ret["SolutionName"]     = Path.GetFileNameWithoutExtension(sln);
            ret["SolutionPath"]     = sln;

            ret["Configuration"]    = configuration;
            ret["Platform"]         = platform;

            return ret;
        }

        protected string extractDefaultConfiguration(List<SolutionCfg> cfg)
        {
            foreach(SolutionCfg c in cfg) {
                if(c.Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase)) {
                    return c.Configuration;
                }
            }

            if(cfg.Count > 0) {
                return cfg[0].Configuration;
            }
            return String.Empty;
        }


        protected string extractDefaultPlatform(List<SolutionCfg> cfg)
        {
            foreach(SolutionCfg c in cfg)
            {
                if(c.Platform.Equals("Mixed Platforms", StringComparison.OrdinalIgnoreCase)) {
                    return c.Platform;
                }

                if(c.Platform.Equals("Any CPU", StringComparison.OrdinalIgnoreCase)) {
                    return c.Platform;
                }
            }

            if(cfg.Count > 0) {
                return cfg[0].Platform;
            }
            return String.Empty;
        }
    }
}
