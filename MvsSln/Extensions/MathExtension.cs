/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.r_eg.MvsSln.Extensions
{
    using static net.r_eg.MvsSln.Static.Members;

    public static class MathExtension
    {
        /// <summary>
        /// Our optimal polynom for hash functions.
        /// </summary>
        /// <param name="r">initial vector</param>
        /// <param name="x">new value</param>
        /// <returns></returns>
        public static int HashPolynom(this int r, int x) => unchecked((r << 5) + r ^ x);

        /// <summary>
        /// Calculate final Hash Code from specified vector and pushed values.
        /// </summary>
        /// <param name="r">initial vector</param>
        /// <param name="values">List of individual Hash Code values.</param>
        /// <returns></returns>
        public static int CalculateHashCode(this int r, params object[] values)
        {
            return r.CalculateHashCode(values?.AsEnumerable() ?? EmptyArray<object>());
        }

        /// <inheritdoc cref="CalculateHashCode(int, object[])"/>
        public static int CalculateHashCode<T>(this int r, IEnumerable<T> values)
        {
            int h = r;
            foreach(T v in values)
            {
                h = h.HashPolynom(v?.GetHashCode() ?? 0);
            }
            return h;
        }

        internal static string ToHexString(this byte[] data, bool uppercase = false)
        {
            if(data == null || data.Length < 1) return string.Empty;
            StringBuilder sb = new(data.Length);

            foreach(byte b in data) sb.Append(b.ToString(uppercase ? "X" : "x"));
            return sb.ToString();
        }
    }
}