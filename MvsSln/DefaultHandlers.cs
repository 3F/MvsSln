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
        public static Dictionary<Type, HandlerValue> MakeFrom(ISlnWhData data)
        {
            if(data == null) throw new ArgumentNullException(nameof(data));
            Dictionary<Type, HandlerValue> ret = [];

            if(data.Header != null)
            {
                ret[typeof(LVisualStudioVersion)] = new(new WVisualStudioVersion(data.Header));
            }

            if(data.ProjectItems != null)
            {
                ret[typeof(LProject)] = new(new WProject(data.ProjectItems, data.ProjectDependencies));
            }

            //[typeof(LProjectDependencies)] = // part of LProject, see CoHandlers

            if(data.SolutionFolders != null)
            {
                ret[typeof(LProjectSolutionItems)] = new(new WProjectSolutionItems(data.SolutionFolders));
            }

            if(data.SolutionConfigs != null)
            {
                ret[typeof(LSolutionConfigurationPlatforms)] = new(new WSolutionConfigurationPlatforms(data.SolutionConfigs));
            }

            if(data.ProjectConfigs != null)
            {
                ret[typeof(LProjectConfigurationPlatforms)] = new(new WProjectConfigurationPlatforms(data.ProjectConfigs));
            }

            if(data.SolutionFolders != null || data.ProjectItems != null)
            {
                ret[typeof(LNestedProjects)] = new(new WNestedProjects(data.SolutionFolders, data.ProjectItems));
            }

            if(data.ExtItems != null)
            {
                ret[typeof(LExtensibilityGlobals)] = new(new WExtensibilityGlobals(data.ExtItems));
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
