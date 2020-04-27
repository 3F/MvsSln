using System;
using System.IO;

namespace MvsSlnTest
{
    internal sealed class TestData
    {
        internal const string ROOT = "resources\\";

        internal static string PathTo(string file) => Path.Combine(ROOT, file ?? throw new ArgumentNullException(nameof(file)));
    }
}
