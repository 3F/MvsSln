/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln.Extensions;

// TODO: move to '.Types' namespace and split ProjectType processing into new additional type.
namespace net.r_eg.MvsSln.Core
{
    public static class Guids
    {
        /// <summary>
        /// Solution Folder.
        /// </summary>
        public const string SLN_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        /// <summary>
        /// .csproj
        /// </summary>
        public const string PROJECT_CS = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        /// <summary>
        /// .dbproj
        /// </summary>
        public const string PROJECT_DB = "{C8D11400-126E-41CD-887F-60BD40844F9E}";

        /// <summary>
        /// .fsproj
        /// </summary>
        public const string PROJECT_FS = "{F2A71F9B-5D33-465A-A702-920D77279786}";

        /// <summary>
        /// .vbproj
        /// </summary>
        public const string PROJECT_VB = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";

        /// <summary>
        /// .vcxproj
        /// </summary>
        public const string PROJECT_VC = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";

        /// <summary>
        /// .vjsproj
        /// </summary>
        public const string PROJECT_VJ = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";

        /// <summary>
        /// .wdproj
        /// </summary>
        public const string PROJECT_WD = "{2CFEAB61-6A3B-4EB8-B523-560B4BEEF521}";

        /// <summary>
        /// 
        /// </summary>
        public const string PROJECT_WEB = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";

        /// <summary>
        /// .deployproj
        /// </summary>
        public const string PROJECT_DEPLOY = "{151D2E53-A2C4-4D7D-83FE-D05416EBD58E}";

        /// <summary>
        /// .sfproj
        /// </summary>
        public const string PROJECT_SF = "{A07B5EB6-E848-4116-A8D0-A826331D98C6}";

        /// <summary>
        /// .fsproj SDK based type.
        /// https://github.com/dotnet/project-system/blob/master/docs/opening-with-new-project-system.md
        /// </summary>
        public const string PROJECT_FS_SDK = "{6EC3EE1D-3C4E-46DD-8F32-0CC8E7565705}";

        /// <summary>
        /// .vbproj SDK based type.
        /// https://github.com/dotnet/project-system/blob/master/docs/opening-with-new-project-system.md
        /// </summary>
        public const string PROJECT_VB_SDK = "{778DAE3C-4631-46EA-AA77-85C1314464D9}";

        /// <summary>
        /// .csproj SDK based type.
        /// https://github.com/dotnet/project-system/blob/master/docs/opening-with-new-project-system.md
        /// </summary>
        public const string PROJECT_CS_SDK = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";

        private readonly static Dictionary<ProjectType, string> projectTypeGuids = new Dictionary<ProjectType, string>()
        {
            { ProjectType.Cs, PROJECT_CS },
            { ProjectType.Db, PROJECT_DB },
            { ProjectType.Fs, PROJECT_FS },
            { ProjectType.Vb, PROJECT_VB },
            { ProjectType.Vc, PROJECT_VC },
            { ProjectType.Vj, PROJECT_VJ },
            { ProjectType.Wd, PROJECT_WD },
            { ProjectType.Web, PROJECT_WEB },
            { ProjectType.SlnFolder, SLN_FOLDER },
            { ProjectType.Deploy, PROJECT_DEPLOY },
            { ProjectType.Sf, PROJECT_SF },
            { ProjectType.FsSdk, PROJECT_FS_SDK },
            { ProjectType.VbSdk, PROJECT_VB_SDK },
            { ProjectType.CsSdk, PROJECT_CS_SDK },
            { ProjectType.Unknown, null }
        };

        /// <summary>
        /// Evaluate project type via Guid.
        /// </summary>
        /// <param name="guid">Project type Guid.</param>
        /// <returns></returns>
        public static ProjectType ProjectTypeBy(string guid)
        {
            return projectTypeGuids.FirstOrDefault(p => p.Value == guid).Key;
        }

        /// <summary>
        /// Evaluate Guid via ProjectType enum.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GuidBy(ProjectType type)
        {
            return projectTypeGuids.GetOrDefault(type);
        }
    }
}