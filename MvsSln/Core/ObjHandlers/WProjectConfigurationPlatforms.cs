/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public class WProjectConfigurationPlatforms: WAbstract, IObjHandler
    {
        /// <summary>
        /// Project configurations with platforms.
        /// </summary>
        public IEnumerable<IConfPlatformPrj> configs;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{SP}GlobalSection(ProjectConfigurationPlatforms) = postSolution");

            foreach(var cfg in configs)
            {
                sb.AppendLine($"{SP}{SP}{cfg.PGuid}.{cfg.Sln}.ActiveCfg = {cfg}");
                if(cfg.IncludeInBuild) {
                    sb.AppendLine($"{SP}{SP}{cfg.PGuid}.{cfg.Sln}.Build.0 = {cfg}");
                }

                if (cfg.IncludeInDeploy) {
                    sb.AppendLine($"{SP}{SP}{cfg.PGuid}.{cfg.Sln}.Deploy.0 = {cfg}");
                }
            }

            sb.Append($"{SP}EndGlobalSection");

            return sb.ToString();
        }

        /// <param name="configs">Project configurations with platforms.</param>
        public WProjectConfigurationPlatforms(IEnumerable<IConfPlatformPrj> configs)
        {
            this.configs = configs ?? throw new ArgumentNullException();
        }
    }
}
