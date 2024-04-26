/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;
using net.r_eg.MvsSln.Core.SlnHandlers;

namespace net.r_eg.MvsSln
{
    public static class DefaultHandlers
    {
        public static Dictionary<Type, HandlerValue> MakeFrom(ISlnResult sr)
        {
            if(sr == null) throw new ArgumentNullException(nameof(sr));
            Dictionary<Type, HandlerValue> ret = [];

            if(sr.Header != null)
            {
                ret[typeof(LVisualStudioVersion)] = new(new WVisualStudioVersion(sr.Header));
            }

            if(sr.ProjectItems != null)
            {
                ret[typeof(LProject)] = new(new WProject(sr.ProjectItems, sr.ProjectDependencies));
            }

            //[typeof(LProjectDependencies)] = // part of LProject, see CoHandlers

            if(sr.SolutionFolders != null)
            {
                ret[typeof(LProjectSolutionItems)] = new(new WProjectSolutionItems(sr.SolutionFolders));
            }

            if(sr.SolutionConfigs != null)
            {
                ret[typeof(LSolutionConfigurationPlatforms)] = new(new WSolutionConfigurationPlatforms(sr.SolutionConfigs));
            }

            if(sr.ProjectConfigs != null)
            {
                ret[typeof(LProjectConfigurationPlatforms)] = new(new WProjectConfigurationPlatforms(sr.ProjectConfigs));
            }

            if(sr.SolutionFolders != null || sr.ProjectItems != null)
            {
                ret[typeof(LNestedProjects)] = new(new WNestedProjects(sr.SolutionFolders, sr.ProjectItems));
            }

            if(sr.ExtItems != null)
            {
                ret[typeof(LExtensibilityGlobals)] = new(new WExtensibilityGlobals(sr.ExtItems));
            }

            return ret;
        }

        internal static List<ISection> MakeSkeleton() =>
        [
            new Section(new LVisualStudioVersion()),

            new Section(new LProject()),
            new Section(new LProjectDependencies()),
            new Section(new LProjectSolutionItems()),

            new Section(handler: null, Keywords.Global),

                new Section(new LSolutionConfigurationPlatforms()),
                new Section(new LProjectConfigurationPlatforms()),

                new Section(new LNestedProjects()),
                new Section(new LExtensibilityGlobals()),

            new Section(handler: null, Keywords.EndGlobal),
        ];
    }
}
