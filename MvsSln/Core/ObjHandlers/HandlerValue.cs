/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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

namespace net.r_eg.MvsSln.Core.ObjHandlers
{
    public struct HandlerValue
    {
        public IObjHandler handler;

        /// <summary>
        /// Unspecified value for handler.
        /// </summary>
        public object value;

        private Guid id;

        public static bool operator ==(HandlerValue a, HandlerValue b)
        {
            if(Object.ReferenceEquals(a, null)) {
                return Object.ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        public static bool operator !=(HandlerValue a, HandlerValue b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if(Object.ReferenceEquals(obj, null) || !(obj is HandlerValue)) {
                return false;
            }

            var x = (HandlerValue)obj;

            return handler == x.handler 
                    && value == x.value 
                    && id == x.id;
        }

        public override int GetHashCode()
        {
            int polynom(int r, int x)
            {
                unchecked {
                    return (r << 5) + r ^ x;
                }
            };

            int h = 0;
            h = polynom(h, handler.GetHashCode());
            h = polynom(h, value.GetHashCode());
            h = polynom(h, id.GetHashCode());

            return h;
        }

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
