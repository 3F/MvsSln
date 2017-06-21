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
using net.r_eg.vsSBE.CI.MSBuild;

namespace net.r_eg.vsSBE.Sln
{
    /// <summary>
    /// For work with .sln files.
    /// Gets list of projects, available configurations etc.
    /// Please note: it's necessary for working without DTE-context/IDE mode, for example with isolated enviroment.
    ///              Use the EnvDTE and ProjectCollection if it's possible!
    /// 
    /// Another variants:
    /// * Using deprecated Microsoft.Build.BuildEngine.Project - http://msdn.microsoft.com/en-us/library/microsoft.build.buildengine.project%28v=vs.100%29.aspx
    /// * Or use reflection of the internal SolutionParser from Microsoft.Build.Construction/BuildEngine.Shared, for example for getting all projects:
    ///   -> void ParseProject(string firstLine)
    ///      -> void ParseFirstProjectLine(string firstLine, ProjectInSolution proj)
    ///      -> crackProjectLine -> PROJECTNAME + RELATIVEPATH
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Full path to root solution directory
        /// </summary>
        private string solutionDir;

        /// <summary>
        /// Provides the result of work
        /// </summary>
        public sealed class Result
        {
            /// <summary>
            /// Solution configurations with platforms.
            /// </summary>
            public List<ConfigSln> configs;

            /// <summary>
            /// Project configurations with platforms.
            /// </summary>
            public List<ConfigPrj> projectConfigs;

            /// <summary>
            /// All found projects for solution.
            /// </summary>
            public List<Project> projects;
        }

        /// <summary>
        /// Parse of selected .sln file
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <returns></returns>
        public Result parse(string sln)
        {
            solutionDir = getPathFrom(sln);

            Result Data = new Result()
            {
                configs         = new List<ConfigSln>(),
                projectConfigs  = new List<ConfigPrj>(),
                projects        = new List<Project>(),
            };

            using(StreamReader reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if(line.StartsWith("Project(", StringComparison.Ordinal)) {
                        hProject(line, ref Data.projects);
                    }

                    if(line.StartsWith("GlobalSection(SolutionConfigurationPlatforms)", StringComparison.Ordinal)) {
                        hSlnConfigs(reader, ref Data.configs);
                    }

                    if(line.StartsWith("GlobalSection(ProjectConfigurationPlatforms)", StringComparison.Ordinal)) {
                        hPrjConfigs(reader, ref Data.projectConfigs);
                    }
                }
            }
            return Data;
        }

        /// <summary>
        /// SolutionConfigurationPlatforms section
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        protected void hSlnConfigs(StreamReader reader, ref List<ConfigSln> configuration)
        {
            string line;
            while((line = reader.ReadLine()) != null && line.Trim() != "EndGlobalSection")
            {
                string left = line.Split('=')[0].Trim(); // Debug|Win32 = Debug|Win32
                if(string.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0) {
                    Log.Trace("SolutionParser: Solution Configuration has been ignored for line '{0}'", line);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) {
                    continue;
                }

                Log.Trace("SolutionParser: Solution Configuration ->['{0}' ; '{1}']", cfg[0], cfg[1]);
                configuration.Add(new ConfigSln() {
                    configuration   = cfg[0],
                    platform        = cfg[1]
                });
            }
        }

        /// <summary>
        /// ProjectConfigurationPlatforms section
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        protected void hPrjConfigs(StreamReader reader, ref List<ConfigPrj> configuration)
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
                    Log.Trace("SolutionParser: Project Configuration has been ignored for line '{0}'", line);
                    continue;
                }

                Log.Trace($"SolutionParser: Project Configuration `{pGuid}`, `{csln}` = `{cprj}`");

                configuration.Add(new ConfigPrj(cprj) {
                    pGuid = pGuid,
                    sln = new ConfigSln(csln),
                    includeInBuild = true // TODO: check existence of .Build.0
                });
            }
        }

        /// <summary>
        /// Project section
        /// </summary>
        /// <param name="line"></param>
        /// <param name="projects"></param>
        protected void hProject(string line, ref List<Project> projects)
        {
            // Pattern based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser.
            string pattern = "^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$";
            Match m = Regex.Match(line, pattern);
            if(!m.Success) {
                Log.Debug("SolutionParser: incorrect line for pattern :: '{0}'", line);
                return;
            }

            string pType = m.Groups["TypeGuid"].Value.Trim();
            
            if(String.Equals("{2150E333-8FDC-42A3-9474-1A3956D46DE8}", pType, StringComparison.OrdinalIgnoreCase)) {
                Log.Trace("SolutionParser: ignored as SolutionFolder");
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
                fullPath = (!String.IsNullOrEmpty(pPath))? Path.Combine(solutionDir, pPath) : pPath;
            }

            Log.Trace("SolutionParser: project ->[Type: '{0}'; Name: '{1}'; Path: '{2}'; GUID: '{3}'; FullPath: '{4}']",
                                                            pType, pName, pPath, pGuid, fullPath);
            projects.Add(new Project() {
                type        = pType,
                name        = pName,
                path        = pPath,
                fullPath    = fullPath,
                pGuid       = pGuid
            });
        }

        protected string getPathFrom(string file)
        {
            string dir = Path.GetDirectoryName(file);
            if(dir[dir.Length - 1] != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }
    }
}
