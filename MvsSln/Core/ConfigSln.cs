/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Solution Configuration
    /// </summary>
    public class ConfigSln: ConfigItem, IConfPlatform
    {
        public ConfigSln(string configuration, string platform)
            : base(configuration, platform)
        {

        }

        public ConfigSln(string formatted)
            : base(formatted)
        {

        }
    }
}
