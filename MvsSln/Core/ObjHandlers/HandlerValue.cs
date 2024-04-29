/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public readonly struct HandlerValue
    {
        public readonly IObjHandler handler;

        /// <summary>
        /// Unspecified value for handler.
        /// </summary>
        public readonly object value;

        private readonly Guid id;

        public static bool operator ==(HandlerValue a, HandlerValue b) => a.Equals(b);

        public static bool operator !=(HandlerValue a, HandlerValue b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || obj is not HandlerValue x) return false;

            return handler == x.handler 
                    && value == x.value 
                    && id == x.id;
        }

        public override int GetHashCode() => 0.CalculateHashCode
        (
            handler,
            value,
            id
        );

        public HandlerValue(IObjHandler handler)
            : this(handler, null)
        {

        }

        public HandlerValue(IObjHandler handler, object value)
        {
            this.handler    = handler;
            this.value      = value;

            id = Guid.NewGuid();
        }
    }
}
