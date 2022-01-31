/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Project Configuration
    /// </summary>
    [DebuggerDisplay("{DbgDisplay}")]
    public class ConfigPrj: ConfigItem, IConfPlatform, IConfPlatformPrj
    {
        /// <summary>
        /// Project Guid.
        /// </summary>
        public string PGuid
        {
            get;
            protected set;
        }

        /// <summary>
        /// Existence of `.Build.0` to activate project for build:
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Build.0 = Release|Any CPU
        /// </summary>
        public bool IncludeInBuild
        {
            get;
            internal set;
        }

        /// <summary>
        /// Existence of `.Deploy.0` to activate project for deployment:
        /// {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Release|Any CPU.Deploy.0 = Release|Any CPU
        /// </summary>
        public bool IncludeInDeploy
        {
            get;
            internal set;
        }

        /// <summary>
        /// Link to solution configuration.
        /// </summary>
        public IConfPlatform Sln
        {
            get;
            protected set;
        }

        public ConfigPrj(string name, string platform, string pGuid, bool build, ConfigSln sln)
            : base(name, platform)
        {
            Set(pGuid, build, false, sln);
        }

        public ConfigPrj(string formatted, string pGuid, bool build, ConfigSln sln)
            : base(formatted)
        {
            Set(pGuid, build, false, sln);
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
        private string DbgDisplay
        {
            get => $"{ToString()} -> {Sln} : [{PGuid}]";
        }

        #endregion
    }
}