/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

namespace net.r_eg.MvsSln.Core
{
    // TODO: move to '.Types' namespace
    public enum ProjectType
    {
        Unknown,

        /// <summary>
        /// Legacy Visual Basic.
        /// </summary>
        Vb,

        /// <summary>
        /// Legacy C#.
        /// </summary>
        Cs,

        Vj,
        Vc,

        /// <summary>
        /// Legacy F#.
        /// </summary>
        Fs,

        Db,
        Wd,
        Web,
        SlnFolder,
        Deploy,

        /// <summary>
        /// Service Fabric project.
        /// </summary>
        Sf,

        /// <summary>
        /// SDK based Visual Basic.
        /// </summary>
        VbSdk,

        /// <summary>
        /// SDK based F#.
        /// </summary>
        FsSdk,

        /// <summary>
        /// SDK based C#.
        /// </summary>
        CsSdk,
    }
}