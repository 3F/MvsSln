/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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
