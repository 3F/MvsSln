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
    public class WProjectSolutionItems: WAbstract, IObjHandler
    {
        /// <summary>
        /// List of solution folders.
        /// </summary>
        protected IEnumerable<SolutionFolder> folders;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            foreach(var folder in folders)
            {
                var prj = folder.header;

                sb.AppendLine(
                    $"Project(\"{prj.pType}\") = \"{prj.name}\", \"{prj.path}\", \"{prj.pGuid}\""
                );

                sb.AppendLine($"{SP}ProjectSection(SolutionItems) = preProject");

                    folder.items.ForEach(item => sb.AppendLine($"{SP}{SP}{item} = {item}"));

                sb.AppendLine($"{SP}EndProjectSection");

                sb.AppendLine("EndProject");
            }

            if(sb.Length > 1) {
                return sb.ToString(0, sb.Length - 2);
            }
            return String.Empty;
        }

        /// <param name="folders">List of solution folders.</param>
        public WProjectSolutionItems(IEnumerable<SolutionFolder> folders)
        {
            this.folders = folders ?? throw new ArgumentNullException();
        }
    }
}
