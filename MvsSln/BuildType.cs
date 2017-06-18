/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Represents available context for actions.
    /// </summary>
    [Guid("70EC2620-D34E-407A-AB2B-6592D4974510")]
    public enum BuildType
    {
        /// <summary>
        /// Common context - any type or type by default
        /// </summary>
        Common = Int32.MaxValue,

        /// <summary>
        /// Unspecified type as Before action.
        /// </summary>
        Before = Common - 10,

        /// <summary>
        /// Unspecified type as After action.
        /// </summary>
        After = Common - 11,

        /// <summary>
        /// Unspecified type as Before and/then After action, 
        /// i.e. double action.
        /// </summary>
        BeforeAndAfter = Common - 12,

        /// <summary>
        /// Reserved type 1
        /// </summary>
        Reserved1 = Common - 13,

        /// <summary>
        /// Reserved type 2
        /// </summary>
        Reserved2 = Common - 14,

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
        /// 'deploy' action
        /// </summary>
        Deploy = 103,

        /// <summary>
        /// 'Start Debugging' action
        /// </summary>
        Start = 104,

        /// <summary>
        /// 'Start Without Debugging' action
        /// </summary>
        StartNoDebug = 105,

        /// <summary>
        /// 'Publish' action
        /// </summary>
        Publish = 106,

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
        /// 'deploy' action for selection
        /// </summary>
        DeploySelection = 203,

        /// <summary>
        /// 'Publish' action for selection
        /// </summary>
        PublishSelection = 204,

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
        /// 'Compile' action
        /// </summary>
        Compile = 300,

        /// <summary>
        /// 'Link only' action
        /// </summary>
        LinkOnly = 301,

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

        /// <summary>
        /// 'deploy' action for project
        /// </summary>
        DeployCtx = 305,

        /// <summary>
        /// 'Publish' action for project
        /// </summary>
        PublishCtx = 306,
    }
}
