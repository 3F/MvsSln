/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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

using System.Collections.Generic;
using System.Linq;

namespace net.r_eg.MvsSln.Core
{
    public static class Guids
    {
        /// <summary>
        /// Solution Folder.
        /// </summary>
        public const string SLN_FOLDER =     "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        public const string PROJECT_CS =     "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string PROJECT_DB =     "{C8D11400-126E-41CD-887F-60BD40844F9E}";
        public const string PROJECT_FS =     "{F2A71F9B-5D33-465A-A702-920D77279786}";
        public const string PROJECT_VB =     "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string PROJECT_VC =     "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        public const string PROJECT_VJ =     "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";
        public const string PROJECT_WD =     "{2CFEAB61-6A3B-4EB8-B523-560B4BEEF521}";
        public const string PROJECT_WEB =    "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string PROJECT_DEPLOY = "{151D2E53-A2C4-4D7D-83FE-D05416EBD58E}";
        public const string PROJECT_SF =     "{A07B5EB6-E848-4116-A8D0-A826331D98C6}";

        private static Dictionary<ProjectType, string> ProjectTypeGuids = new Dictionary<ProjectType, string>()
        {
            { ProjectType.Cs, PROJECT_CS },
            { ProjectType.Db, PROJECT_DB },
            { ProjectType.Fs, PROJECT_FS },
            { ProjectType.Vb, PROJECT_VB },
            { ProjectType.Vc, PROJECT_VC },
            { ProjectType.Vj, PROJECT_VJ },
            { ProjectType.Wd, PROJECT_WD },
            { ProjectType.Web, PROJECT_WEB },
            { ProjectType.SlnFolder, Guids.SLN_FOLDER },
            { ProjectType.Deploy, PROJECT_DEPLOY },
            { ProjectType.Sf, PROJECT_SF },
            { ProjectType.Unknown, null }
        };

        /// <summary>
        /// Evaluate project type via Guid.
        /// </summary>
        /// <param name="guid">Project type Guid.</param>
        /// <returns></returns>
        public static ProjectType ProjectTypeBy(string guid)
        {
            return ProjectTypeGuids.Where(p => p.Value == guid)
                    .Select(p => p.Key).First();
        }

        /// <summary>
        /// Evaluate Guid via ProjectType enum.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GuidBy(ProjectType type)
        {
            return ProjectTypeGuids[type];
        }
    }
}