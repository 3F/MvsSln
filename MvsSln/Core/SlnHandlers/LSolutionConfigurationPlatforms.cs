/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LSolutionConfigurationPlatforms: LAbstract, ISlnHandler
    {
        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.SolutionConfPlatforms) == SlnItems.SolutionConfPlatforms);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("GlobalSection(SolutionConfigurationPlatforms)", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            if(svc.Sln.SolutionConfigList == null) {
                svc.Sln.SolutionConfigList = new List<IConfPlatform>();
            }

            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                string left = _line.Before('=')?.Trim(); // Debug|Win32 = Debug|Win32
                if(left == null 
                    || String.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    LSender.Send(this, $"Solution Configuration has been ignored for line '{_line}'", Message.Level.Warn);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) {
                    continue;
                }

                LSender.Send(this, $"Solution Configuration ->['{cfg[0]}' ; '{cfg[1]}']", Message.Level.Info);
                svc.Sln.SolutionConfigList.Add(new ConfigSln(cfg[0], cfg[1]));
            }

            return true;
        }
    }
}
