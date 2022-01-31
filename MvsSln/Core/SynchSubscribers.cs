/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Thread-safe container of listeners.
    /// </summary>
    /// <typeparam name="T">IListener based type.</typeparam>
    public class SynchSubscribers<T>: ISynchSubscribers<T>
        where T : IHandler
    {
        /// <summary>
        /// justification: A common using of SynchSubscribers should be as an only sequential accessing to all elements at once - that is O(1).
        ///                And most important - it's contiguous storage in order of adding of elements, because we need to save priority by listeners.
        /// But for any single accessing it should be O(n), thus we also use O(1) accessor below to improve performance of the List type.
        /// </summary>
        protected List<T> listeners = new List<T>();

        /// <summary>
        /// A shallow copy of listeners which has O(1) for any single accessing to elements.
        /// This is not an ordered, thread-safe container, and unfortunately we can't use this as primarily container (read justification above).
        /// </summary>
        protected ConcurrentDictionary<Guid, T> accessor = new ConcurrentDictionary<Guid, T>();

        private object sync = new object();

        /// <summary>
        /// Number of elements contained in the thread-safe collection.
        /// </summary>
        public int Count
        {
            get
            {
                lock(sync)
                {
                    return listeners.Count;
                }
            }
        }

        /// <summary>
        /// Gets the object used to synchronize access to the thread-safe collection.
        /// </summary>
        public object SyncRoot
        {
            get => sync;
        }

        /// <summary>
        /// Adds an listener to thread-safe collection.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Register(T listener)
        {
            lock(sync)
            {
                if(Contains(listener)) {
                    return false;
                }

                //items.Insert(Count, listener);
                listeners.Add(listener);
                accessor[listener.Id] = listener;

                LSender.Send(this, $"listener has been added into container - {listener.Id}", Message.Level.Debug);
                return true;
            }
        }

        /// <summary>
        /// Removes specified listener from the collection.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Unregister(T listener)
        {
            lock(sync)
            {
                int idx = listeners.FindIndex(p => p.Id == listener.Id);
                if(idx == -1) {
                    return false;
                }

                listeners.RemoveAt(idx);
                accessor.TryRemove(listener.Id, out T v);

                LSender.Send(this, $"listener has been removed from container - {listener.Id}", Message.Level.Debug);
                return true;
            }
        }

        /// <summary>
        /// Reset all collection.
        /// </summary>
        public void Reset()
        {
            lock(sync)
            {
                listeners.Clear();
                accessor.Clear();
            }
        }

        /// <summary>
        /// Determines whether the collection contains an listener.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Contains(T listener)
        {
            return Exists(listener.Id);
        }

        /// <summary>
        /// Checks existence of listener by Guid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(Guid id)
        {
            lock(sync)
            {
#if DEBUG
                Debug.Assert(accessor.Count == listeners.Count);
#endif
                return accessor.ContainsKey(id);
                //return listeners.Any(l => ((IListener)l).Id == id);
            }
        }

        /// <summary>
        /// Get listener by specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>null if not found.</returns>
        public T GetById(Guid id)
        {
            lock(sync)
            {
                if(Exists(id)) {
                    return accessor[id];
                }
                return default(T);
                //return listeners.Where(l => ((IListener)l).Id == id).FirstOrDefault();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> result;
            lock(sync) {
                result = listeners.GetEnumerator();
            }
            return result;
        }

        public T this[Guid id]
        {
            get {
                return GetById(id);
            }
            set
            {
                lock(sync)
                {
                    T listener  = GetById(id);
                    listener    = value;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                lock(sync) {
                    return listeners[index];
                }
            }
            set
            {
                lock(sync)
                {
                    if(index < 0 || index >= listeners.Count) {
                        throw new ArgumentOutOfRangeException("index", index, $"Value must be in range: 0 - {listeners.Count - 1}");
                    }
                    listeners[index] = value;
                }
            }
        }
    }
}
