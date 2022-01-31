/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public interface IObjHandler: IHandler
    {
        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
        string Extract(object data);
    }
}
