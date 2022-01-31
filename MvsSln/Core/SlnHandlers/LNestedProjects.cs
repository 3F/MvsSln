/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Linq;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LNestedProjects: LAbstract, ISlnHandler
    {
        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.SolutionItems) == SlnItems.SolutionItems);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("GlobalSection(NestedProjects)", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != "EndGlobalSection")
            {
                int pos = _line.IndexOf('='); // Guids: src = dest
                if(pos < 0) {
                    LSender.Send(this, $"Incorrect NestedProjects records: '{_line}'", Message.Level.Warn);
                    return false;
                }

                string src  = _line.Substring(0, pos).Trim();
                string dest = _line.Substring(pos + 1).Trim();

                LSender.Send(this, $"NestedProjects '{src}' -> '{dest}'", Message.Level.Info);

                var parent = svc.Sln.SolutionFolderList.Where(f => f.header.pGuid == dest).First();

                svc.Sln.SolutionFolderList.Where(f => f.header.pGuid == src)
                    .ForEach(f => f.header.parent.Value = parent);

                svc.Sln.ProjectItemList.Where(p => p.pGuid == src)
                    .ForEach(p => p.parent.Value = parent);
            }

            return true;
        }
    }
}
