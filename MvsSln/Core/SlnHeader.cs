/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Core
{
    public struct SlnHeader
    {
        /// <summary>
        /// ... Format Version 12.00
        /// </summary>
        public Version FormatVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// # Visual Studio 15
        /// ...
        /// # Visual Studio 2010
        /// ...
        /// </summary>
        public string ProgramVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// VisualStudioVersion = 15.0.26730.15
        /// </summary>
        public Version VisualStudioVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// MinimumVisualStudioVersion = 10.0.40219.1
        /// </summary>
        public Version MinimumVisualStudioVersion
        {
            get;
            private set;
        }

        public void SetFormatVersion(string version)
        {
            FormatVersion = Extract(version);
        }

        public void SetProgramVersion(string version)
        {
            ProgramVersion = version;
        }

        public void SetVisualStudioVersion(string version)
        {
            VisualStudioVersion = Extract(version);
        }

        public void SetMinimumVersion(string version)
        {
            MinimumVisualStudioVersion = Extract(version);
        }

        public SlnHeader(SlnHeader header)
        {
            FormatVersion               = header.FormatVersion;
            ProgramVersion              = header.ProgramVersion;
            VisualStudioVersion         = header.VisualStudioVersion;
            MinimumVisualStudioVersion  = header.MinimumVisualStudioVersion;
        }

        private Version Extract(string version)
        {
            return String.IsNullOrWhiteSpace(version) ? null : new Version(version);
        }
    }
}
