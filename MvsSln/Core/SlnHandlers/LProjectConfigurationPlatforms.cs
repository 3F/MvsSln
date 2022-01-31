/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LProjectConfigurationPlatforms: LAbstract, ISlnHandler
    {
        protected enum LineAttr
        {
            InvalidOrUnknown,

            ActiveCfg,
            Build0,
            Deploy0
        }

        protected struct Cortege
        {
            public string pGuid, csln, cprj;

            public override string ToString()
            {
                return $"`{pGuid}`, `{csln}` = `{cprj}`";
            }
        }

        protected class EqCortegeComparer: IEqualityComparer<Cortege>
        {
            public bool Equals(Cortege a, Cortege b)
            {
                return a.pGuid == b.pGuid && a.csln == b.csln && a.cprj == b.cprj;
            }

            public int GetHashCode(Cortege x)
            {
                return 0.CalculateHashCode
                (
                    x.pGuid, x.csln, x.cprj
                );
            }
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

            var records = new Dictionary<Cortege, ConfigPrj>(new EqCortegeComparer());

            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                var v = Parse(ref _line, out LineAttr ltype);

                if(ltype == LineAttr.InvalidOrUnknown)
                {
                    LSender.Send(this, $"Incorrect Project Configuration: {v}; raw: '{_line}'", Message.Level.Warn);
                    continue;
                }

                //NOTE: Build0 and Deploy0 records are valid too. Even if an ActiveCfg is corrupted and does not exist at all.
                if(!records.ContainsKey(v))
                {
                    LSender.Send(this, $"Found Project Configuration: {v}", Message.Level.Info);

                    records[v] = new ConfigPrj(v.cprj, v.pGuid, false, new ConfigSln(v.csln));
                    svc.Sln.ProjectConfigList.Add(records[v]);
                }

                if(ltype == LineAttr.Build0)
                {
                    LSender.Send(this, $"Project Configuration, update Build.0  {v}", Message.Level.Debug);
                    records[v].IncludeInBuild = true;
                    continue;
                }

                if(ltype == LineAttr.Deploy0)
                {
                    LSender.Send(this, $"Project Configuration, update Deploy.0  {v}", Message.Level.Debug);
                    records[v].IncludeInDeploy = true;
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// [Projects Guid]                        [Solution pair]     [ltype]     [Project pair]
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.ActiveCfg = Release|Any CPU   - available configuration
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU     - active Build (this line exists only when this flag is true)
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Deploy.0 = Release|Any CPU    - active Deployment (this line exists only when this flag is true)
        /// 
        /// Possible symbols for Solution/Project pair includes `.` and `=`:
        /// https://github.com/3F/MvsSln/issues/13
        /// 
        /// -_- awesome format as follow:
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.Debug.x64.x86|Any.CPU.etc.Build.0 = Debug.x64.x86|Any.CPU.etc
        /// \___________________________________/  \___________/ \_________/ \_____/ ^ \___________/ \_________/
        /// 
        /// For `=` we will not support this due to errors by VS itself (VS bug from VS2010 to modern VS2019)
        /// https://github.com/3F/MvsSln/issues/13#issuecomment-501346079
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="ltype"></param>
        /// <returns></returns>
        protected Cortege Parse(ref string raw, out LineAttr ltype)
        {
            int splitter = raw.IndexOf('=');

            if(splitter == -1)
            {
                ltype = LineAttr.InvalidOrUnknown;
                return default(Cortege);
            }

            string cprj = raw.Substring(splitter + 1);

            splitter    = raw.FirstNonWhiteSpace(splitter - 1, true) + 1;
            int rpos    = raw.LastIndexOf('.', splitter); // .ActiveCfg =
                                                          // .Build.0 =
                                                          // ------^

            if(splitter - rpos == 2) { // .0
                rpos = raw.LastIndexOf('.', rpos - 1);
            }

            int lpos        = raw.IndexOf('.');
            string pGuid    = raw.Substring(0, lpos);

            string csln     = raw.Substring(++lpos, rpos - lpos);
            string type     = raw.Substring(++rpos, splitter - rpos);

            ltype = GetAttribute(type.Trim());

            return new Cortege()
            {
                pGuid   = pGuid.Trim(),
                csln    = csln.Trim(),
                cprj    = cprj.Trim(),
            };
        }

        protected LineAttr GetAttribute(string raw)
        {
            if(raw.Equals("ActiveCfg", StringComparison.InvariantCulture)) {
                return LineAttr.ActiveCfg;
            }

            if(raw.Equals("Build.0", StringComparison.InvariantCulture)) {
                return LineAttr.Build0;
            }

            if(raw.Equals("Deploy.0", StringComparison.InvariantCulture)) {
                return LineAttr.Deploy0;
            }

            return LineAttr.InvalidOrUnknown;
        }
    }
}
