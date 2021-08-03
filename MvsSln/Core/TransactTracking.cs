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
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core
{
    public sealed class TransactTracking<T, TCore> 
        where TCore: class, ICollection<T>
    {
        private TCore core;
        private Queue<T> queries = new Queue<T>();

        public IEnumerable<T> Queue
        {
            get => queries;
        }

        public TransactTracking<T, TCore> Track(T section)
        {
            queries.Enqueue(section);
            return this;
        }

        public TransactTracking<T, TCore> Commit()
        {
            queries.ForEach(s => core.Add(s));
            Reset();
            return this;
        }

        public TransactTracking<T, TCore> Rollback()
        {
            Reset();
            return this;
        }

        public TransactTracking<T, TCore> Action(TransactAction type)
        {
            switch(type)
            {
                case TransactAction.Commit: {
                    return Commit();
                }
                case TransactAction.Rollback: {
                    return Rollback();
                }
                case TransactAction.None: {
                    return this;
                }
            }

            throw new NotSupportedException();
        }

        public TransactTracking(TCore core)
        {
            this.core = core ?? throw new ArgumentNullException();
        }

        private void Reset()
        {
            queries.Clear();
        }
    }
}
