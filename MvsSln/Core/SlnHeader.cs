/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
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
