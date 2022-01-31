/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    /// <summary>
    /// TODO: review
    /// </summary>
    internal struct CoHandlers
    {
        public HashSet<Type> set;

        public Dictionary<Guid, bool> has;

        public IEnumerable<ISlnHandler> handlers;

        public bool Contains(Guid id)
        {
            return has.ContainsKey(id) && has[id];
        }

        /// <param name="slnHandlers"></param>
        public CoHandlers(IEnumerable<ISlnHandler> slnHandlers)
        {
            handlers = slnHandlers ?? throw new ArgumentNullException();

            set = new HashSet<Type>();
            has = new Dictionary<Guid, bool>();

            foreach(ISlnHandler h in handlers)
            {
                if(h.CoHandlers == null || h.CoHandlers.Count < 1) {
                    continue;
                }

                var registered = h.CoHandlers.Intersect(
                    handlers.Select(r => r.GetType())
                );

                has[h.Id] = registered.Count() > 0;

                var _this = this;
                registered.ForEach(t => _this.set.Add(t));
            }
        }
    }
}
