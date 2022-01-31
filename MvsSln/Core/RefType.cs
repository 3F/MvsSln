/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Diagnostics;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{Value}")]
    public sealed class RefType<T>
    {
        public T Value
        {
            get;
            set;
        }

        public static implicit operator T(RefType<T> v) => v == null ? default : v.Value;

        public static implicit operator RefType<T>(T v) => new RefType<T>(v);

        public static bool operator ==(RefType<T> a, RefType<T> b)
        {
            bool _EqNull(RefType<T> x)
            {
                return x is null || ReferenceEquals(x.Value, null);
            }
            return _EqNull(a) ? _EqNull(b) : a.Equals(b);
        }

        public static bool operator !=(RefType<T> a, RefType<T> b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is RefType<T>)) {
                return false;
            }
            return Value.Equals(((RefType<T>)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public RefType(T value)
        {
            Value = value;
        }

        public RefType()
        {

        }
    }
}