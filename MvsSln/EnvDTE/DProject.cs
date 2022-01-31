/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
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