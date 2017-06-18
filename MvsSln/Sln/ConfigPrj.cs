/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;

namespace net.r_eg.vsSBE.Sln
{
    /// <summary>
    /// Project Configuration
    /// </summary>
    [DebuggerDisplay("{format()} [{pGuid}]")]
    public struct ConfigPrj
    {
        /// <summary>
        /// Project Guid
        /// </summary>
        public string pGuid;

        public string configuration;

        public string platform;

        public bool includeInBuild;

        public ConfigSln sln;

        public static string Format(string configuration, string platform)
        {
            return $"{configuration}|{platform}";
        }

        public string format()
        {
            return Format(configuration, platform);
        }

        public ConfigPrj(string formatted)
            : this()
        {
            string[] cfg = formatted.Split('|');
            if(cfg.Length < 2) {
                throw new ArgumentException($"The format `{formatted}` of configuration is not supported.");
            }

            configuration   = cfg[0];
            platform        = cfg[1];
        }
    }
}
