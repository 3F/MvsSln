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
using Microsoft.Build.Framework;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    /// <summary>
    /// temp stub ...
    /// </summary>
    internal class Log: ILog
    {
        /// <summary>
        /// Flag of Diagnostic mode
        /// </summary>
        public bool IsDiagnostic
        {
            get {
                return (level == LoggerVerbosity.Diagnostic);
            }
        }

        /// <summary>
        /// Level for this instance.
        /// </summary>
        protected LoggerVerbosity level;

        public static void Trace(string message, params object[] args)
        {
            Msg(message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            Msg(message, args);
        }

        public static void Msg(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        /// <summary>
        /// Writes message for information level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void info(string message, params object[] args)
        {
            Msg(message, args);
        }

        /// <summary>
        /// Writes message for debug level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void debug(string message, params object[] args)
        {
            if(IsDiagnostic) {
                info(message, args);
            }
        }

        public Log(LoggerVerbosity level)
        {
            this.level = level;
        }
    }
}
