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
    public class LProject: LAbstract, ISlnHandler
    {
        /// <summary>
        /// Completeness of implementation.
        /// Aggregates additional handlers that will process same line.
        /// </summary>
        public override ICollection<Type> CoHandlers
        {
            get;
            protected set;
        } = new List<Type> { typeof(LProjectDependencies) };

        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.Projects) == SlnItems.Projects);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("Project(", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
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
