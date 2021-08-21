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
