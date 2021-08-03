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
