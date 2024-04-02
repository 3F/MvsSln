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

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    using TransactSection = TransactTracking<ISection, IList<ISection>>;

    public sealed class Svc: ISvc
    {
        private readonly StreamReader stream;

        private readonly TransactSection tracking;

        private long nline = 0;

        private readonly object sync = new();

        public Encoding CurrentEncoding => stream?.CurrentEncoding;

        public ISlnResultSvc Sln { get; set; }

        public Dictionary<Guid, object> UData { get; set; } = [];

        public string ReadLine()
        {
            lock(sync)
            {
                ++nline;
                return stream?.ReadLine();
            }
        }

        public string ReadLine(object handler)
        {
            string line = ReadLine();
            Track(line, handler);
            return line;
        }

        public void ResetStream()
        {
            if(stream != null)
            {
                nline = stream.BaseStream.Seek(0, SeekOrigin.Begin);
                return;
            }
            nline = 0;
        }

        public ISection Track(RawText line, object handler = null)
        {
            if((Sln.ResultType & SlnItems.Map) != SlnItems.Map) {
                return null;
            }

            lock(sync)
            {
                tracking?.Commit(); // to commit all delayed sections

                ISection section = new Section(handler, line, nline);
                Sln.Map.Add(section);
                return section;
            }
        }

        public TransactSection TransactTrack(RawText line, object handler = null)
        {
            return TransactTrack(out ISection _, line, handler);
        }

        public TransactSection TransactTrack(out ISection section, RawText line, object handler = null)
        {
            section = null;

            if(tracking == null 
                || (Sln.ResultType & SlnItems.Map) != SlnItems.Map)
            {
                return null;
            }

            section = new Section(handler, line, nline);
            return tracking.Track(section);
        }

        /// <param name="reader"></param>
        /// <param name="rsln"></param>
        public Svc(StreamReader reader, ISlnResultSvc rsln)
            : this(reader)
        {
            Sln         = rsln ?? throw new ArgumentNullException(nameof(rsln));
            tracking    = new(Sln.Map);
        }

        /// <param name="reader"></param>
        public Svc(StreamReader reader)
        {
            stream = reader ?? throw new ArgumentNullException(nameof(reader));
        }
    }
}
