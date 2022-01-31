/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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
