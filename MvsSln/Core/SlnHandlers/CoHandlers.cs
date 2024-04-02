/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    internal sealed class CoHandlers
    {
#if FEATURE_COH_EXT

        public readonly HashSet<Type> set;

        public readonly IEnumerable<ISlnHandler> handlers;

        public readonly Dictionary<Guid, bool> has;
#else

        private readonly Dictionary<Guid, bool> has;
#endif

        public bool Contains(Guid id) => has.ContainsKey(id) && has[id];

        public CoHandlers(IEnumerable<ISlnHandler> slnHandlers)
        {
#if FEATURE_COH_EXT
            set = [];
#else
            IEnumerable<ISlnHandler> handlers;
#endif
            handlers = slnHandlers ?? throw new ArgumentNullException(nameof(slnHandlers));

            has = [];

            foreach(ISlnHandler h in handlers)
            {
                if(h.CoHandlers == null || h.CoHandlers.Count < 1) continue;

                IEnumerable<Type> registered = h.CoHandlers.Intersect
                (
                    handlers.Select(r => r.GetType())
                );

                has[h.Id] = registered.Any();

#if FEATURE_COH_EXT
                foreach(Type t in registered) set.Add(t);
#endif
            }
        }
    }
}
