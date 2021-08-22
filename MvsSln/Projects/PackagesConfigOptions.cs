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

namespace net.r_eg.MvsSln.Projects
{
    [Flags]
    public enum PackagesConfigOptions
    {
        None,

        /// <summary>
        /// Use default behavior.
        /// </summary>
        Default = LoadOrNew | SilentLoading,

        /// <summary>
        /// Load existing storage. Will throw related exceptions for any failure.
        /// </summary>
        Load = 0x01,

        /// <summary>
        /// Load existing storage or create a new one for any failure.
        /// </summary>
        LoadOrNew = 0x02,

        /// <summary>
        /// Hide some errors when trying to parse the data at the loading stage, such as an empty file, etc.
        /// </summary>
        /// <remarks>Use <see cref="PackagesConfig.FailedLoading"/> for any related issues.</remarks>
        SilentLoading = 0x04,

        /// <summary>
        /// Treat the directory path as the path to storage data.
        /// </summary>
        PathToStorage = 0x08,
    }
}