/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class LSolutionConfigurationPlatforms: LAbstract, ISlnHandler
    {
        public override bool IsActivated(ISvc svc)
        {
            return (svc.Sln.ResultType & SlnItems.SolutionConfPlatforms) == SlnItems.SolutionConfPlatforms;
        }

        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith(SolutionConfigurationPlatforms, StringComparison.Ordinal);
        }

        public override bool Positioned(ISvc svc, RawText line)
        {
            if(svc.Sln.SolutionConfigList == null) svc.Sln.SolutionConfigList = [];

            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != EndGlobalSection)
            {
                string left = _line.Before('=')?.Trim(); // Debug|Win32 = Debug|Win32
                if(left == null 
                    || string.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    LSender.Send(this, $"Solution Configuration has been ignored for line '{_line}'", Message.Level.Warn);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) continue;

                LSender.Send(this, $"Solution Configuration ->['{cfg[0]}' ; '{cfg[1]}']", Message.Level.Info);
                svc.Sln.SolutionConfigList.Add(new ConfigSln(cfg[0], cfg[1]));
            }

            return true;
        }
    }
}
