/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class WVisualStudioVersion: WAbstract, IObjHandler
    {
        /// <summary>
        /// Header information.
        /// </summary>
        protected SlnHeader header;

        public override string Extract(object data)
        {
            lbuilder.Clear();

            string fmt = string.Format("{0}.{1:00}", header.FormatVersion.Major, header.FormatVersion.Minor);
            lbuilder.AppendLine($"Microsoft Visual Studio Solution File, Format Version {fmt}");

            if(header.ProgramVersion != null)
            {
                lbuilder.AppendLine($"# Visual Studio {header.ProgramVersion}");
            }

            if(header.VisualStudioVersion != null)
            {
                lbuilder.AppendLine($"{VisualStudioVersion} = {header.VisualStudioVersion}");
            }

            if(header.MinimumVisualStudioVersion != null)
            {
                lbuilder.AppendLine($"{MinimumVisualStudioVersion} = {header.MinimumVisualStudioVersion}");
            }

            return lbuilder.ToString(removeNewLine: true);
        }

        public WVisualStudioVersion(SlnHeader header)
        {
            if(header.FormatVersion == null) {
                throw new ArgumentNullException(MsgR._0_IsRequired.Format(nameof(header.FormatVersion)));
            }
            this.header = header;
        }
    }
}
