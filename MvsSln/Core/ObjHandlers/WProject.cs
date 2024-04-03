/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class WProject: WAbstract, IObjHandler
    {
        /// <summary>
        /// All found projects in solution.
        /// </summary>
        protected IEnumerable<ProjectItem> projectItems;

        /// <summary>
        /// Solution Project Dependencies.
        /// </summary>
        protected ISlnProjectDependencies projectDependencies;

        public override string Extract(object data)
        {
            lbuilder.Clear();

            foreach(ProjectItem prj in projectItems)
            {
                lbuilder.AppendLine
                (
                    $"{Project_}\"{prj.pType}\") = \"{prj.name}\", \"{prj.path}\", \"{prj.pGuid}\""
                );

                if(projectDependencies.Dependencies.ContainsKey(prj.pGuid) 
                    && projectDependencies.Dependencies[prj.pGuid].Count > 0)
                {
                    lbuilder.AppendLv1Line(ProjectDependenciesPostProject);

                    {
                        projectDependencies.Dependencies[prj.pGuid]
                                           .ForEach(dep => lbuilder.AppendLv2Line($"{dep} = {dep}"));
                    }

                    lbuilder.AppendLv1Line(EndProjectSection);
                }

                lbuilder.AppendLine(EndProject);
            }

            return lbuilder.ToString(noLastNewLine: true);
        }

        /// <param name="pItems">List of projects in solution.</param>
        /// <param name="deps">Solution Project Dependencies.</param>
        public WProject(IEnumerable<ProjectItem> pItems, ISlnProjectDependencies deps)
        {
            projectItems        = pItems ?? throw new ArgumentNullException(nameof(pItems));
            projectDependencies = deps ?? throw new ArgumentNullException(nameof(deps));
        }
    }
}
