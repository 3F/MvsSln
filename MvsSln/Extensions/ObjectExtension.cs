/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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

namespace net.r_eg.MvsSln.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Execute action separately from result.
        /// </summary>
        /// <typeparam name="T">The type of value that should be returned.</typeparam>
        /// <param name="obj">Unspecified object.</param>
        /// <param name="act">Any action that should be executed.</param>
        /// <returns>Same value from selected object as T type.</returns>
        public static T E<T>(this object obj, Action act)
        {
            act();
            return (T)obj;
        }

        /// <summary>
        /// Execute action separately from result.
        /// Alias to `E&lt;object&gt;()`
        /// </summary>
        /// <param name="obj">Unspecified object.</param>
        /// <param name="act">Any action that should be executed.</param>
        /// <returns>Same value from selected object.</returns>
        public static object E(this object obj, Action act)
        {
            return E<object>(obj, act);
        }
    }
}