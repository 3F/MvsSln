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
        /// Specifies the EOL character (or a sequence of characters) for this <see cref="IObjHandler"/>.
        /// </summary>
        /// <remarks>Platform independent. Alternatively see <see cref="System.Environment.NewLine"/></remarks>
        string NewLine { get; set; }

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this <see cref="IObjHandler"/>.</param>
        /// <returns>
        /// A piece of final data from the current <see cref="IObjHandler"/> implementation. <br/>
        /// null should be considered as non-processed or ignored result.
        /// </returns>
        string Extract(object data);
    }
}
