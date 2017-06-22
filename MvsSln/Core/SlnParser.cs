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
using System.Text.RegularExpressions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Parser for basic elements from .sln files.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// for work with EnvDTE and ProjectCollection without DTE-context for Isolated Environment.
    /// i.e. alternative to:
    /// * deprecated Microsoft.Build.BuildEngine.Project
    /// * access via reflection to the internal SolutionParser from Microsoft.Build.Construction/BuildEngine.Shared
    ///   -> void ParseProject(string firstLine)
    ///      -> void ParseFirstProjectLine(string firstLine, ProjectInSolution proj)
    ///      -> crackProjectLine -> PROJECTNAME + RELATIVEPATH
    /// etc.
    /// 
    /// Today it just re-licensed and separated into new project 'as is'. Thus:
    /// TODO: after import - abstraction layer, and scalable items
    /// </summary>
    internal class SlnParser
    {
        /// <summary>
        /// Full path to root solution directory
        /// </summary>
        public string SolutionDir
        {
            get;
            protected set;
        }

        /// <summary>
        /// Parse of selected .sln file
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        public SlnResult Parse(string sln, SlnItems type)
        {
            SolutionDir = GetPathFrom(sln);
            var data    = InitResult(type);

            using(var reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if(data.projectItems != null 
                        && line.StartsWith("Project(", StringComparison.Ordinal))
                    {
                        SetProjects(line, ref data.projectItems);
                    }

                    if(data.solutionConfigs != null 
                        && line.StartsWith("GlobalSection(SolutionConfigurationPlatforms)", StringComparison.Ordinal))
                    {
                        SetSlnConfigs(reader, ref data.solutionConfigs);
                    }

                    if(data.projectConfigs != null 
                        && line.StartsWith("GlobalSection(ProjectConfigurationPlatforms)", StringComparison.Ordinal))
                    {
                        SetPrjConfigs(reader, ref data.projectConfigs);
                    }
                }
            }

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

        protected SlnResult InitResult(SlnItems type)
        {
            var ret = new SlnResult();

            if((type & SlnItems.Projects) != 0) {
                ret.projectItems = new List<ProjectItem>();
            }

            if((type & SlnItems.SolutionConfPlatforms) != 0) {
                ret.solutionConfigs = new List<ConfigSln>();
            }

            if((type & SlnItems.ProjectConfPlatforms) != 0) {
                ret.projectConfigs = new List<ConfigPrj>();
            }

            return ret;
        }

        /// <summary>
        /// SolutionConfigurationPlatforms section
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        protected void SetSlnConfigs(StreamReader reader, ref List<ConfigSln> configuration)
        {
            string line;
            while((line = reader.ReadLine()) != null && line.Trim() != "EndGlobalSection")
            {
                string left = line.Split('=')[0].Trim(); // Debug|Win32 = Debug|Win32
                if(string.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0) {
                    LSender.Send(this, $"SolutionParser: Solution Configuration has been ignored for line '{line}'", Message.Level.Debug);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) {
                    continue;
                }

                LSender.Send(this, $"SolutionParser: Solution Configuration ->['{cfg[0]}' ; '{cfg[1]}']", Message.Level.Trace);
                configuration.Add(new ConfigSln(cfg[0], cfg[1]));
            }
        }

        /// <summary>
        /// ProjectConfigurationPlatforms section
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        protected void SetPrjConfigs(StreamReader reader, ref List<ConfigPrj> configuration)
        {
            /*
               [Projects Guid]                        [Solution pair]                [Project pair]
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.ActiveCfg = Release|Any CPU   - configuration name
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU     - flag of build  (this line exists only when this flag is true)
            */
            string line;
            while((line = reader.ReadLine()) != null && line.Trim() != "EndGlobalSection")
            {
                int x, y;

                x           = line.IndexOf('.');
                var pGuid   = line.Substring(0, x).Trim();

                y           = line.IndexOf('.', ++x);
                string csln = line.Substring(x, y - x).Trim();

                x           = line.IndexOf('=', ++y);
                string type = line.Substring(y, x - y).Trim();

                string cprj = line.Substring(x + 1).Trim();

                if(!type.Equals("ActiveCfg", StringComparison.OrdinalIgnoreCase)) {
                    LSender.Send(this, $"SolutionParser: Project Configuration has been ignored for line '{line}'", Message.Level.Debug);
                    continue;
                }

                LSender.Send(this, $"SolutionParser: Project Configuration `{pGuid}`, `{csln}` = `{cprj}`", Message.Level.Trace);

                // TODO: IncludeInBuild = true -> check existence of .Build.0
                configuration.Add(
                    new ConfigPrj(cprj, pGuid, true, new ConfigSln(csln))
                );
            }
        }

        /// <summary>
        /// Project section
        /// </summary>
        /// <param name="line"></param>
        /// <param name="projects"></param>
        protected void SetProjects(string line, ref List<ProjectItem> projects)
        {
            // Pattern based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser.
            string pattern = "^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$";
            Match m = Regex.Match(line, pattern);
            if(!m.Success) {
                LSender.Send(this, $"SolutionParser: incorrect line for pattern :: '{line}'", Message.Level.Warn);
                return;
            }

            string pType = m.Groups["TypeGuid"].Value.Trim();
            
            if(String.Equals("{2150E333-8FDC-42A3-9474-1A3956D46DE8}", pType, StringComparison.OrdinalIgnoreCase)) {
                LSender.Send(this, "SolutionParser: ignored as SolutionFolder", Message.Level.Debug);
                return;
            }

            string pName = m.Groups["Name"].Value.Trim();
            string pPath = m.Groups["Path"].Value.Trim();
            string pGuid = m.Groups["Guid"].Value.Trim();

            string fullPath;
            if(Path.IsPathRooted(pPath)) {
                fullPath = pPath;
            }
            else {
                fullPath = (!String.IsNullOrEmpty(pPath))? Path.Combine(SolutionDir, pPath) : pPath;
            }

            LSender.Send(this, $"SolutionParser: project ->['{pGuid}'; '{pName}'; '{pPath}'; '{fullPath}'; '{pType}' ]", Message.Level.Trace);
            projects.Add(new ProjectItem() {
                type        = pType,
                name        = pName,
                path        = pPath,
                fullPath    = fullPath,
                pGuid       = pGuid
            });
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
