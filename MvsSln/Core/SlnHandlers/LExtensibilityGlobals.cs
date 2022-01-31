/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LExtensibilityGlobals: LAbstract, ISlnHandler
    {
        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.ExtItems) == SlnItems.ExtItems);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("GlobalSection(ExtensibilityGlobals)", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            if(svc.Sln.ExtItems == null) {
                svc.Sln.ExtItems = new Dictionary<string, string>();
            }

            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                int pos = _line.IndexOf('=');
                if(pos < 0) // we will use non-strict processing
                {
                    svc.Sln.ExtItems[_line.Trim()] = null;
                    LSender.Send(this, $"Found extensible null record:{_line}", Message.Level.Info);
                    continue;
                }

                string key  = _line.Substring(0, pos).Trim();
                string val  = _line.Substring(pos + 1).Trim();

                svc.Sln.ExtItems[key] = val;
                LSender.Send(this, $"Found extensible key-value: `{key}` = `{val}`", Message.Level.Info);
            }

            return true;
        }
    }
}
