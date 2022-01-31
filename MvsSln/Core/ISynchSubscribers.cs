/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public interface ISynchSubscribers<T>: IEnumerable, IEnumerable<T>
        where T: IHandler
    {
        /// <summary>
        /// Number of elements contained in the thread-safe collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the object used to synchronize access to the thread-safe collection.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Adds an listener to thread-safe collection.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Register(T listener);

        /// <summary>
        /// Removes specified listener from the collection.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Unregister(T listener);

        /// <summary>
        /// Reset all collection.
        /// </summary>
        void Reset();

        /// <summary>
        /// Determines whether the collection contains an listener.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Contains(T listener);

        /// <summary>
        /// Checks existence of listener by Guid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(Guid id);

        /// <summary>
        /// Get listener by specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>null if not found.</returns>
        T GetById(Guid id);
    }
}
