/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
