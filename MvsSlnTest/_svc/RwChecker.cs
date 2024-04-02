using System;
using System.Collections.Generic;
using System.IO;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Core.ObjHandlers;

namespace MvsSlnTest._svc
{
    internal sealed class RwChecker: IDisposable
    {
        internal delegate void CbLine(string line1, string line2, int position);

        private readonly StreamReader origin, mres;
        private readonly StreamWriter memdst;
        private readonly SlnWriter wres;

        public static void Check(Sln sln, Dictionary<Type, HandlerValue> whandlers)
        {
            using(new RwChecker(sln.Result, new(sln.Result.SolutionFile), whandlers).Check()) { }
        }

        public RwChecker Check() => Check
        (
            (l1, l2, i) =>
            throw new InvalidDataException($"{i}: {l1} == {l2}")
        );

        public RwChecker Check(CbLine ifNotEq)
        {
            if(ifNotEq == null) throw new ArgumentNullException(nameof(ifNotEq));

            memdst.BaseStream.Seek(0, SeekOrigin.Begin);
            int idx = 0;

            string l1;
            while((l1 = origin.ReadLine()) != null)
            {
                string l2 = mres.ReadLine();
                if(l1 != l2)
                {
                    ifNotEq(l1, l2, idx);
                }
                ++idx;
            }
            return this;
        }

        public RwChecker(ISlnResult rsln, StreamReader sr, Dictionary<Type, HandlerValue> whandlers)
        {
            origin = sr ?? throw new ArgumentNullException(nameof(sr));

            memdst = new(new MemoryStream());

            wres = new(memdst, whandlers);
            wres.Write(rsln.Map);

            memdst.Flush();
            mres = new(memdst.BaseStream);
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
                wres.Dispose();
                origin.Dispose();

                disposed = true;
            }
        }

        #endregion
    }
}
