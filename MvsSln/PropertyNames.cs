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
