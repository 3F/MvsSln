/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.IO;
using System.Threading.Tasks;

namespace net.r_eg.MvsSln.Static
{
    public static class Members
    {
        /// <summary>
        /// Determines path format in current environment.
        /// </summary>
        /// <remarks>`/` in <see cref="Path.DirectorySeparatorChar"/> as path for Unix-like systems</remarks>
        public static bool IsUnixLikePath => Path.DirectorySeparatorChar == '/';

        /// <summary>
        /// 32-bit or 64-bit addressing in the current process?
        /// </summary>
        public static bool Is64bit => IntPtr.Size == sizeof(long);

#if NET40

        public static T[] EmptyArray<T>() => _EmptyArray<T>.value;

        internal static Task<T> GetTaskFromResult<T>(T input)
        {
            TaskCompletionSource<T> tcs = new();
            tcs.SetResult(input);
            return tcs.Task;
        }

        private static class _EmptyArray<T>
        {
            public static readonly T[] value = [];
        }

#else

        public static T[] EmptyArray<T>() => Array.Empty<T>();

#endif

    }
}