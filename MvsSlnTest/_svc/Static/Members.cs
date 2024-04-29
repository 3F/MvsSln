using net.r_eg.MvsSln.Extensions;

namespace MvsSlnTest._svc.Static
{
    using static net.r_eg.MvsSln.Static.Members;

    public static class Members
    {
        /// <summary>
        /// Use <see cref="StringExtension.AdaptPath(string, bool)"/> and change root C:/path/... as /C/path/... if <see cref="IsUnixLikePath"/> is true.
        /// </summary>
        /// <param name="path">C:\path\ or like</param>
        /// <returns></returns>
        internal static string AdaptWinPath(this string path)
        {
            if(string.IsNullOrWhiteSpace(path)) return path;

            path = path.AdaptPath().TrimStart();

            if(path.Length < 2 || !IsUnixLikePath || path[1] != ':') return path;

            return $"/{path[0]}{path.Substring(2)}";
        }
    }
}