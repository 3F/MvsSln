/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Core
{
    public sealed class SlnHeader
    {
        /// <summary>
        /// VS2010 Pro 10.0.40219 SP1
        /// </summary>
        public static readonly Version VisualStudio10_0_40219_1 = new(10, 0, 40219, 1);

        /// <summary>
        /// E.g. Microsoft Visual Studio Solution File, Format Version 12.00
        /// </summary>
        public Version FormatVersion { get; private set; }

        /// <summary>
        /// Formatted <see cref="FormatVersion"/>
        /// </summary>
        public string FormatVersionMajorMinor
            => FormatVersion == null ? null 
                : string.Format("{0}.{1:00}", FormatVersion.Major, FormatVersion.Minor);

        /// <summary>
        /// E.g. 
        /// # Visual Studio 15
        /// ...
        /// # Visual Studio 2010
        /// ...
        /// </summary>
        public string ProgramVersion { get; private set; }

        /// <summary>
        /// E.g. VisualStudioVersion = 15.0.26730.15
        /// </summary>
        public Version VisualStudioVersion { get; private set; }

        /// <summary>
        /// E.g. MinimumVisualStudioVersion = 10.0.40219.1
        /// </summary>
        public Version MinimumVisualStudioVersion { get; private set; }

        public SlnHeader SetFormatVersion(string version)
        {
            FormatVersion = Extract(version);
            return this;
        }

        public SlnHeader SetProgramVersion(string version)
        {
            ProgramVersion = version;
            return this;
        }

        public SlnHeader SetVisualStudioVersion(string version)
        {
            VisualStudioVersion = Extract(version);
            return this;
        }

        public SlnHeader SetMinimumVersion(string version)
        {
            MinimumVisualStudioVersion = Extract(version);
            return this;
        }

        public static SlnHeader MakeDefault()
            => new("12.00", "17.8.34525.116", VisualStudio10_0_40219_1);

        public SlnHeader(SlnHeader header)
        {
            if(header == null) return;

            FormatVersion               = header.FormatVersion;
            ProgramVersion              = header.ProgramVersion;
            VisualStudioVersion         = header.VisualStudioVersion;
            MinimumVisualStudioVersion  = header.MinimumVisualStudioVersion;
        }

        public SlnHeader(string fVersion, string visualStudio, string program = null)
            : this(fVersion, visualStudio, VisualStudio10_0_40219_1, program)
        {

        }

        public SlnHeader(string fVersion, string visualStudio, Version minimum, string program = null)
        {
            SetFormatVersion(fVersion ?? throw new ArgumentNullException(nameof(fVersion)));
            SetVisualStudioVersion(visualStudio ?? throw new ArgumentNullException(nameof(visualStudio)));
            MinimumVisualStudioVersion = minimum ?? throw new ArgumentNullException(nameof(minimum));
            SetProgramVersion(program ?? VisualStudioVersion.Major.ToString());
        }

        public SlnHeader(string fVersion, string visualStudio, string minimum, string program)
            : this
            (
                  fVersion,
                  visualStudio,
                  Extract(minimum ?? throw new ArgumentNullException(nameof(minimum))),
                  program ?? throw new ArgumentNullException(nameof(program))
             )
        {

        }

        public SlnHeader(string formatVersion)
        {
            SetFormatVersion(formatVersion ?? throw new ArgumentNullException(nameof(formatVersion)));
        }

        public SlnHeader()
        {

        }

        private static Version Extract(string version)
        {
            return string.IsNullOrWhiteSpace(version) ? null : new Version(version);
        }
    }
}
