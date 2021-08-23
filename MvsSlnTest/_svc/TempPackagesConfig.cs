using System;
using System.IO;
using net.r_eg.MvsSln.Projects;

namespace MvsSlnTest._svc
{
    internal sealed class TempPackagesConfig: PackagesConfig, IDisposable
    {
        public TempPackagesConfig(string path, PackagesConfigOptions options)
            : base(path, options)
        {

        }

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            if(!disposed)
            {
                if(IsNew) System.IO.File.Delete(file);

                disposed = true;
            }
        }

        #endregion
    }
}
