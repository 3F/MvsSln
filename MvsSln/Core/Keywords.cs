/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    internal sealed class Keywords
    {
        internal const string GlobalSection = "GlobalSection";

        internal const string EndGlobalSection = "EndGlobalSection";

        internal const string ProjectSection = "ProjectSection";

        internal const string EndProjectSection = "EndProjectSection";

        internal const string preProject = "preProject";

        internal const string postProject = "postProject";

        internal const string preSolution = "preSolution";

        internal const string postSolution = "postSolution";

        internal const string Project_ = "Project(";

        internal const string EndProject = "EndProject";

        internal const string Global = "Global";

        internal const string EndGlobal = "EndGlobal";

        internal const string SolutionConfigurationPlatforms = GlobalSection + "(SolutionConfigurationPlatforms)";

        internal const string SolutionItems = ProjectSection + "(SolutionItems)";

        internal const string ExtensibilityGlobals = GlobalSection + "(ExtensibilityGlobals)";

        internal const string NestedProjects = GlobalSection + "(NestedProjects)";

        internal const string ProjectDependencies = ProjectSection + "(ProjectDependencies)";

        internal const string ProjectConfigurationPlatforms = GlobalSection + "(ProjectConfigurationPlatforms)";

        internal const string MinimumVisualStudioVersion = "MinimumVisualStudioVersion";

        internal const string VisualStudioVersion = "VisualStudioVersion";

        internal const string SolutionItemsPreProject = $"{SolutionItems} = {preProject}";

        internal const string ProjectDependenciesPostProject = $"{ProjectDependencies} = {postProject}";

        internal const string NestedProjectsPreSolution = $"{NestedProjects} = {preSolution}";

        internal const string SolutionConfigurationPlatformsPreSolution = $"{SolutionConfigurationPlatforms} = {preSolution}";

        internal const string ExtensibilityGlobalsPostSolution = $"{ExtensibilityGlobals} = {postSolution}";

        internal const string ProjectConfigurationPlatformsPostSolution = $"{ProjectConfigurationPlatforms} = {postSolution}";
    }
}
