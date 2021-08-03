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
using System.Text;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public class WVisualStudioVersion: WAbstract, IObjHandler
    {
        /// <summary>
        /// Header information.
        /// </summary>
        protected SlnHeader header;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            string fmt = String.Format("{0}.{1:00}", header.FormatVersion.Major, header.FormatVersion.Minor);
            sb.AppendLine($"Microsoft Visual Studio Solution File, Format Version {fmt}");

            if(header.ProgramVersion != null) {
                sb.AppendLine($"# Visual Studio {header.ProgramVersion}");
            }

            if(header.VisualStudioVersion != null) {
                sb.AppendLine($"VisualStudioVersion = {header.VisualStudioVersion.ToString()}");
            }

            if(header.MinimumVisualStudioVersion != null) {
                sb.AppendLine($"MinimumVisualStudioVersion = {header.MinimumVisualStudioVersion.ToString()}");
            }

            return sb.ToString(0, sb.Length - 2);
        }

        public WVisualStudioVersion(SlnHeader header)
        {
            if(header.FormatVersion == null) {
                throw new ArgumentNullException("Format Version is required.");
            }
            this.header = header;
        }
    }
}
