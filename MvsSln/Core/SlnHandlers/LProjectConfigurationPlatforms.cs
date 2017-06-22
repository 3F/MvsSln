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
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LProjectConfigurationPlatforms: LAbstract, ISlnHandler
    {
        /// <param name="stream">Used stream.</param>
        /// <param name="line">Received line.</param>
        /// <param name="rsln">Handled solution data.</param>
        public override void Positioned(StreamReader stream, string line, SlnResult rsln)
        {
            if((rsln.type & SlnItems.ProjectConfPlatforms) == 0) {
                return;
            }

            if(!line.StartsWith("GlobalSection(ProjectConfigurationPlatforms)", StringComparison.Ordinal)) {
                return;
            }

            if(rsln.projectConfigs == null) {
                rsln.projectConfigs = new List<ConfigPrj>();
            }

            /*
               [Projects Guid]                        [Solution pair]                [Project pair]
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.ActiveCfg = Release|Any CPU   - configuration name
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU     - flag of build  (this line exists only when this flag is true)
            */
            string _line;
            while((_line = stream.ReadLine()) != null && _line.Trim() != "EndGlobalSection")
            {
                int x, y;

                x           = _line.IndexOf('.');
                var pGuid   = _line.Substring(0, x).Trim();

                y           = _line.IndexOf('.', ++x);
                string csln = _line.Substring(x, y - x).Trim();

                x           = _line.IndexOf('=', ++y);
                string type = _line.Substring(y, x - y).Trim();

                string cprj = _line.Substring(x + 1).Trim();

                if(!type.Equals("ActiveCfg", StringComparison.OrdinalIgnoreCase)) {
                    LSender.Send(this, $"SolutionParser: Project Configuration has been ignored for line '{_line}'", Message.Level.Debug);
                    continue;
                }

                LSender.Send(this, $"SolutionParser: Project Configuration `{pGuid}`, `{csln}` = `{cprj}`", Message.Level.Trace);

                // TODO: IncludeInBuild = true -> check existence of .Build.0
                rsln.projectConfigs.Add(
                    new ConfigPrj(cprj, pGuid, true, new ConfigSln(csln))
                );
            }
        }
    }
}
