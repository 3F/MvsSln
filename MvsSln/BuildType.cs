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
using System.Runtime.InteropServices;

namespace net.r_eg.MvsSln
{
    [Guid("6445BCC6-B722-43C9-A230-003D8B4FFED2")]
    public enum BuildType
    {
        /// <summary>
        /// Common context - any type or type by default
        /// </summary>
        Common = Int32.MaxValue,

        /// <summary>
        /// 'build' action
        /// </summary>
        Build = 100,

        /// <summary>
        /// 'rebuild' action
        /// </summary>
        Rebuild = 101,

        /// <summary>
        /// 'clean' action
        /// </summary>
        Clean = 102,

        /// <summary>
        /// 'build' action for selection
        /// </summary>
        BuildSelection = 200,

        /// <summary>
        /// 'rebuild' action for selection
        /// </summary>
        RebuildSelection = 201,

        /// <summary>
        /// 'clean' action for selection
        /// </summary>
        CleanSelection = 202,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildOnlyProject = 205,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildOnlyProject = 206,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanOnlyProject = 207,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildCtx = 302,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildCtx = 303,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanCtx = 304,
    }
}
