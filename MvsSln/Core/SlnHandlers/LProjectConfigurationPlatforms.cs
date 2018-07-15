/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2018  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LProjectConfigurationPlatforms: LAbstract, ISlnHandler
    {
        protected class EqCortegeComparer: IEqualityComparer<Cortege>
        {
            public bool Equals(Cortege a, Cortege b)
            {
                if(a.pGuid != b.pGuid 
                    || a.csln != b.csln 
                    || a.cprj != b.cprj
                    )
                {
                    return false;
                }
                return true;
            }

            public int GetHashCode(Cortege obj)
            {
                Func<int, int, int> polynom = delegate(int r, int x) {
                    unchecked {
                        return (r << 5) + r ^ x;
                    }
                };

                int h = 0;
                h = polynom(h, obj.pGuid.GetHashCode());
                h = polynom(h, obj.csln.GetHashCode());
                h = polynom(h, obj.cprj.GetHashCode());

                return h;
            }
        }

        protected struct Cortege
        {
            public string pGuid;
            public string csln;
            public string cprj;
        }

        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.ProjectConfPlatforms) == SlnItems.ProjectConfPlatforms);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("GlobalSection(ProjectConfigurationPlatforms)", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            if(svc.Sln.ProjectConfigList == null) {
                svc.Sln.ProjectConfigList = new List<IConfPlatformPrj>();
            }

            /*
               [Projects Guid]                        [Solution pair]                [Project pair]
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.ActiveCfg = Release|Any CPU   - configuration name
               {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU     - flag of build  (this line exists only when this flag is true)
            */
            string _line;

            var cortege = new Dictionary<Cortege, ConfigPrj>(new EqCortegeComparer());
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                int x, y;

                x           = _line.IndexOf('.');
                var pGuid   = _line.Substring(0, x).Trim();

                y           = _line.IndexOf('.', ++x);
                string csln = _line.Substring(x, y - x).Trim();

                x           = _line.IndexOf('=', ++y);
                string type = _line.Substring(y, x - y).Trim();

                string cprj = _line.Substring(x + 1).Trim();

                bool isActiveCfg    = type.Equals("ActiveCfg", StringComparison.OrdinalIgnoreCase);
                bool isBuild0       = type.Equals("Build.0", StringComparison.OrdinalIgnoreCase);

                if(!isActiveCfg && !isBuild0) {
                    LSender.Send(this, $"Project Configuration has been ignored for line '{_line}'", Message.Level.Debug);
                    continue;
                }

                var ident = new Cortege() {
                    pGuid   = pGuid,
                    csln    = csln,
                    cprj    = cprj,
                };

                if(!cortege.ContainsKey(ident))
                {
                    LSender.Send(this, $"New Project Configuration `{pGuid}`, `{csln}` = `{cprj}` /{type}", Message.Level.Info);
                    cortege[ident] = new ConfigPrj(cprj, pGuid, isBuild0, new ConfigSln(csln));
                    svc.Sln.ProjectConfigList.Add(cortege[ident]);
                    continue;
                }

                if(isBuild0)
                {
                    LSender.Send(this, $"Project Configuration, update Build.0  `{pGuid}`", Message.Level.Debug);
                    cortege[ident].IncludeInBuild = true;
                    continue;
                }
            }

            return true;
        }
    }
}
