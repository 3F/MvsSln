/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Extensions
{
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
            int h = r;
            foreach(var v in values) {
                h = h.HashPolynom(v?.GetHashCode() ?? 0);
            }
            return h;
        }
    }
}