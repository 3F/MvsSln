/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public class WProjectConfigurationPlatforms: WAbstract, IObjHandler
    {
        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatformPrj> configs;

        public override string Extract(object data)
        {
            lbuilder.Clear();

            lbuilder.AppendLv1Line("GlobalSection(ProjectConfigurationPlatforms) = postSolution");

            foreach(IConfPlatformPrj cfg in configs)
            {
                lbuilder.AppendLv2Line($"{cfg.PGuid}.{cfg.Sln}.ActiveCfg = {cfg}");

                if(cfg.IncludeInBuild)
                {
                    lbuilder.AppendLv2Line($"{cfg.PGuid}.{cfg.Sln}.Build.0 = {cfg}");
                }

                if(cfg.IncludeInDeploy)
                {
                    lbuilder.AppendLv2Line($"{cfg.PGuid}.{cfg.Sln}.Deploy.0 = {cfg}");
                }
            }

            return lbuilder.AppendLv1("EndGlobalSection").ToString();
        }

        /// <param name="configs">Project configurations with platforms.</param>
        public WProjectConfigurationPlatforms(IEnumerable<IConfPlatformPrj> configs)
        {
            this.configs = configs ?? throw new ArgumentNullException(nameof(configs));
        }
    }
}
