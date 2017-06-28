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
using System.Diagnostics;
using Microsoft.Build.Evaluation;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{ProjectName}: [{ProjectItem.projectConfig}] {ProjectGuid}")]
    public class XProject
    {
        public Project Project
        {
            get;
            protected set;
        }

        public ProjectItemCfg ProjectItem
        {
            get;
            protected set;
        }

        public string ProjectGuid
        {
            get => GetProjectGuid(Project);
        }

        public string ProjectName
        {
            get => GetProjectName(Project);
        }

        public XProject(ProjectItemCfg pItem, Project prj)
        {
            ProjectItem = pItem;
            Project     = prj ?? throw new ArgumentNullException(nameof(prj), "Value cannot be null.");
        }

        protected virtual string GetProjectGuid(Project eProject)
        {
            return eProject.GetPropertyValue("ProjectGuid");
        }

        protected virtual string GetProjectName(Project eProject)
        {
            return eProject.GetPropertyValue("ProjectName");
        }
    }
}