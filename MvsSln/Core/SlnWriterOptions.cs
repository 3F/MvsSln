/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Core
{
    [Flags]
    public enum SlnWriterOptions
    {
        None = 0,

        CreateProjectsIfNotExist = 0x001,

        StrictTouch = 0x002,
    }
}