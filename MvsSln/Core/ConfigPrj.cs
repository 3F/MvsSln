/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Project Configuration
    /// </summary>
    [DebuggerDisplay("{DbgDisplay}")]
    public class ConfigPrj: ConfigItem, IConfPlatform, IConfPlatformPrj
    {
        public string PGuid { get; protected set; }

        public bool IncludeInBuild { get; protected internal set; }

        public bool IncludeInDeploy { get; protected internal set; }

        public IConfPlatform Sln { get; protected set; }

        public ConfigPrj(IConfPlatform prj, string pGuid, bool build = true, bool deploy = false)
            : this
            (
                  prj?.Configuration ?? throw new ArgumentNullException(nameof(prj)),
                  prj.Platform,
                  pGuid,
                  build,
                  deploy,
                  new(prj.Configuration, prj.Platform)
            )
        {

        }

        public ConfigPrj(IConfPlatform prj, string pGuid, bool build, ConfigSln sln)
            : this(prj, pGuid, build, deploy: false, sln)
        {

        }

        public ConfigPrj(IConfPlatform prj, string pGuid, bool build, bool deploy, ConfigSln sln)
            : this
            (
                  prj?.Configuration ?? throw new ArgumentNullException(nameof(prj)),
                  prj.Platform,
                  pGuid,
                  build,
                  deploy,
                  sln
            )
        {

        }

        public ConfigPrj(string name, string platform, string pGuid, bool build, ConfigSln sln)
            : base(name, platform)
        {
            Set(pGuid, build, deploy: false, sln);
        }

        public ConfigPrj(string formatted, string pGuid, bool build, ConfigSln sln)
            : base(formatted)
        {
            Set(pGuid, build, deploy: false, sln);
        }

        public ConfigPrj(string name, string platform, string pGuid, bool build, bool deploy, ConfigSln sln)
            : base(name, platform)
        {
            Set(pGuid, build, deploy, sln);
        }

        public ConfigPrj(string formatted, string pGuid, bool build, bool deploy, ConfigSln sln)
            : base(formatted)
        {
            Set(pGuid, build, deploy, sln);
        }

        private void Set(string pGuid, bool build, bool deploy, ConfigSln sln)
        {
            PGuid           = pGuid;
            IncludeInBuild  = build;
            IncludeInDeploy = deploy;
            Sln             = sln;
        }

        #region DebuggerDisplay

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DbgDisplay => $"{ToString()} -> {Sln} : [{PGuid}]";

        #endregion
    }
}