/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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
using net.r_eg.MvsSln.Core.SlnHandlers;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Parser for basic elements from .sln files.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class SlnParser: ISlnContainer
    {
        /// <summary>
        /// Available solution handlers.
        /// </summary>
        public SynchSubscribers<ISlnHandler> SlnHandlers
        {
            get;
            protected set;
        } = new SynchSubscribers<ISlnHandler>();

        /// <summary>
        /// Parse of selected .sln file
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        public SlnResult Parse(string sln, SlnItems type)
        {
            if(String.IsNullOrWhiteSpace(sln)) {
                throw new ArgumentNullException("sln", "Value cannot be null or empty");
            }

            var data = new SlnResult() {
                solutionDir = GetPathFrom(sln),
                type = type,
            };

            HandlePreProcessing(sln, data);
            using(var reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null) {
                    HandlePositioned(reader, line.Trim(), data);
                }
            }
            HandlePostProcessing(sln, data);

            if(data.solutionConfigs != null)
            {
                data.defaultConfig = new ConfigItem(
                    ExtractDefaultConfiguration(data.solutionConfigs), 
                    ExtractDefaultPlatform(data.solutionConfigs)
                );
            }

            data.properties = GlobalProperties(sln, data.defaultConfig?.Configuration, data.defaultConfig?.Platform);
            return data;
        }

        public SlnParser()
        {
            SlnHandlers.register(new LProject());
            SlnHandlers.register(new LProjectConfigurationPlatforms());
            SlnHandlers.register(new LSolutionConfigurationPlatforms());
            SlnHandlers.register(new LProjectDependencies());
        }

        protected virtual void HandlePreProcessing(string file, SlnResult data)
        {
            foreach(ISlnHandler h in SlnHandlers) {
                h.PreProcessing(file, data);
            }
        }

        protected virtual void HandlePositioned(StreamReader reader, string line, SlnResult data)
        {
            foreach(ISlnHandler h in SlnHandlers) {
                h.Positioned(reader, line, data);
            }
        }

        protected virtual void HandlePostProcessing(string file, SlnResult data)
        {
            foreach(ISlnHandler h in SlnHandlers) {
                h.PostProcessing(file, data);
            }
        }

        protected Dictionary<string, string> GlobalProperties(string sln, string configuration, string platform)
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

        protected string ExtractDefaultConfiguration(List<ConfigSln> cfg)
        {
            foreach(ConfigSln c in cfg) {
                if(c.Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase)) {
                    return c.Configuration;
                }
            }

            if(cfg.Count > 0) {
                return cfg[0].Configuration;
            }
            return String.Empty;
        }


        protected string ExtractDefaultPlatform(List<ConfigSln> cfg)
        {
            foreach(ConfigSln c in cfg)
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

        protected string GetPathFrom(string file)
        {
            string dir = Path.GetDirectoryName(file);
            if(dir[dir.Length - 1] != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }
    }
}
