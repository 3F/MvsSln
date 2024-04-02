/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Exceptions;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core
{
    public class SlnWriter: IDisposable
    {
        protected StreamWriter stream;

        private static readonly SemaphoreSlim semsync = new(initialCount: 1, maxCount: 1);

        /// <summary>
        /// Available writers to process sections.
        /// </summary>
        public IDictionary<Type, HandlerValue> Handlers { get; protected set; }

#if !NET40

        /// <inheritdoc cref="Write(IEnumerable{ISection})"/>
        public async Task WriteAsync(IEnumerable<ISection> sections)
        {
            sections = ValidateAndGetWritable(sections);
            await SyncAsync(async () => await sections.ForEach(WriteAsync));
        }

        /// <inheritdoc cref="Write(ISection)"/>
        public async Task WriteAsync(ISection section)
        {
            string raw = PrepareSection(section);
            if(raw != null) await WriteAsync(raw);
        }

        /// <inheritdoc cref="WriteAsString(IEnumerable{ISection})"/>
        public async Task<string> WriteAsStringAsync(IEnumerable<ISection> sections)
        {
            CheckStreamForString();

            return await SyncAsync(async () =>
            {
                await FlushAndReset().WriteNoLockAsync(sections);
                return await GetReaderForString().ReadToEndAsync();
            });
        }

#endif

        /// <summary>
        /// To write all not ignored sections with rules from handlers.
        /// </summary>
        /// <param name="sections"></param>
        public void Write(IEnumerable<ISection> sections)
        {
            sections = ValidateAndGetWritable(sections);
            Sync(() => sections.ForEach(Write));
        }

        /// <summary>
        /// To write a single section with rules from handlers.
        /// </summary>
        /// <param name="section"></param>
        public void Write(ISection section)
        {
            string raw = PrepareSection(section);
            if(raw != null) Write(raw);
        }

        /// <summary>
        /// To write all not ignored sections with rules from handlers into the input string.
        /// </summary>
        /// <param name="sections"></param>
        /// <returns>Processed sections as string data</returns>
        /// <exception cref="NotSupportedException"></exception>
        public string WriteAsString(IEnumerable<ISection> sections)
        {
            CheckStreamForString();

            return Sync(() =>
            {
                FlushAndReset().WriteNoLock(sections);
                return GetReaderForString().ReadToEnd();
            });
        }

        /// <inheritdoc cref="SlnWriter(string, IDictionary{Type, HandlerValue}, Encoding)"/>
        public SlnWriter(string sln, IDictionary<Type, HandlerValue> handlers)
            : this(sln, handlers, Encoding.UTF8)
        {

        }

        /// <param name="sln">Destination file.</param>
        /// <param name="handlers">Prepared write-handlers (see <see cref="IObjHandler"/>) for a specific types of readers (see <see cref="ISlnHandler"/>).</param>
        /// <param name="enc">Text encoding for result data.</param>
        public SlnWriter(string sln, IDictionary<Type, HandlerValue> handlers, Encoding enc)
            : this(new StreamWriter(sln, false, enc), handlers)
        {

        }

        /// <inheritdoc cref="SlnWriter(IDictionary{Type, HandlerValue}, Encoding)"/>
        public SlnWriter(IDictionary<Type, HandlerValue> handlers)
            : this(new StreamWriter(new MemoryStream()), handlers)
        {

        }

        /// <summary>
        /// Initialize using <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="handlers">Prepared write-handlers (see <see cref="IObjHandler"/>) for a specific types of readers (see <see cref="ISlnHandler"/>).</param>
        /// <param name="enc">Text encoding for result data.</param>
        public SlnWriter(IDictionary<Type, HandlerValue> handlers, Encoding enc)
            : this(new StreamWriter(new MemoryStream(), enc), handlers)
        {

        }

        /// <inheritdoc cref="SlnWriter(IDictionary{Type, HandlerValue}, Encoding)"/>
        public SlnWriter(StreamWriter writer, IDictionary<Type, HandlerValue> handlers)
        {
            stream      = writer ?? throw new ArgumentNullException(nameof(writer));
            Handlers    = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        protected virtual void CheckStreamForString()
        {
            if(stream?.BaseStream is not MemoryStream)
            {
                throw new NotSupportedException($"{stream.BaseStream.GetType()} is not {nameof(MemoryStream)}");
            }
        }

#if !NET40

        protected async Task WriteNoLockAsync(IEnumerable<ISection> sections)
        {
            await ValidateAndGetWritable(sections).ForEach(WriteAsync);
        }

#endif
        protected void WriteNoLock(IEnumerable<ISection> sections)
        {
            ValidateAndGetWritable(sections).ForEach(Write);
        }

        protected IEnumerable<ISection> ValidateAndGetWritable(IEnumerable<ISection> sections)
        {
            sections = sections.Select(s => s.Clone()).ToArray();
            Validate(sections);

            return WritableSections(sections);
        }

        protected string GetHandlerValueOrRaw(Type tid, ISection section)
            => Handlers.GetOrDefault(tid).handler?.Extract(Handlers[tid].value)
            ?? section.Raw.data;

        protected string PrepareSection(ISection section)
        {
            if(section == null) throw new ArgumentNullException(nameof(section));

            if(section.Ignore) return null;

            return section.Handler == null
                ? section.Raw.data
                : GetHandlerValueOrRaw(section.Handler.GetType(), section);
        }

        protected void Validate(IEnumerable<ISection> sections)
        {
            var coh = GetSlnHandlers(sections)
                        .Where(s => s.CoHandlers != null && s.CoHandlers.Count > 0)
                        .ToDictionary(key => key.GetType(), value => value.CoHandlers);

            foreach(var h in Handlers)
            {
                if(coh.ContainsKey(h.Key))
                {
                    if(coh[h.Key].Except(Handlers.Keys).Count() != coh[h.Key].Count)
                    {
                        throw new CoHandlerRuleException
                        (
                            $"{MsgR.OnlyParentHandlerAllowed} '{h.Key}' <- {string.Join(", ", coh[h.Key].Select(c => c.Name))}"
                        );
                    }
                    continue;
                }

                if(coh.Where(v => v.Value.Contains(h.Key))
                        .Select(v => v.Key)
                        .Except(Handlers.Keys).Any())
                {
                    throw new CoHandlerRuleException($"{MsgR.ParentHandlerInstead} of '{h.Key}'");
                }
            }
        }

        protected HashSet<ISlnHandler> GetSlnHandlers(IEnumerable<ISection> sections)
        {
            //var sh = sections.Where(s => s.Handler is ISlnHandler)
            //                 .GroupBy(s => s.Handler).Select(s => s.Key);

            var sh = new HashSet<ISlnHandler>();
            sections.Where(s => s.Handler is ISlnHandler)
                    .ForEach(s => sh.Add((ISlnHandler)s.Handler));

            return sh;
        }

        protected IEnumerable<ISection> WritableSections(IEnumerable<ISection> sections)
        {
            var ret     = new List<ISection>();
            var hTypes  = new HashSet<Type>();

            void _HashType(Type root, Type item)
            {
                if(Handlers.ContainsKey(root)) hTypes.Add(item);
            }

            foreach(ISection part in sections.Where(s => !s.Ignore))
            {
                if(part.Handler == null)
                {
                    ret.Add(part);
                    continue;
                }

                Type root = part.Handler.GetType();

                if(part.Handler is ISlnHandler sh)
                {
                    sh.CoHandlers?.ForEach(h => _HashType(root, h));
                }

                if(hTypes.Contains(root)) continue;
                _HashType(root, root);

                ret.Add(part);
            }

            return ret;
        }

#if !NET40

        protected async Task<T> SyncAsync<T>(Func<Task<T>> act)
        {
            await semsync.WaitAsync();
            try
            {
                return await act();
            }
            finally
            {
                semsync.Release();
            }
        }

        protected virtual async Task WriteAsync(string raw)
        {
            await stream.WriteLineAsync(raw);
        }

#endif

        protected virtual void Write(string raw)
        {
            stream.WriteLine(raw);
        }

        protected T Sync<T>(Func<T> act)
        {
            semsync.Wait();
            try
            {
                return act();
            }
            finally
            {
                semsync.Release();
            }
        }

        /// <returns>
        /// Returns new <see cref="StreamReader"/> but do NOT dispose it because this is from BaseStream 
        /// that will be disposed along with <see cref="SlnWriter"/>.
        /// </returns>
        private StreamReader GetReaderForString()
        {
            FlushAndReset();
            return new StreamReader(stream.BaseStream, stream.Encoding);
        }

        private SlnWriter FlushAndReset()
        {
            if(stream.BaseStream.Position > 0)
            {
                stream.Flush();
                stream.BaseStream.Position = 0;
            }
            return this;
        }

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _)
        {
            if(!disposed)
            {
                stream?.Dispose();

                disposed = true;
            }
        }

        #endregion
    }
}
