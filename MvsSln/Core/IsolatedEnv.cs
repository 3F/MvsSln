/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

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

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _)
        {
            if(!disposed)
            {
                if(Projects != null) UnloadAll(false);

                disposed = true;
            }
        }

        #endregion
    }
}