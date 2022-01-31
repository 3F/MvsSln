/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{DbgDisplay}")]
    public class Section: ISection
    {
        /// <summary>
        /// Contains handler which is ready to process this section, or already processes.
        /// </summary>
        public object Handler
        {
            get;
            protected set;
        }

        /// <summary>
        /// Known line number to this section.
        /// </summary>
        public long Line
        {
            get;
            protected set;
        }

        /// <summary>
        /// Raw data from stream.
        /// </summary>
        public RawText Raw
        {
            get;
            protected set;
        }

        /// <summary>
        /// To ignore this from other sections.
        /// </summary>
        public bool Ignore
        {
            get;
            set;
        }

        /// <summary>
        /// User's mixed object for anything.
        /// </summary>
        public object User
        {
            get;
            set;
        }

        /// <summary>
        /// To update handler which is ready to process this section.
        /// </summary>
        /// <param name="handler">New handler.</param>
        public void UpdateHandler(object handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// Clone data from this section into new ISection instance.
        /// </summary>
        /// <returns></returns>
        public ISection Clone()
        {
            return new Section(Handler, Raw, Line) {
                Ignore  = Ignore,
                User    = User
            };
        }

        public Section(object h, RawText raw, long line = -1)
        {
            Handler = h;
            Raw     = raw;
            Line    = line;
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => $"{(Ignore ? "x: " : "")}[{Handler?.GetType().Name}] #{Line}:'{Raw}'";
        }

        #endregion
    }
}
