/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    public interface ISection
    {
        /// <summary>
        /// Contains handler which is ready to process this section, or already processes.
        /// </summary>
        object Handler { get; }

        /// <summary>
        /// Known line number to this section.
        /// </summary>
        long Line { get; }

        /// <summary>
        /// Raw data from stream.
        /// </summary>
        RawText Raw { get; }

        /// <summary>
        /// To ignore this from other sections.
        /// </summary>
        bool Ignore { get; set; }

        /// <summary>
        /// User's mixed object for anything.
        /// </summary>
        object User { get; set; }

        /// <summary>
        /// To update handler which is ready to process this section.
        /// </summary>
        /// <param name="handler">New handler.</param>
        void UpdateHandler(object handler);

        /// <summary>
        /// Clone data from this section into new ISection instance.
        /// </summary>
        /// <returns></returns>
        ISection Clone();
    }
}
