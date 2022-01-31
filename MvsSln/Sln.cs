/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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
