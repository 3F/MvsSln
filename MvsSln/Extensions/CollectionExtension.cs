﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 * Copyright (c) MvsSln contributors: https://github.com/3F/MvsSln/graphs/contributors
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
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Foreach in Linq manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="act">The action that should be executed for each item.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> act)
        {
            if(items == null) {
                return;
            }

            foreach(var item in items) {
                act(item);
            }
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
        public static IDictionary<string, string> AddOrUpdate(this IDictionary<string, string> source, IDictionary<string, string> items)
        {
            if(items == null) {
                return source;
            }

            foreach(var i in items) {
                source[i.Key] = i.Value;
            }
            return source;
        }
    }
}