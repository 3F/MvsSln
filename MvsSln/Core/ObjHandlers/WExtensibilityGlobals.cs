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
    public class WExtensibilityGlobals: WAbstract, IObjHandler
    {
        protected IDictionary<string, string> items;

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        public override string Extract(object data)
        {
            if(items == null) {
                return String.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"{SP}GlobalSection(ExtensibilityGlobals) = postSolution");

            items.ForEach(i =>
                sb.AppendLine($"{SP}{SP}{i.Key}" + (i.Value != null ? $" = {i.Value}" : String.Empty))
            );

            sb.Append($"{SP}EndGlobalSection");
            return sb.ToString();
        }

        /// <param name="items">Extensible Key[-Value] records like `SolutionGuid` and so on.</param>
        public WExtensibilityGlobals(IDictionary<string, string> items)
        {
            this.items = items;
        }
    }
}
