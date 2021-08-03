/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
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

namespace net.r_eg.MvsSln.EnvDTE
{
    /// <summary>
    /// Wrapper of dynamic EnvDTE.Project.
    /// </summary>
    [Obsolete("Scheduled for removal in future major releases: https://github.com/3F/MvsSln/issues/22")]
    public class DProject
    {
        /// <summary>
        /// Gets the full path and name of the EnvDTE.Project object's file.
        /// </summary>
        public string FullName
        {
            get => Raw?.FullName;
        }

        /// <summary>
        /// The references in the project.
        /// </summary>
        public dynamic References
        {
            get => Raw?.Object.References;
        }

        /// <summary>
        /// Dynamic access to EnvDTE.Project.
        /// </summary>
        public dynamic Raw
        {
            get;
            protected set;
        }

        /// <summary>
        /// To check existence of references by name and PublicKeyToken.
        /// https://msdn.microsoft.com/en-us/library/vslangproj.reference.aspx
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pubkey"></param>
        /// <returns></returns>
        public bool HasReference(string name, string pubkey = null)
        {
            foreach(var pRef in References) {
                if(pRef.Name == name && (pubkey == null || pRef.PublicKeyToken == pubkey)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Saves the project or project item.
        /// </summary>
        /// <param name="FileName">Optional name in which to save the project or project item.</param>
        public void Save(string FileName = "")
        {
            Raw?.Save(FileName);
        }

        /// <param name="pdte"></param>
        public DProject(dynamic pdte)
        {
            Raw = pdte;
        }
    }
}