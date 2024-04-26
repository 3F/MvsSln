/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public interface IProjectItemsHandler: IObjHandler
    {
        /// <summary>
        /// Access to <see cref="ProjectItem"/> records at <see cref="IObjHandler"/>.
        /// </summary>
        IEnumerable<ProjectItem> Projects { get; }
    }
}
