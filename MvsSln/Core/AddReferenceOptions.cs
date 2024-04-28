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
    public enum AddReferenceOptions
    {
        None,

        Default = HideEmbedInteropTypes | HideSpecificVersion,

        DefaultResolve = Default
                        | ResolveAssemblyName
                        | OmitArchitecture
                        | OmitCultureNeutral
                        | OmitPublicKeyTokenNull,

        Mini = Default | HidePrivate,

        MiniResolve = Mini | DefaultResolve | OmitCulture,

        MicroResolve = MiniResolve | OmitPublicKeyToken,

        NanoResolve = MiniResolve | OmitVersion,

        PicoResolve = NanoResolve | MicroResolve,

        DefaultAsm = Default | MakeRelativePath,

        /// <summary>
        /// Meta `Private` (n. Copy Local) = true.
        /// </summary>
        /// <remarks>If not, false. In addition, see <see cref="HidePrivate"/> to disable it.</remarks>
        Private = 1,

        /// <summary>
        /// Meta `EmbedInteropTypes` = true.
        /// </summary>
        /// <remarks>If not, false. In addition, see <see cref="HideEmbedInteropTypes"/> to disable it.</remarks>
        EmbedInteropTypes = 2,

        /// <summary>
        /// Meta 'SpecificVersion' = true.
        /// </summary>
        /// <remarks>If not, false. In addition, see <see cref="HideSpecificVersion"/> to disable it.</remarks>
        SpecificVersion = 4,

        /// <summary>
        /// Do not generate `Private` meta at all.
        /// </summary>
        HidePrivate = 8,

        /// <summary>
        /// Do not generate `EmbedInteropTypes` meta at all.
        /// </summary>
        HideEmbedInteropTypes = 0x10,

        /// <summary>
        /// Do not generate `SpecificVersion` meta at all.
        /// </summary>
        HideSpecificVersion = 0x20,

        /// <summary>
        /// Do not generate `HintPath` meta at all.
        /// </summary>
        HideHintPath = 0x40,

        /// <summary>
        /// Evaluate properties from input paths. 
        /// e.g. `metacor\$(namespace)\$(libname)` as `metacor\net.r_eg.DllExport\DllExport.dll`
        /// </summary>
        EvaluatePath = 0x80,

        /// <summary>
        /// Save `HintPath` meta with evaluated value.
        /// </summary>
        /// <remarks>Activates <see cref="EvaluatePath"/></remarks>
        EvaluatedHintPath = 0x100 | EvaluatePath,

        /// <summary>
        /// Resolve assembly name using full path to module.
        /// </summary>
        ResolveAssemblyName = 0x200,

        /// <summary>
        /// Do not specify `Version` in Include.
        /// </summary>
        /// <remarks>~ Version=1.5.1.35977</remarks>
        OmitVersion = 0x400,

        /// <summary>
        /// Do not specify `PublicKeyToken=null` in Include.
        /// </summary>
        OmitPublicKeyTokenNull = 0x800,

        /// <summary>
        /// Do not specify `PublicKeyToken` in Include.
        /// </summary>
        /// <remarks>~ PublicKeyToken=4bbd2ef743db151e</remarks>
        OmitPublicKeyToken = 0x1000,

        /// <summary>
        /// Do not specify `Culture=neutral` in Include.
        /// </summary>
        /// <remarks>Culture=neutral</remarks>
        OmitCultureNeutral = 0x2000,

        /// <summary>
        /// Do not specify `Culture` in Include.
        /// </summary>
        /// <remarks>~ Culture=...</remarks>
        OmitCulture = 0x4000 | OmitCultureNeutral,

        /// <summary>
        /// Do not specify `processorArchitecture` in Include.
        /// </summary>
        /// <remarks>~ processorArchitecture=...</remarks>
        OmitArchitecture = 0x8000,

        /// <summary>
        /// Make relative path for `HintPath` meta when it is possible.
        /// </summary>
        MakeRelativePath = 0x10_000,
    }
}
