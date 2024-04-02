/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public abstract class WAbstract: IObjHandler
    {
        [Obsolete("Use modern LineBuilder")]
        public const string SP = "\t";

        protected LineBuilder lbuilder = new();

        public string NewLine
        {
            get => lbuilder.NewLine;
            set => lbuilder.NewLine = value;
        }

        public abstract string Extract(object data);

        /// <summary>
        /// Gets unique id of listener.
        /// </summary>
        public Guid Id
        {
            get;
            protected set;
        }

        public WAbstract()
        {
            Id = GetType().GUID;
        }
    }
}
