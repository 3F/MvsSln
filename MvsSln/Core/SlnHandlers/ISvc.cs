/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    using TransactSection = TransactTracking<ISection, IList<ISection>>;

    public interface ISvc
    {
        /// <summary>
        /// Used encoding for all data.
        /// </summary>
        Encoding CurrentEncoding { get; }

        /// <summary>
        /// Prepared solution data.
        /// </summary>
        ISlnResultSvc Sln { get; set; }

        /// <summary>
        /// Unspecified storage of the user scope.
        /// </summary>
        Dictionary<Guid, object> UData { get; set; }

        /// <summary>
        /// Reads a line of characters from stream.
        /// </summary>
        /// <returns></returns>
        string ReadLine();

        /// <summary>
        /// Reads a line of characters from stream with tracking.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        string ReadLine(object handler);

        /// <summary>
        /// Resets stream and its related data.
        /// </summary>
        void ResetStream();

        /// <summary>
        /// Tracking for line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="handler">Specific handler if used, or null as an unspecified.</param>
        ISection Track(RawText line, object handler = null);

        /// <summary>
        /// Transact tracking for line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="handler">Specific handler if used, or null as an unspecified.</param>
        /// <returns></returns>
        TransactSection TransactTrack(RawText line, object handler = null);

        /// <summary>
        /// Transact tracking for line.
        /// </summary>
        /// <param name="section">Provides requested section.</param>
        /// <param name="line"></param>
        /// <param name="handler">Specific handler if used, or null as an unspecified.</param>
        /// <returns></returns>
        TransactSection TransactTrack(out ISection section, RawText line, object handler = null);
    }
}
