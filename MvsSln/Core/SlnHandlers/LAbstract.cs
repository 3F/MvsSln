/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public abstract class LAbstract: ISlnHandler
    {
        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public abstract bool IsActivated(ISvc svc);

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public abstract bool Condition(RawText line);

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public abstract bool Positioned(ISvc svc, RawText line);

        /// <summary>
        /// Completeness of implementation.
        /// Aggregates additional handlers that will process same line.
        /// </summary>
        public virtual ICollection<Type> CoHandlers
        {
            get;
            protected set;
        }

        /// <summary>
        /// Action with incoming line.
        /// </summary>
        public virtual LineAct LineControl
        {
            get;
            protected set;
        } = LineAct.Process;

        /// <summary>
        /// Gets unique id of listener.
        /// </summary>
        public Guid Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// The logic before processing file.
        /// </summary>
        /// <param name="svc"></param>
        public virtual void PreProcessing(ISvc svc)
        {

        }

        /// <summary>
        /// The logic after processing file.
        /// </summary>
        /// <param name="svc"></param>
        public virtual void PostProcessing(ISvc svc)
        {

        }

        public LAbstract()
        {
            Id = GetType().GUID;
        }

        /// <param name="line">Initialize data from raw line.</param>
        /// <param name="solutionDir">Path to solution directory.</param>
        /// <returns></returns>
        protected ProjectItem GetProjectItem(string line, string solutionDir)
        {
            var pItem = new ProjectItem(line, solutionDir);

            if(pItem.pGuid == null) {
                LSender.Send(this, $"The Guid is null or empty for line :: '{line}'", Message.Level.Error);
                return default(ProjectItem);
            }

            if(String.Equals(Guids.SLN_FOLDER, pItem.pType, StringComparison.OrdinalIgnoreCase)) {
                LSender.Send(this, $"{pItem.name} has been ignored as solution-folder :: '{line}'", Message.Level.Debug);
                return default(ProjectItem);
            }

            return pItem;
        }
    }
}
