/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class WSolutionConfigurationPlatforms: WAbstract, IObjHandler
    {
        /// <summary>
        /// Solution configurations with platforms.
        /// </summary>
        protected IEnumerable<IConfPlatform> configs;

        public override string Extract(object data)
        {
            if(configs == null) return null;

            lbuilder.Clear();
            lbuilder.AppendLv1Line(SolutionConfigurationPlatformsPreSolution);

            configs.ForEach(cfg => lbuilder.AppendLv2Line($"{cfg} = {cfg}"));

            return lbuilder.AppendLv1(EndGlobalSection).ToString();
        }

        /// <param name="configs">Solution configurations with platforms.</param>
        public WSolutionConfigurationPlatforms(IEnumerable<IConfPlatform> configs)
        {
            this.configs = configs;
        }

        public WSolutionConfigurationPlatforms() { }
    }
}
