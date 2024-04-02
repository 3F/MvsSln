/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class LProject: LAbstract, ISlnHandler
    {
        public override ICollection<Type> CoHandlers { get; protected set; }
            = [typeof(LProjectDependencies)];

        public override bool IsActivated(ISvc svc)
        {
            return (svc.Sln.ResultType & SlnItems.Projects) == SlnItems.Projects;
        }

        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith(Project_, StringComparison.Ordinal);
        }

        public override bool Positioned(ISvc svc, RawText line)
        {
            var pItem = GetProjectItem(line.trimmed, svc.Sln.SolutionDir);
            if(pItem.pGuid == null) {
                return false;
            }

            if(svc.Sln.ProjectItemList == null) {
                svc.Sln.ProjectItemList = new List<ProjectItem>();
            }

            svc.Sln.ProjectItemList.Add(pItem);
            return true;
        }
    }
}
