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
        /// The custom rule of the Configuration and Platform names.
        /// </summary>
        IRuleOfConfig Rule { get; }

        /// <summary>
        /// To use an `Sensitivity` logic when comparing {IConfPlatform}
        /// together with `==` , `!=`.
        /// </summary>
        bool SensitivityComparing { get; set; }

        string Configuration { get; }

        string ConfigurationByRule { get; }

        /// <summary>
        /// {ConfigurationByRule} with optional case insensitive logic.
        /// Uses {SensitivityComparing} flag.
        /// </summary>
        string ConfigurationByRuleICase { get; }

        string Platform { get; }

        string PlatformByRule { get; }

        /// <summary>
        /// {PlatformByRule} with optional case insensitive logic.
        /// Uses {SensitivityComparing} flag.
        /// </summary>
        string PlatformByRuleICase { get; }

        /// <summary>
        /// Checking an config/platform by using {Rule} instance.
        /// </summary>
        /// <param name="config">Configuration name.</param>
        /// <param name="platform">Platform name.</param>
        /// <param name="icase">Case insensitive flag.</param>
        /// <returns></returns>
        bool IsEqualByRule(string config, string platform, bool icase = false);
    }
}
