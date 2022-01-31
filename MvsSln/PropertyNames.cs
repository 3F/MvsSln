/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln
{
    /// <summary>
    /// Regular MSBuild Properties and related.
    /// </summary>
    public static class PropertyNames
    {
        /// <summary>
        /// Any undefined properties.
        /// </summary>
        public const string UNDEFINED       = "*Undefined*";

        public const string CONFIG          = "Configuration";
        public const string PLATFORM        = "Platform";

        public const string SLN_NAME        = "SolutionName";
        public const string SLN_DIR         = "SolutionDir";
        public const string SLN_EXT         = "SolutionExt";
        public const string SLN_FNAME       = "SolutionFileName";
        public const string SLN_PATH        = "SolutionPath";

        public const string PRJ_NAME        = "ProjectName";
        public const string PRJ_GUID        = "ProjectGuid";

        /// <summary>
        /// Platform Target for binaries.
        /// </summary>
        public const string PRJ_PLATFORM    = "PlatformTarget";

        /// <summary>
        /// Used namespace for project.
        /// </summary>
        public const string PRJ_NAMESPACE   = "RootNamespace";

        /// <summary>
        /// Default for VS env.
        /// __VSSPROPID Enumeration (Microsoft.VisualStudio.Shell.Interop)
        /// </summary>
        public const string DEVENV_DIR      = "DevEnvDir";

        /// <summary>
        /// Default for VS env:
        /// "true" (can be changed in other components) in Microsoft.VisualStudio.Project.ProjectNode 
        /// :: DoMSBuildSubmission + SetupProjectGlobalPropertiesThatAllProjectSystemsMustSet
        /// </summary>
        public const string VS_BUILD        = "BuildingInsideVisualStudio";

        /// <summary>
        /// Default for VS env:
        /// "false" in Microsoft.VisualStudio.Package.GlobalPropertyHandler
        /// </summary>
        public const string CODE_ANAL_ORUN  = "RunCodeAnalysisOnce";
    }
}
