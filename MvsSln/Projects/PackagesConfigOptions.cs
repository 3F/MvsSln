/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Projects
{
    [Flags]
    public enum PackagesConfigOptions
    {
        None,

        /// <summary>
        /// Use default behavior.
        /// </summary>
        Default = LoadOrNew | SilentLoading,

        /// <summary>
        /// Load existing storage. Will throw related exceptions for any failure.
        /// </summary>
        Load = 0x01,

        /// <summary>
        /// Load existing storage or create a new one for any failure.
        /// </summary>
        LoadOrNew = 0x02,

        /// <summary>
        /// Hide some errors when trying to parse the data at the loading stage, such as an empty file, etc.
        /// </summary>
        /// <remarks>Use <see cref="PackagesConfig.FailedLoading"/> for any related issues.</remarks>
        SilentLoading = 0x04,

        /// <summary>
        /// Treat the directory path as the path to storage data.
        /// </summary>
        PathToStorage = 0x08,
    }
}