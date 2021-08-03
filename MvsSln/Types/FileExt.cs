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