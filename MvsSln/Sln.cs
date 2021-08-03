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
using System.IO;
using System.Text;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln
{
    /// <summary>
    /// Wrapper of the default solution parser.
    /// </summary>
    public sealed class Sln: IDisposable
    {
        private readonly ISlnContainer parser = new SlnParser();

        /// <summary>
        /// Parsed solution data.
        /// </summary>
        public ISlnResult Result
        {
            get;
            private set;
        }

        /// <param name="file">Solution file</param>
        /// <param name="type">Allowed type of operations.</param>
        public Sln(string file, SlnItems type)
        {
            Result = parser.Parse(file, type);
        }

        /// <param name="reader"></param>
        /// <param name="type">Allowed type of operations.</param>
        public Sln(StreamReader reader, SlnItems type)
        {
            Result = parser.Parse(reader, type);
        }

        /// <param name="type">Allowed type of operations.</param>
        /// <param name="raw">Raw data inside string.</param>
        /// <param name="Enc">Encoding of raw data.</param>
        public Sln(SlnItems type, string raw, Encoding Enc)
            : this(type, new RawText() { data = raw, encoding = Enc }, null)
        {

        }

        /// <param name="type">Allowed type of operations.</param>
        /// <param name="raw">Raw data inside string.</param>
        public Sln(SlnItems type, string raw)
            : this(type, raw, Encoding.UTF8)
        {

        }

        /// <param name="type">Allowed type of operations.</param>
        /// <param name="raw">Solution raw data.</param>
        /// <param name="projects">Dictionary of raw xml projects by Guid.</param>
        public Sln(SlnItems type, RawText raw, IDictionary<string, RawText> projects)
        {
            parser.RawXmlProjects = projects;
            using(var reader = new StreamReader(raw.data.GetStream(raw.encoding), raw.encoding)) {
                Result = parser.Parse(reader, type);
            }
        }

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            if(!disposed)
            {
                Result?.Env?.Dispose();

                disposed = true;
            }
        }

        #endregion
    }
}
