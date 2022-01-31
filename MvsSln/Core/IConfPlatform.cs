/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    public interface IConfPlatform
    {
        /// <summary>
        /// Configured rule of the <see cref="Configuration"/> and <see cref="Platform"/> representations.
        /// </summary>
        IRuleOfConfig Rule { get; }

        /// <summary>
        /// Use "sensitivity" logic when comparing <see cref="IConfPlatform"/>
        /// together with `==` , `!=`.
        /// </summary>
        bool SensitivityComparing { get; set; }

        /// <summary>
        /// Configuration name.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// Configuration name using <see cref="Rule"/>.
        /// </summary>
        string ConfigurationByRule { get; }

        /// <summary>
        /// <see cref="ConfigurationByRule"/> with optional case insensitive logic.
        /// </summary>
        /// <remarks>Uses <see cref="SensitivityComparing"/> flag.</remarks>
        string ConfigurationByRuleICase { get; }

        /// <summary>
        /// Platform name.
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// Platform name using <see cref="Rule"/>.
        /// </summary>
        string PlatformByRule { get; }

        /// <summary>
        /// <see cref="PlatformByRule"/> with optional case insensitive logic.
        /// Uses <see cref="SensitivityComparing"/> flag.
        /// </summary>
        string PlatformByRuleICase { get; }

        /// <summary>
        /// Formatted final configuration.
        /// </summary>
        /// <remarks>Using <see cref="Rule"/>.</remarks>
        string Formatted { get; }

        /// <summary>
        /// Checking an config/platform by using <see cref="Rule"/> instance.
        /// </summary>
        /// <param name="config">Configuration name.</param>
        /// <param name="platform">Platform name.</param>
        /// <param name="icase">Case insensitive flag.</param>
        /// <returns></returns>
        bool IsEqualByRule(string config, string platform, bool icase = false);
    }
}
