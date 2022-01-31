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
        /// <summary>
        /// Default indent.
        /// </summary>
        public const string SP = "\t";

        /// <summary>
        /// To extract prepared raw-data.
        /// </summary>
        /// <param name="data">Any object data which is ready for this IObjHandler.</param>
        /// <returns>Final part of sln data.</returns>
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
