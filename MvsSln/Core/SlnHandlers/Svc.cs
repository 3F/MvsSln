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
using System.Collections.Generic;
using System.IO;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public sealed class Svc: ISvc
    {
        private long nline = 0;

        private object sync = new object();

        /// <summary>
        /// Used stream.
        /// </summary>
        public StreamReader Stream
        {
            get;
            set;
        }

        /// <summary>
        /// Prepared solution data.
        /// </summary>
        public ISlnResultSvc Sln
        {
            get;
            set;
        }

        /// <summary>
        /// Unspecified storage of the user scope.
        /// </summary>
        public Dictionary<Guid, object> UData
        {
            get;
            set;
        } = new Dictionary<Guid, object>();

        /// <summary>
        /// Reads a line of characters from stream.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            lock(sync)
            {
                ++nline;
                return Stream?.ReadLine();
            }
        }

        /// <summary>
        /// Reads a line of characters from stream with tracking.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string ReadLine(object handler)
        {
            string line = ReadLine();
            Track(line, handler);
            return line;
        }

        /// <summary>
        /// Resets stream and its related data.
        /// </summary>
        public void ResetStream()
        {
            if(Stream != null) {
                nline = Stream.BaseStream.Seek(0, SeekOrigin.Begin);
                return;
            }
            nline = 0;
        }

        /// <summary>
        /// Tracking for line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="handler">Specific handler if used, or null as an unspecified.</param>
        public ISection Track(RawText line, object handler = null)
        {
            if((Sln.ResultType & SlnItems.Map) != SlnItems.Map) {
                return null;
            }

            if(Sln.MapList == null) {
                Sln.MapList = new List<ISection>();
            }

            ISection section = new Section(handler, line, nline);
            Sln.MapList.Add(section);
            return section;
        }
    }
}
