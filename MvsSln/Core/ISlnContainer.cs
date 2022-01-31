/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;
using System.IO;
using net.r_eg.MvsSln.Core.SlnHandlers;

namespace net.r_eg.MvsSln.Core
{
    public interface ISlnContainer
    {
        /// <summary>
        /// Available solution handlers.
        /// </summary>
        SynchSubscribers<ISlnHandler> SlnHandlers { get; }

        /// <summary>
        /// Dictionary of raw xml projects by Guid.
        /// Will be used if projects cannot be accessed from filesystem.
        /// </summary>
        IDictionary<string, RawText> RawXmlProjects { get; set; }

        /// <summary>
        /// To reset and register all default handlers.
        /// </summary>
        void SetDefaultHandlers();

        /// <summary>
        /// Parse of selected .sln file.
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        ISlnResult Parse(string sln, SlnItems type);

        /// <summary>
        /// To parse data from used stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        ISlnResult Parse(StreamReader reader, SlnItems type);
    }
}