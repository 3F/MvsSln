/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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