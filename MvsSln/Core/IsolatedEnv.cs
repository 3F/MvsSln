/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 * Copyright (c) MvsSln contributors: https://github.com/3F/MvsSln/graphs/contributors
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
using net.r_eg.MvsSln.Log;

namespace net.r_eg.MvsSln.Core
{
    /// <summary>
    /// Isolated environment.
    /// </summary>
    public class IsolatedEnv: XProjectEnv, IXProjectEnv, IEnvironment, IDisposable
    {
        /// <param name="data">Prepared data from solution parser.</param>
        /// <param name="properties">Specified sln properties.</param>
        /// <param name="raw">Optional dictionary of raw xml projects by Guid.</param>
        public IsolatedEnv(ISlnResult data, IDictionary<string, string> properties, IDictionary<string, RawText> raw = null)
            : base(data, properties, raw)
        {

        }

        /// <param name="data">Prepared data from solution parser.</param>
        /// <param name="raw">Optional dictionary of raw xml projects by Guid.</param>
        public IsolatedEnv(ISlnResult data, IDictionary<string, RawText> raw = null)
            : base(data, raw)
        {

        }

        //TODO: user option along with `LoadProjects` func
        protected bool Free()
        {
            if(Projects == null) {
                return true;
            }

            LSender.Send(this, $"Release loaded projects for current environment (total: {PrjCollection.LoadedProjects.Count})", Message.Level.Debug);
            foreach(var xp in Projects)
            {
                if(xp.Project == null) {
                    continue;
                }

                try
                {
                    if(xp.Project.FullPath != null) {
                        PrjCollection.UnloadProject(xp.Project);
                    }
                    else if(xp.Project.Xml != null) {
                        PrjCollection.TryUnloadProject(xp.Project.Xml);
                    }
                }
                catch(Exception ex) {
                    LSender.Send(this, $"Project '{xp.ProjectGuid}:{xp.ProjectItem.projectConfig}' was not unloaded: '{ex.Message}'", Message.Level.Trace);
                }
            }

            LSender.Send(this, $"Collection now contains '{PrjCollection.LoadedProjects.Count}' loaded projects.", Message.Level.Debug);
            return true;
        }

        #region IDisposable

        // To detect redundant calls
        private bool disposed = false;

        // To correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }
            disposed = true;

            //...
            Free();
        }

        #endregion
    }
}