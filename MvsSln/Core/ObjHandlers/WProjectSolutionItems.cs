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

    public class WProjectSolutionItems: WAbstract, IObjHandler
    {
        /// <summary>
        /// List of solution folders.
        /// </summary>
        protected IEnumerable<SolutionFolder> folders;

        public override string Extract(object data)
        {
            lbuilder.Clear();

            foreach(SolutionFolder folder in folders)
            {
                ProjectItem prj = folder.header;

                lbuilder.AppendLine
                (
                    $"{Project_}\"{prj.pType}\") = \"{prj.name}\", \"{prj.path}\", \"{prj.pGuid}\""
                )
                .AppendLv1Line(SolutionItemsPreProject);

                {
                    folder.items.ForEach(item => lbuilder.AppendLv2Line($"{item} = {item}"));
                }

                lbuilder.AppendLv1Line(EndProjectSection).AppendLine(EndProject);
            }

            return lbuilder.ToString(removeNewLine: true);
        }

        /// <param name="folders">List of solution folders.</param>
        public WProjectSolutionItems(IEnumerable<SolutionFolder> folders)
        {
            this.folders = folders ?? throw new ArgumentNullException(nameof(folders));
        }
    }
}
