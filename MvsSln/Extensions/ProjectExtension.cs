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

using Microsoft.Build.Evaluation;

namespace net.r_eg.MvsSln.Extensions
{
    public static class ProjectExtension
    {
        public static bool IsEqual(this Project a, Project b)
        {
            if(a == null || b == null) {
                return a == b;
            }

            return a.FullPath == b.FullPath
                && a.GetConfig() == b.GetConfig()
                && a.GetPlatform() == b.GetPlatform()
                && a.GetProjectGuid() == b.GetProjectGuid()
                && a.GetProjectName() == b.GetProjectName();
        }

        public static string GetSolutionDir(this Project eProject)
        {
            return eProject?.GetPropertyValue(PropertyNames.SLN_DIR);
        }

        public static string GetProjectRelativePath(this Project eProject)
        {
            if(eProject == null) {
                return null;
            }
            return eProject.GetSolutionDir().MakeRelativePath(eProject.FullPath);
        }

        public static string GetConfig(this Project eProject)
        {
            return eProject?.GetPropertyValue(PropertyNames.CONFIG);
        }

        public static string GetPlatform(this Project eProject)
        {
            return eProject?.GetPropertyValue(PropertyNames.PLATFORM);
        }

        public static string GetSolutionExt(this Project eProject)
        {
            return eProject?.GetPropertyValue(PropertyNames.SLN_EXT);
        }

        public static string GetProjectGuid(this Project eProject)
        {
            //TODO: return null if empty (not defined) to 3.0
            return eProject?.GetPropertyValue(PropertyNames.PRJ_GUID)/*.NullIfEmpty()*/;
        }

        public static string GetProjectName(this Project eProject)
        {
            //TODO: return null if empty (not defined) to 3.0
            //NOTE: this property can also define an unified project name between various .sln files (_2010.sln, _2017.sln)
            return eProject?.GetPropertyValue(PropertyNames.PRJ_NAME)/*.NullIfEmpty()*/;
        }
    }
}