/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using net.r_eg.MvsSln.Core.SlnHandlers;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Projects;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Parser for basic elements from .sln files.
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class SlnParser: ISlnContainer
    {
        /// <summary>
        /// The name of file if used stream from memory.
        /// </summary>
        public const string MEM_FILE = "$None.sln$";

        /// <summary>
        /// To use specific Encoding by default for some operations with data.
        /// </summary>
        protected Encoding encoding = Encoding.Default;

        private CoHandlers _coh;

        /// <summary>
        /// Available solution handlers.
        /// </summary>
        public SynchSubscribers<ISlnHandler> SlnHandlers
        {
            get;
            protected set;
        } = new SynchSubscribers<ISlnHandler>();

        /// <summary>
        /// Dictionary of raw xml projects by Guid.
        /// Will be used if projects cannot be accessed from filesystem.
        /// </summary>
        public IDictionary<string, RawText> RawXmlProjects
        {
            get;
            set;
        }

        /// <summary>
        /// To reset and register all default handlers.
        /// </summary>
        public void SetDefaultHandlers()
        {
            SlnHandlers.Reset();

            SlnHandlers.Register(new LVisualStudioVersion());
            SlnHandlers.Register(new LProject());
            SlnHandlers.Register(new LProjectDependencies());
            SlnHandlers.Register(new LProjectSolutionItems());
            SlnHandlers.Register(new LNestedProjects());
            SlnHandlers.Register(new LProjectConfigurationPlatforms());
            SlnHandlers.Register(new LSolutionConfigurationPlatforms());
            SlnHandlers.Register(new LExtensibilityGlobals());

            // TODO: validate CoHandlers ref
        }

        /// <summary>
        /// Parse of selected .sln file.
        /// </summary>
        /// <param name="sln">Solution file</param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        public ISlnResult Parse(string sln, SlnItems type)
        {
            if(string.IsNullOrWhiteSpace(sln)) {
                throw new ArgumentNullException(nameof(sln), MsgResource.ValueNoEmptyOrNull);
            }

            using var reader = new StreamReader(sln, encoding);
            return Parse(reader, type);
        }

        /// <summary>
        /// To parse data from used stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type">Allowed type of operations.</param>
        /// <returns></returns>
        public ISlnResult Parse(StreamReader reader, SlnItems type)
        {
            if(reader == null) {
                throw new ArgumentNullException(nameof(reader), MsgResource.ValueNoEmptyOrNull);
            }

            string sln = (reader.BaseStream is FileStream stream) ? stream.Name : MEM_FILE;

            var data = new SlnResult() {
                SolutionDir = sln.GetDirectoryFromFile(),
                ResultType  = type,
            };

            Process(new Svc(reader, data));

            if(data.SolutionConfigs != null)
            {
                data.DefaultConfig = new ConfigItem(
                    ExtractDefaultConfiguration(data.SolutionConfigs), 
                    ExtractDefaultPlatform(data.SolutionConfigs)
                );
            }

            data.Properties = GlobalProperties(
                sln, 
                data.DefaultConfig?.Configuration, 
                data.DefaultConfig?.Platform
            );

            Aliases(data);

            if((type & SlnItems.Env) == SlnItems.Env)
            {
                data.Env = new IsolatedEnv(data, RawXmlProjects);
                if((type & SlnItems.EnvWithMinimalProjects) == SlnItems.EnvWithMinimalProjects) {
                    data.Env.LoadMinimalProjects();
                }
                if((type & SlnItems.EnvWithProjects) == SlnItems.EnvWithProjects) {
                    data.Env.LoadProjects();
                }
            }

            if((type & SlnItems.ProjectDependenciesXml) == SlnItems.ProjectDependenciesXml)
            {
                if(data.ProjectDependencies != null && data.Env?.Projects != null)
                {
                    // The following class provides additional features for project references in ISlnPDManager manner, 
                    // But we'll just activate references for existing ProjectDependencies (shallow copy)
                    // just to eliminate miscellaneous units between VS and msbuild world: https://github.com/3F/MvsSln/issues/25
                    new ProjectReferences(data.ProjectDependencies, data.Env.Projects);
                }
            }

            data.PackagesConfigs = PackagesConfigLocator.FindAndLoadConfigs(data, type);

            return data;
        }

        /// <param name="defaultHandlers">To register and activate all handlers by default if true.</param>
        public SlnParser(bool defaultHandlers = true)
        {
            if(defaultHandlers) {
                SetDefaultHandlers();
            }
        }

        protected void Process(ISvc svc)
        {
            _coh = new CoHandlers(SlnHandlers);

            DoPreProcessing(svc);
            {
                string line;
                while((line = svc.ReadLine()) != null) {
                    DoPositioned(svc, new RawText(line, svc.CurrentEncoding));
                }
            }
            DoPostProcessing(svc);
        }

        protected virtual void DoPreProcessing(ISvc svc)
        {
            SlnHandlers.ForEach((h) => h.PreProcessing(svc));
        }

        protected virtual void DoPositioned(ISvc svc, RawText line)
        {
            foreach(ISlnHandler h in SlnHandlers)
            {
                if(!h.Condition(line)) {
                    continue;
                }

                if(TrackedPosition(h, svc, line)
                    && (h.CoHandlers == null || !_coh.Contains(h.Id)))
                {
                    return;
                }
            }

            svc.Track(line);
        }

        protected virtual void DoPostProcessing(ISvc svc)
        {
            SlnHandlers.ForEach((h) => h.PostProcessing(svc));
        }

        /// <summary>
        /// TODO: another way to manage aliases for data.
        /// </summary>
        /// <param name="data"></param>
        protected void Aliases(SlnResult data)
        {
            SetProjectConfigurationPlatforms(data);
            SetProjectItemsConfigs(data);
        }

        protected void SetProjectConfigurationPlatforms(SlnResult data)
        {
            if(data.SolutionConfigs == null || data.ProjectConfigs == null) {
                return;
            }

            var ret = new Dictionary<IConfPlatform, IConfPlatformPrj[]>();
            foreach(var sln in data.SolutionConfigs) {
                ret[sln] = data.ProjectConfigs.Where(p => (ConfigItem)p.Sln == (ConfigItem)sln).ToArray();
            }

            data.ProjectConfigurationPlatforms = new RoProperties<IConfPlatform, IConfPlatformPrj[]>(ret);
        }

        protected void SetProjectItemsConfigs(SlnResult data)
        {
            if(data.ProjectConfigurationPlatforms == null) {
                return;
            }

            var ret = new List<ProjectItemCfg>();
            foreach(var slnConf in data.ProjectConfigurationPlatforms)
            {
                foreach(var prjConf in slnConf.Value)
                {
                    ProjectItem pItem;

                    if(data.ProjectItems == null) {
                        pItem = default;
                    }
                    else {
                        pItem = data.ProjectItems.FirstOrDefault(p => p.pGuid.Guid() == prjConf.PGuid.Guid());
                    }

                    ret.Add(new ProjectItemCfg(pItem, slnConf.Key, prjConf));
                }
            }
            data.ProjectItemsConfigs = ret;
        }

        protected RoProperties GlobalProperties(string sln, string configuration, string platform)
        {
            return new RoProperties(new Dictionary<string, string>(sln.GetFileProperties())
            {
                [PropertyNames.CONFIG]      = configuration,
                [PropertyNames.PLATFORM]    = platform
            });
        }

        protected virtual string ExtractDefaultConfiguration(IEnumerable<IConfPlatform> cfg)
        {
            foreach(IConfPlatform c in cfg) {
                if(c.Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase)) {
                    return c.Configuration;
                }
            }

            foreach(IConfPlatform c in cfg) {
                return c.Configuration;
            }
            return null;
        }

        protected virtual string ExtractDefaultPlatform(IEnumerable<IConfPlatform> cfg)
        {
            foreach(IConfPlatform c in cfg)
            {
                if(c.Platform.Equals("Mixed Platforms", StringComparison.OrdinalIgnoreCase)) {
                    return c.Platform;
                }

                if(c.Platform.Equals("Any CPU", StringComparison.OrdinalIgnoreCase)) {
                    return c.Platform;
                }
            }

            foreach(IConfPlatform c in cfg) {
                return c.Platform;
            }
            return null;
        }

        private bool TrackedPosition(ISlnHandler h, ISvc svc, RawText line)
        {
            bool isAct = h.IsActivated(svc);

            TransactTracking<ISection, IList<ISection>> tt = null;
            if(h.LineControl == LineAct.Process) {
                tt = svc.TransactTrack(line, isAct ? h : null);
            }

            if(!isAct) {
                return false;
            }

            bool res = h.Positioned(svc, line);
            tt?.Action(res ? TransactAction.Commit : TransactAction.Rollback);

            return res;
        }
    }
}
