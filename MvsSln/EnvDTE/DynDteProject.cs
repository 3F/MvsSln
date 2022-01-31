/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.EnvDTE
{
    /// <summary>
    /// Helper for access to EnvDTE.Project without direct reference.
    /// </summary>
    [Obsolete("Scheduled for removal in future major releases: https://github.com/3F/MvsSln/issues/22")]
    public class DynDteProject
    {
        /// <summary>
        /// Environment with initialized xprojects.
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// EnvDTE.Project
        /// </summary>
        protected dynamic pdte;

        /// <summary>
        /// EnvDTE.Projects wrapped by DProject.
        /// https://msdn.microsoft.com/en-us/library/envdte.projects.aspx
        /// </summary>
        public IEnumerable<DProject> Projects
        {
            get
            {
                foreach(var p in pdte?.Collection) {
                    yield return new DProject(p);
                }
            }
        }

        /// <summary>
        /// Access to each IXProject and saving data via EnvDTE.
        /// </summary>
        /// <param name="metalib">Optional meta-library file name without extension to filter.</param>
        /// <param name="metalibKey">PublicKeyToken of meta-library if used.</param>
        public IEnumerable<IXProject> GetAndSaveXProjects(string metalib = null, string metalibKey = null)
        {
            foreach(var dtePrj in Projects)
            {
                if((metalib != null && metalibKey != null) 
                    && !dtePrj.HasReference(metalib, metalibKey))
                {
                    continue;
                }

                var xprojects = env.UniqueByGuidProjects?.Where(p =>
                    p.ProjectItem.project.fullPath.Equals(dtePrj.FullName, StringComparison.InvariantCultureIgnoreCase)
                );

                foreach(var xprj in xprojects) {
                    yield return xprj;
                }
                
                dtePrj.Save(dtePrj.FullName);
            }
        }

        /// <summary>
        /// To update property value for all available projects.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="metalib">Optional meta-library file name without extension to filter.</param>
        /// <param name="metalibKey">PublicKeyToken of meta-library if used.</param>
        public void UpdatePropertyForAllProjects(string name, string value, string metalib = null, string metalibKey = null)
        {
            CheckName(ref name);

            foreach(var prj in GetAndSaveXProjects(metalib, metalibKey))
            {
                if(value != null) {
                    prj.SetProperty(name, value);
                }
                else {
                    prj.RemoveProperty(name);
                }
            }
        }

        /// <param name="pdte"></param>
        /// <param name="env"></param>
        public DynDteProject(dynamic pdte, IEnvironment env)
        {
            this.pdte   = pdte;
            this.env    = env ?? throw new ArgumentNullException(nameof(env), MsgResource.ValueNoEmptyOrNull);
        }

        protected virtual void CheckName(ref string name)
        {
            if(String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name), MsgResource.ValueNoEmptyOrNull);
            }
        }
    }
}