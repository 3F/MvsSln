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
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Projects
{
    public interface IPackageInfo
    {
        /// <summary>
        /// Package id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Package version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// One-time parsed <see cref="Version"/> on the first access.
        /// </summary>
        Version VersionParsed { get; }

        /// <summary>
        /// Some related package meta information.
        /// Eg.:
        /// * targetFramework="net472"
        /// * output="vsSolutionBuildEvent"
        /// ...
        /// </summary>
        IDictionary<string, string> Meta { get; }

        /// <summary>
        /// Try get "targetFramework" value from <see cref="Meta"/> information.
        /// It must return <see langword="null"/> if attribute is not defined.
        /// </summary>
        /// <remarks>Alias to <see cref="Meta"/> accessing.</remarks>
        string MetaTFM { get; }

        /// <summary>
        /// Try get "output" value from <see cref="Meta"/> information.
        /// It must return <see langword="null"/> if attribute is not defined.
        /// </summary>
        /// <remarks>Alias to <see cref="Meta"/> accessing.</remarks>
        string MetaOutput { get; }

        /// <summary>
        /// Remove current package from storage.
        /// </summary>
        IPackagesConfig Remove();
    }
}