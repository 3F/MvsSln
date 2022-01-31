/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Text.RegularExpressions;

namespace net.r_eg.MvsSln.Core
{
    public sealed class RPatterns
    {
        /// <summary>
        /// Pattern of 'Project(' line - based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        public static Regex ProjectLine
        {
            get;
        } = new Regex("^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern of 'ProjectSection(ProjectDependencies)' lines - based on crackPropertyLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        public static Regex PropertyLine
        {
            get;
        } = new Regex("^(?<PName>[^=]*)\\s*=\\s*(?<PValue>[^=]*)$", RegexOptions.Compiled);
    }
}