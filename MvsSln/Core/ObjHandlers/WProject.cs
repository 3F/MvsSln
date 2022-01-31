/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Text;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
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

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            foreach(var prj in projectItems)
            {
                sb.AppendLine(
                    $"Project(\"{prj.pType}\") = \"{prj.name}\", \"{prj.path}\", \"{prj.pGuid}\""
                );

                if(projectDependencies.Dependencies.ContainsKey(prj.pGuid) 
                    && projectDependencies.Dependencies[prj.pGuid].Count > 0)
                {
                    sb.AppendLine($"{SP}ProjectSection(ProjectDependencies) = postProject");

                        projectDependencies.Dependencies[prj.pGuid]
                                           .ForEach(dep => sb.AppendLine($"{SP}{SP}{dep} = {dep}"));

                    sb.AppendLine($"{SP}EndProjectSection");
                }

                sb.AppendLine("EndProject");
            }

            if(sb.Length > 1) {
                return sb.ToString(0, sb.Length - 2);
            }
            return String.Empty;
        }

        /// <param name="pItems">List of projects in solution.</param>
        /// <param name="deps">Solution Project Dependencies.</param>
        public WProject(IEnumerable<ProjectItem> pItems, ISlnProjectDependencies deps)
        {
            projectItems        = pItems ?? throw new ArgumentNullException();
            projectDependencies = deps ?? throw new ArgumentNullException();
        }
    }
}
