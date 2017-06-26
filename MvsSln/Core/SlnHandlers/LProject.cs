/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core.SlnHandlers
{
    public class LProject: LAbstract, ISlnHandler
    {
        /// <summary>
        /// New position in stream.
        /// </summary>
        /// <param name="stream">Used stream.</param>
        /// <param name="line">Received line.</param>
        /// <param name="rsln">Handled solution data.</param>
        public override void Positioned(StreamReader stream, string line, SlnResult rsln)
        {
            if((rsln.type & SlnItems.Projects) == 0) {
                return;
            }

            if(!line.StartsWith("Project(", StringComparison.Ordinal)) {
                return;
            }

            if(rsln.projectItems == null) {
                rsln.projectItems = new List<ProjectItem>();
            }

            var pItem = new ProjectItem(line, rsln.solutionDir);
            if(pItem.pGuid == null) {
                LSender.Send(this, $"LProject: The Guid is null empty for line :: '{line}'", Message.Level.Error);
                return;
            }

            if(String.Equals(Guids.SLN_FOLDER, pItem.pType, StringComparison.OrdinalIgnoreCase)) {
                LSender.Send(this, $"{pItem.name} has been ignored as solution-folder :: '{line}'", Message.Level.Debug);
                return;
            }

            rsln.projectItems.Add(pItem);
        }
    }
}
