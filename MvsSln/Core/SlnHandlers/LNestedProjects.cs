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
    using static net.r_eg.MvsSln.Core.Keywords;

    public class LNestedProjects: LAbstract, ISlnHandler
    {
        public override bool IsActivated(ISvc svc)
        {
            return (svc.Sln.ResultType & SlnItems.SolutionItems) == SlnItems.SolutionItems;
        }

        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith(NestedProjects, StringComparison.Ordinal);
        }

        public override bool Positioned(ISvc svc, RawText line)
        {
            string _line;
            while((_line = svc.ReadLine(this)) != null && _line.Trim() != EndGlobalSection)
            {
                int pos = _line.IndexOf('='); // Guids: src = dest
                if(pos < 0) {
                    LSender.Send(this, $"Incorrect NestedProjects records: '{_line}'", Message.Level.Warn);
                    return false;
                }

                string src  = _line.Substring(0, pos).Trim();
                string dest = _line.Substring(pos + 1).Trim();

                LSender.Send(this, $"{NestedProjects} '{src}' -> '{dest}'", Message.Level.Info);

                SolutionFolder parent = svc.Sln.SolutionFolderList.First(f => f.header.pGuid == dest);

                svc.Sln.SolutionFolderList.Where(f => f.header.pGuid == src)
                    .ForEach(f => f.header.parent.Value = parent);

                svc.Sln.ProjectItemList.Where(p => p.pGuid == src)
                    .ForEach(p => p.parent.Value = parent);
            }

            return true;
        }
    }
}
