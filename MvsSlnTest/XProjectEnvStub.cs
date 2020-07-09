using System.Collections.Generic;
using net.r_eg.MvsSln.Core;

namespace MvsSlnTest
{
    internal sealed class XProjectEnvStub: XProjectEnv
    {
        internal IDictionary<string, string> SlnProperties => slnProperties;

        internal XProjectEnvStub(ISlnResult data, IDictionary<string, string> properties, IDictionary<string, RawText> raw = null)
            : base(data, properties, raw)
        {

        }
    }
}
