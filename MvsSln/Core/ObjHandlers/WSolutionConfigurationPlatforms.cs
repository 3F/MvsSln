/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Text;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public class WSolutionConfigurationPlatforms: WAbstract, IObjHandler
    {
        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        protected IEnumerable<IConfPlatform> configs;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{SP}GlobalSection(SolutionConfigurationPlatforms) = preSolution");

            configs.ForEach(cfg => sb.AppendLine($"{SP}{SP}{cfg} = {cfg}"));

            sb.Append($"{SP}EndGlobalSection");

            return sb.ToString();
        }

        /// <param name="configs">Solution configurations with platforms.</param>
        public WSolutionConfigurationPlatforms(IEnumerable<IConfPlatform> configs)
        {
            this.configs = configs ?? throw new ArgumentNullException();
        }
    }
}
