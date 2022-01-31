/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LProjectSolutionItems: LAbstract, ISlnHandler
    {
        /// <summary>
        /// Checks the readiness to process data.
        /// </summary>
        /// <param name="svc"></param>
        /// <returns>True value if it's ready at current time.</returns>
        public override bool IsActivated(ISvc svc)
        {
            return ((svc.Sln.ResultType & SlnItems.SolutionItems) == SlnItems.SolutionItems);
        }

        /// <summary>
        /// Condition for line to continue processing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true value to continue.</returns>
        public override bool Condition(RawText line)
        {
            return line.trimmed.StartsWith("Project(", StringComparison.Ordinal);
        }

        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="line">Received line.</param>
        /// <returns>true if it was processed by current handler, otherwise it means ignoring.</returns>
        public override bool Positioned(ISvc svc, RawText line)
        {
            var pItem = new ProjectItem(line.trimmed, svc.Sln.SolutionDir);

            if(pItem.pGuid == null 
                || !String.Equals(Guids.SLN_FOLDER, pItem.pType, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            LSender.Send(this, $"Found solution-folder: '{pItem.name}'", Message.Level.Info);

            var folderItems = new List<RawText>();
            while((line = svc.ReadLine(this)) != null && (line != "EndProject"))
            {
                if(!line.trimmed.StartsWith("ProjectSection(SolutionItems) = preProject", StringComparison.Ordinal)) {
                    continue;
                }

                for(line = svc.ReadLine(this); line != null; line = svc.ReadLine(this))
                {
                    if(line.trimmed.StartsWith("EndProjectSection", StringComparison.Ordinal)) {
                        break;
                    }

                    var item = line.trimmed.Before('=');
                    if(item == null)
                    {
                        LSender.Send(
                            this, 
                            $"Ignored incorrect item for '{pItem.name}':{pItem.pGuid} - '{line}'", 
                            Message.Level.Warn
                        );
                        continue;
                    }

                    item = item.TrimEnd();
                    LSender.Send(this, $"Found item: '{item}'", Message.Level.Info);

                    folderItems.Add(new RawText(item, svc.CurrentEncoding));
                }
            }

            if(svc.Sln.SolutionFolderList == null) {
                svc.Sln.SolutionFolderList = new List<SolutionFolder>();
            }

            svc.Sln.SolutionFolderList.Add(new SolutionFolder(pItem, folderItems));
            return true;
        }
    }
}
