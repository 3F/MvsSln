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
    public class WNestedProjects: WAbstract, IObjHandler
    {
        protected IEnumerable<SolutionFolder> folders;

        protected IEnumerable<ProjectItem> pItems;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{SP}GlobalSection(NestedProjects) = preSolution");
            bool hasDep = false;

            folders?.ForEach(p =>
            {
                if(p.header.parent?.Value != null)
                {
                    sb.AppendLine($"{SP}{SP}{p.header.pGuid} = {p.header.parent.Value?.header.pGuid}");
                    hasDep = true;
                }
            });

            pItems?.ForEach(p =>
            {
                if(p.parent?.Value != null)
                {
                    sb.AppendLine($"{SP}{SP}{p.pGuid} = {p.parent.Value?.header.pGuid}");
                    hasDep = true;
                }
            });

            if(!hasDep) {
                return String.Empty;
            }

            sb.Append($"{SP}EndGlobalSection");
            return sb.ToString();
        }

        /// <param name="folders">Information about folders.</param>
        public WNestedProjects(IEnumerable<SolutionFolder> folders)
            : this(folders, null)
        {

        }

        /// <param name="pItems">Information about project items.</param>
        public WNestedProjects(IEnumerable<ProjectItem> pItems)
            : this(null, pItems)
        {

        }

        /// <param name="folders">Information about folders.</param>
        /// <param name="pItems">Information about project items.</param>
        public WNestedProjects(IEnumerable<SolutionFolder> folders, IEnumerable<ProjectItem> pItems)
        {
            this.folders    = folders;
            this.pItems     = pItems;
        }
    }
}
