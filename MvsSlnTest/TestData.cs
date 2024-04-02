using net.r_eg.MvsSln.Extensions;

namespace MvsSlnTest
{
    internal sealed class TestData
    {
        internal const string ROOT = "resources\\";

        internal static string GetPathTo(string file) => $@"{ROOT}{file}".AdaptPath();

        internal static string GetPkgLegacyDir(string path) => $@"{path}packages\".AdaptPath();
    }
}
