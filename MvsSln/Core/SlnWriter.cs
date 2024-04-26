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
    using static net.r_eg.MvsSln.Static.Members;

    public class SlnWriter: IDisposable
    {
        private static readonly SemaphoreSlim semsync = new(initialCount: 1, maxCount: 1);

        protected readonly StreamWriter stream;

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
                await FlushAndResetAsync();
                await WriteNoLockAsync(sections);

                await FlushAndResetAsync();
                return await GetReaderForString().ReadToEndAsync();
            });
        }

#else

        /// <inheritdoc cref="Write(IEnumerable{ISection})"/>
        /// <remarks>netfx4.0 legacy TAP implementation.</remarks>
        public Task WriteAsync(IEnumerable<ISection> sections)
        {
            sections = ValidateAndGetWritable(sections);

            semsync.Wait();
            try
            {
                return WriteValidatedNoLockAsync(sections);
            }
            finally
            {
                semsync.Release();
            }
        }

        /// <inheritdoc cref="Write(ISection)"/>
        /// <remarks>netfx4.0 legacy TAP implementation.</remarks>
        public Task WriteAsync(ISection section)
        {
            string raw = PrepareSection(section);
            return (raw != null) ? WriteAsync(raw) : GetTaskFromResult(false);
        }

        /// <inheritdoc cref="WriteAsString(IEnumerable{ISection})"/>
        /// <remarks>netfx4.0 legacy TAP implementation.</remarks>
        public Task<string> WriteAsStringAsync(IEnumerable<ISection> sections)
        {
            CheckStreamForString();

            semsync.Wait();
            try
            {
                return FlushAndReset()
                        .WriteNoLockAsync(sections)
                        .ContinueWith(t => FlushAndReset().ReadToEndAsync(stream.BaseStream))
                        .ContinueWith(t => t.Result.Result);
            }
            finally
            {
                semsync.Release();
            }
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
        /// To write all not ignored sections with rules from handlers into the string.
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

                return FlushAndReset()
                        .GetReaderForString().ReadToEnd();
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

            IObjHandler first = handlers.Values.FirstOrDefault().handler;
            if(first != null)
            {
                stream.NewLine = first.NewLine;
            }
        }

        /// <summary>
        /// Custom stream implementation using <see cref="SlnWriter"/> logic.
        /// </summary>
        protected SlnWriter()
        {

        }

        protected virtual void CheckStreamForString()
        {
            if(stream?.BaseStream is not MemoryStream)
            {
                throw new NotSupportedException($"{stream?.BaseStream.GetType()} is not {nameof(MemoryStream)}");
            }
        }

        /// <returns>
        /// Returns new <see cref="StreamReader"/> but do NOT dispose it because this is from BaseStream 
        /// that will be disposed along with <see cref="SlnWriter"/>.
        /// </returns>
        protected virtual StreamReader GetReaderForString()
            => new(stream.BaseStream, stream.Encoding);

#if !NET40

        protected async Task WriteNoLockAsync(IEnumerable<ISection> sections)
        {
            await ValidateAndGetWritable(sections).ForEach(WriteAsync);
        }

#else

        protected Task WriteNoLockAsync(IEnumerable<ISection> sections)
        {
            return WriteValidatedNoLockAsync(ValidateAndGetWritable(sections));
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
            List<ISection> ret      = [];
            HashSet<Type> hTypes    = [];

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

                if(!Handlers.ContainsKey(root) || Handlers[root].handler != null)
                {
                    ret.Add(part);
                }
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
            if(stream == null) throw new NotSupportedException(MsgR._0_IsEmptyOrNull.Format(nameof(stream)));
            await stream.WriteLineAsync(raw);
        }

#else

        protected Task<T> SyncAsync<T>(Func<Task<T>> act)
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

        protected virtual Task WriteAsync(string raw)
        {
            if(stream == null) throw new NotSupportedException(MsgR._0_IsEmptyOrNull.Format(nameof(stream)));

            byte[] data = stream.Encoding.GetBytes(raw + stream.NewLine);
            return Task.Factory.FromAsync
            (
                (callback, state) => stream.BaseStream.BeginWrite(data, 0, data.Length, callback, state),
                stream.BaseStream.EndWrite,
                state: null
            );
        }

        protected virtual Task<string> ReadToEndAsync(Stream stream)
        {
            if(stream == null) throw new NotSupportedException(MsgR._0_IsEmptyOrNull.Format(nameof(stream)));
            if(stream.Length > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(stream), stream.Length, string.Empty);

            if(stream.Length < 1) return GetTaskFromResult<string>(null);
            byte[] data = new byte[stream.Length];

            return Task.Factory.FromAsync
            (
                (callback, state) => stream.BeginRead(data, 0, (int)stream.Length, callback, state),
                stream.EndRead,
                state: null
            )
            .ContinueWith(t => this.stream.Encoding.GetString(data));
        }

#endif

        protected virtual void Write(string raw)
        {
            if(stream == null) throw new NotSupportedException(MsgR._0_IsEmptyOrNull.Format(nameof(stream)));
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

#if NET40

        private Task WriteValidatedNoLockAsync(IEnumerable<ISection> sections)
        {
            Task last = null;
            foreach(ISection s in sections) last = WriteAsync(s);

            return last ?? GetTaskFromResult(false);
        }

#else

        private async Task<SlnWriter> FlushAndResetAsync()
        {
            await stream?.FlushAsync();
            return Reset();
        }

#endif

        private SlnWriter FlushAndReset()
        {
            stream?.Flush();
            return Reset();
        }

        private SlnWriter Reset()
        {
            if(stream?.BaseStream.Position > 0)
            {
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
