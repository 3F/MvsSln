﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    using static net.r_eg.MvsSln.Core.Keywords;

    public class WNestedProjects: WAbstract, IObjHandler
    {
        protected IEnumerable<SolutionFolder> folders;

        protected IEnumerable<ProjectItem> pItems;

        public override string Extract(object data)
        {
            if(folders == null && pItems == null) return null;

            lbuilder.Clear();
            lbuilder.AppendLv1Line(NestedProjectsPreSolution);
            bool hasDep = false;

            folders?.ForEach(p =>
            {
                if(p.header.parent?.Value != null)
                {
                    lbuilder.AppendLv2Line($"{p.header.pGuid} = {p.header.parent.Value?.header.pGuid}");
                    hasDep = true;
                }
            });

            pItems?.ForEach(p =>
            {
                if(p.parent?.Value != null)
                {
                    lbuilder.AppendLv2Line($"{p.pGuid} = {p.parent.Value?.header.pGuid}");
                    hasDep = true;
                }
            });

            if(!hasDep) return null;

            return lbuilder.AppendLv1(EndGlobalSection).ToString();
        }

        /// <inheritdoc cref="WNestedProjects(IEnumerable{SolutionFolder}, IEnumerable{ProjectItem})"/>
        public WNestedProjects(IEnumerable<SolutionFolder> folders)
            : this(folders, pItems: null)
        {

        }

        /// <inheritdoc cref="WNestedProjects(IEnumerable{SolutionFolder}, IEnumerable{ProjectItem})"/>
        public WNestedProjects(IEnumerable<ProjectItem> pItems)
            : this(folders: null, pItems)
        {

        }

        /// <param name="folders">Information about folders.</param>
        /// <param name="pItems">Information about project items.</param>
        public WNestedProjects(IEnumerable<SolutionFolder> folders, IEnumerable<ProjectItem> pItems)
        {
            this.folders    = folders;
            this.pItems     = pItems;
        }

        public WNestedProjects() { }
    }
}
