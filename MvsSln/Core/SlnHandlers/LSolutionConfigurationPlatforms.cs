/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LSolutionConfigurationPlatforms: LAbstract, ISlnHandler
    {
        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(Svc svc, RawText line)
        {
            if((svc.Sln.ResultType & SlnItems.SolutionConfPlatforms) != SlnItems.SolutionConfPlatforms) {
                return false;
            }

            if(!line.trimmed.StartsWith("GlobalSection(SolutionConfigurationPlatforms)", StringComparison.Ordinal)) {
                return false;
            }

            if(svc.Sln.SolutionConfigList == null) {
                svc.Sln.SolutionConfigList = new List<IConfPlatform>();
            }

            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                string left = _line.Split('=')[0].Trim(); // Debug|Win32 = Debug|Win32
                if(string.Compare(left, "DESCRIPTION", StringComparison.OrdinalIgnoreCase) == 0) {
                    LSender.Send(this, $"SolutionParser: Solution Configuration has been ignored for line '{_line}'", Message.Level.Debug);
                    continue;
                }

                string[] cfg = left.Split('|');
                if(cfg.Length < 2) {
                    continue;
                }

                LSender.Send(this, $"Solution Configuration ->['{cfg[0]}' ; '{cfg[1]}']", Message.Level.Trace);
                svc.Sln.SolutionConfigList.Add(new ConfigSln(cfg[0], cfg[1]));
            }

            return true;
        }
    }
}
