/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
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
