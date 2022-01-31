/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Projects
{
    internal static class PackagesConfigLocator
    {
        internal const string DIR = "packages";

        public static IEnumerable<PackagesConfig> FindAndLoadConfigs(ISlnResult sln, SlnItems items)
            => FindConfigs(sln, items)
                .Select(c => new PackagesConfig(c, PackagesConfigOptions.Load 
                                                    | PackagesConfigOptions.PathToStorage 
                                                    | PackagesConfigOptions.SilentLoading));

        public static IEnumerable<string> FindConfigs(ISlnResult sln, SlnItems items)
            => FindAllConfigs(sln, items).Distinct();

        private static IEnumerable<string> FindAllConfigs(ISlnResult sln, SlnItems items)
        {
            if(sln == null) throw new ArgumentNullException(nameof(sln));

            if(items.HasFlag(SlnItems.PackagesConfigSolution))
            {
                foreach(var config in FindSolutionConfigs(sln, items)) yield return config;
            }

            if(items.HasFlag(SlnItems.PackagesConfigLegacy))
            {
                foreach(var config in FindLegacyConfigs(sln)) yield return config;
            }
        }

        private static IEnumerable<string> FindSolutionConfigs(ISlnResult sln, SlnItems items)
        {
            string dfile = Path.GetFullPath(Path.Combine(sln.SolutionDir, PackagesConfig.FNAME));
            if(File.Exists(dfile)) yield return dfile;

            if(sln.SolutionFolders != null)
            foreach(RawText file in sln.SolutionFolders.SelectMany(f => f.items))
            {
                if(!file.trimmed.EndsWith(PackagesConfig.FNAME)) continue;

                string input = Path.GetFullPath(Path.Combine(sln.SolutionDir, file));
                if(File.Exists(input)) yield return input;
            }
        }

        private static IEnumerable<string> FindLegacyConfigs(ISlnResult sln)
        {
            string dfile = Path.GetFullPath(Path.Combine(sln.SolutionDir, DIR, PackagesConfig.FNAME));
            if(File.Exists(dfile)) yield return dfile;

            if(sln.ProjectItems != null)
            foreach(var prj in sln.ProjectItems)
            {
                string input = Path.Combine(Path.GetDirectoryName(prj.fullPath), PackagesConfig.FNAME);
                if(File.Exists(input)) yield return input;
            }
        }
    }
}
