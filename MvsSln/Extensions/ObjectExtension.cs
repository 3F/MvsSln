/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Execute action on value in the chain separately from result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="act"></param>
        /// <returns>Input value.</returns>
        public static T E<T>(this T obj, Action act) => E(obj, _=> act());

        /// <summary>
        /// Execute action on value in the chain separately from result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="act"></param>
        /// <returns>Input value.</returns>
        public static T E<T>(this T obj, Action<T> act)
        {
            act?.Invoke(obj);
            return obj;
        }
    }
}