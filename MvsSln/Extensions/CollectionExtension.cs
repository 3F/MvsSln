/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.r_eg.MvsSln.Core.ObjHandlers;

namespace net.r_eg.MvsSln.Extensions
{
    public static class CollectionExtension
    {
#if !NET40

        /// <inheritdoc cref="ForEach{T}(IEnumerable{T}, Func{T, long, Task}) "/>
        public static async Task<IEnumerable<T>> ForEach<T>(this IEnumerable<T> items, Func<T, Task> act)
        {
            return await (items?.ForEach((x, i) => act(x)) ?? Task.FromResult(items));
        }

        /// <summary>
        /// Awaitable Foreach in Linq manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Input elements for iteration through it.</param>
        /// <param name="act">Async action that should be executed for each item.</param>
        /// <returns>
        /// Returns the original input value to continue the chain.
        /// <br/>
        /// Null is possible because this method can also extend T[] arrays.
        /// </returns>
        public static async Task<IEnumerable<T>> ForEach<T>(this IEnumerable<T> items, Func<T, long, Task> act)
        {
            if(items == null) return await Task.FromResult(items);

            long n = 0;
            foreach(T item in items)
            {
                await act(item, n++);
            }
            return items;
        }

#endif

        /// <inheritdoc cref="ForEach{T}(IEnumerable{T}, Action{T, long})"/>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> act)
        {
            return items?.ForEach((x, i) => act(x));
        }

        /// <summary>
        /// Foreach in Linq manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Input elements for iteration through it</param>
        /// <param name="act">The action that should be executed for each item.</param>
        /// <returns>
        /// Returns the original input value to continue the chain.
        /// <br/>
        /// Null is possible because this method can also extend T[] arrays. 
        /// </returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T, long> act)
        {
            if(items == null) return items;

            long n = 0;
            foreach(T item in items)
            {
                act(item, n++);
            }
            return items;
        }

        public static bool EqualsE<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            using IEnumerator<T> ea = a.GetEnumerator();
            using IEnumerator<T> eb = b.GetEnumerator();
            while(ea.MoveNext())
            {
                if(!eb.MoveNext() || !eb.Current.Equals(ea.Current)) return false;
            }
            return true;
        }

        /// <summary>
        /// Adds/Updates data in source via data from `items`.
        /// 
        /// Any duplicates will be just updated:
        /// ie. similar to `Concat()` except internal restriction for `Insert()`.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns>Updated source.</returns>
        public static IDictionary<string, string> AddOrUpdate(this IDictionary<string, string> source, IEnumerable<KeyValuePair<string, string>> items)
        {
            if(source == null || items == null) {
                return source;
            }

            foreach(var i in items) {
                source[i.Key] = i.Value;
            }
            return source;
        }

        /// <summary>
        /// Returns either value from dictionary or configured default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="def">Use this if key is not found.</param>
        /// <returns></returns>
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> data, TKey key, TVal def = default)
        {
            if(data == null) {
                return def;
            }
            return data.ContainsKey(key) ? data[key] : def;
        }

        /// <summary>
        /// Removes element from list by using specific comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="elem"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool Remove<T>(this IList<T> src, T elem, Func<T, T, bool> comparer)
        {
            for(int i = 0; i < src.Count; ++i)
            {
                if(comparer(src[i], elem)) {
                    src.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public static IDictionary<Type, HandlerValue> UpdateNewLine(this IDictionary<Type, HandlerValue> handlers, string newline)
        {
            handlers?.ForEach(h =>
            {
                if(h.Value.handler != null) h.Value.handler.NewLine = newline;
            });
            return handlers;
        }
    }
}