/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public interface ISlnHandler: IHandler
    {
        /// <summary>
        /// Completeness of implementation.
        /// Aggregates additional handlers that will process same line.
        /// </summary>
        ICollection<Type> CoHandlers { get; }

        /// <summary>
        /// Action with incoming line.
        /// </summary>
        LineAct LineControl { get; }

        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        bool IsActivated(ISvc svc);

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        bool Condition(RawText line);

        /// <summary>
        /// The logic before processing file.
        /// </summary>
        /// <param name="svc"></param>
        void PreProcessing(ISvc svc);

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        bool Positioned(ISvc svc, RawText line);

        /// <summary>
        /// The logic after processing file.
        /// </summary>
        /// <param name="svc"></param>
        void PostProcessing(ISvc svc);
    }
}
