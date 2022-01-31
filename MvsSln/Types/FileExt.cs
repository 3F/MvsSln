/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Types
{
    public static class FileExt
    {
        private static Dictionary<ProjectType, string> projectTypeFileExt = new Dictionary<ProjectType, string>()
        {
            { ProjectType.Cs, ".csproj" },
            { ProjectType.Db, ".dbproj" },
            { ProjectType.Fs, ".fsproj" },
            { ProjectType.Vb, ".vbproj" },
            { ProjectType.Vc, ".vcxproj" },
            { ProjectType.Vj, ".vjsproj" },
            { ProjectType.Wd, ".wdproj" },
            { ProjectType.Deploy, ".deployproj" },
            { ProjectType.Sf, ".sfproj" },
            { ProjectType.Unknown, null }
        };

        /// <summary>
        /// Evaluate project type via its file.
        /// </summary>
        /// <param name="file">File name with extension.</param>
        /// <returns></returns>
        public static ProjectType GetProjectTypeByFile(string file)
        {
            if(string.IsNullOrWhiteSpace(file)) {
                return ProjectType.Unknown;
            }
            return GetProjectTypeByExt(Path.GetExtension(file));
        }

        /// <summary>
        /// Evaluate project type via its file extension.
        /// </summary>
        /// <param name="ext">File extension.</param>
        /// <returns></returns>
        public static ProjectType GetProjectTypeByExt(string ext)
        {
            return projectTypeFileExt.FirstOrDefault(p => p.Value == ext).Key;
        }

        /// <summary>
        /// Evaluate file extension via ProjectType enum.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileExtensionBy(ProjectType type)
        {
            return projectTypeFileExt.GetOrDefault(type);
        }
    }
}