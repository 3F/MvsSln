/*!
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

    public class WExtensibilityGlobals: WAbstract, IObjHandler
    {
        protected IDictionary<string, string> items;

        public override string Extract(object data)
        {
            if(items == null) return null;

            lbuilder.Clear();
            lbuilder.AppendLv1Line(ExtensibilityGlobalsPostSolution);

            items.ForEach(i =>
                lbuilder.AppendLv2Line($"{i.Key}" + (i.Value != null ? $" = {i.Value}" : string.Empty))
            );

            return lbuilder.AppendLv1(EndGlobalSection).ToString();
        }

        /// <param name="items">Extensible Key[-Value] records like `SolutionGuid` and so on.</param>
        public WExtensibilityGlobals(IDictionary<string, string> items)
        {
            this.items = items;
        }

        public WExtensibilityGlobals() { }
    }
}
