/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using net.r_eg.MvsSln.Core;

#if FEATURE_HUID
using net.r_eg.hashing;
#else
using System.Security.Cryptography;
#endif

namespace net.r_eg.MvsSln.Extensions
{
    using static Static.Members;

    public static class StringExtension
    {
        /// <summary>
        /// Get <see cref="System.Guid"/> for input string using specified hashing algorithm*
        /// </summary>
        /// <remarks>
        /// *Huid (Fnv-1a-128 (via LX4Cnh)), SHA-1, or MD5; depending on compilation options.
        /// 
        /// <br/><br/>
        /// Note: Huid and SHA-1 hashing works in <see cref="Guids.domainMvsSln"/> (the base),
        /// while implementation on MD5 uses initial vector.
        /// 
        /// <br/><br/>
        /// https://github.com/3F/MvsSln/issues/51
        /// </remarks>
        /// <param name="str">Any string data to generate <see cref="System.Guid"/></param>
        /// <returns>Either parsed GUID from string or new generated using specified hashing algorithm*</returns>
        public static Guid Guid(this string str)
        {
            if(System.Guid.TryParse(str, out Guid res)) return res;

            str ??= string.Empty;

            // Note about FIPS https://github.com/3F/DllExport/issues/171#issuecomment-752043556

#if FEATURE_HUID

            return Huid.NewGuid(Guids.domainMvsSln, str);

#elif FEATURE_GUID_SHA1

            const int _FMT = 16; // The UUID format is 16 octets

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            using HashAlgorithm alg = SHA1.Create();

            alg.TransformBlock(Guids.domainMvsSln.ToByteArray(), 0, _FMT, null, 0);
            alg.TransformFinalBlock(bytes, 0, bytes.Length);

            byte[] ret = new byte[_FMT];
            Array.Copy(alg.Hash, 0, ret, 0, _FMT);

            // 6-7 octets, the high field of the timestamp multiplexed with the version number;
            // *local byte order
            ret[7] &= 0x0F;
            ret[7] |= 5 << 4;
            /* rfc4122, UUID version ------v
                0     1     0     1        5     The name-based version specified in this document
                                                 that uses SHA-1 hashing.
            */

            // 8 octet, the high field of the clock sequence multiplexed with the variant;
            // *reserved
            ret[8] &= 0x3F;
            ret[8] |= 0x80;

            return new Guid(ret);

#else
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            //Note: legacy version does not use Guids.domainMvsSln

    #if NET5_0_OR_GREATER
            return new Guid(MD5.HashData(bytes));
    #else
            using MD5 alg = MD5.Create();
            return new Guid(alg.ComputeHash(bytes));
    #endif

#endif
        }

        /// <summary>
        /// Sln format of GUID:
        /// 32 uppercase digits separated by hyphens, enclosed in braces:
        /// ie. {100FD7F2-3278-49C7-B9D4-A91F1C65BED3}
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string SlnFormat(this Guid guid)
        {
            return guid.ToString("B").ToUpper();
        }

        /// <summary>
        /// Returns string GUID formated via `GuidSlnFormat`
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ReformatSlnGuid(this string guid)
        {
            return guid?.Trim().Guid().SlnFormat();
        }

        /// <summary>
        /// Return null when string is empty.
        /// </summary>
        public static string NullIfEmpty(this string str) => string.IsNullOrEmpty(str) ? null : str;

        /// <summary>
        /// Gets part of string before specific symbols.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c">Separators.</param>
        /// <returns>Left part of string before symbols, or null value if no any symbols are found.</returns>
        public static string Before(this string str, params char[] c)
        {
            int pos = str.IndexOfAny(c);
            if(pos == -1) {
                return null;
            }

            return str.Substring(0, pos);
        }

        /// <summary>
        /// Extracts file properties:
        /// SLN_DIR; SLN_EXT; SLN_FNAME; SLN_NAME; SLN_PATH
        /// </summary>
        /// <param name="file">Path to Solution file.</param>
        /// <returns>Use {PropertyNames} for accessing to extracted data.</returns>
        public static Dictionary<string, string> GetFileProperties(this string file)
        {
            if(string.IsNullOrEmpty(file))
            {
                return new Dictionary<string, string>()
                {
                    [PropertyNames.SLN_DIR]     = PropertyNames.UNDEFINED,
                    [PropertyNames.SLN_EXT]     = PropertyNames.UNDEFINED,
                    [PropertyNames.SLN_FNAME]   = PropertyNames.UNDEFINED,
                    [PropertyNames.SLN_NAME]    = PropertyNames.UNDEFINED,
                    [PropertyNames.SLN_PATH]    = PropertyNames.UNDEFINED,
                };
            }

            return new Dictionary<string, string>()
            {
                [PropertyNames.SLN_DIR]     = GetDirectoryFromFile(file),
                [PropertyNames.SLN_EXT]     = Path.GetExtension(file),
                [PropertyNames.SLN_FNAME]   = Path.GetFileName(file),
                [PropertyNames.SLN_NAME]    = Path.GetFileNameWithoutExtension(file),
                [PropertyNames.SLN_PATH]    = file,
            };
        }

        /// <summary>
        /// Get position of first non-WhiteSpace character from string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset">Initial position.</param>
        /// <param name="rightToLeft">Moving from right to left if true. Otherwise from left to right if false.</param>
        /// <returns></returns>
        public static int FirstNonWhiteSpace(this string str, int offset = 0, bool rightToLeft = false)
        {
            if(str == null) {
                return -1;
            }

            int i = offset;

            while(true)
            {
                if(i < 0 || i > str.Length - 1) {
                    return -1;
                }

                if(!char.IsWhiteSpace(str[i])) {
                    return i;
                }

                i += rightToLeft ? -1 : 1;
            }
        }

        /// <param name="file">File path; null is possible.</param>
        /// <returns></returns>
        public static string GetDirectoryFromFile(this string file)
        {
            if(file != null) {
                file = Path.GetDirectoryName(file);
            }
            return file.DirectoryPathFormat();
        }

        /// <summary>
        /// Formatting of the path to directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string DirectoryPathFormat(this string path)
        {
            if(String.IsNullOrWhiteSpace(path)) {
                return Path.DirectorySeparatorChar.ToString();
            }
            path = path.Trim();

            if(!IsDirectoryPath(path)) {
                path += Path.DirectorySeparatorChar;
            }
            
            return path;
        }

        /// <summary>
        /// Check if this is a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryPath(this string path)
        {
            return IsEndSlash(path?.TrimEnd());
        }

        /// <summary>
        /// Makes relative path from absolute.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakeRelativePath(this string root, string path)
        {
            if(String.IsNullOrWhiteSpace(root) || String.IsNullOrWhiteSpace(path)) {
                return null;
            }

            if(!Uri.TryCreate(root.AdaptPath().DirectoryPathFormat(), UriKind.Absolute, out Uri uriRoot)) {
                return null;
            }

            path = path.AdaptPath();

            if(!Uri.TryCreate(path, UriKind.Absolute, out Uri uriPath)) {
                uriPath = new Uri(uriRoot, new Uri(path, UriKind.Relative));
            }

            Uri urirel = uriRoot.MakeRelativeUri(uriPath);

            return Uri.UnescapeDataString
            (
                urirel.IsAbsoluteUri ? urirel.LocalPath : urirel.ToString()
            )
            .AdaptPath(forceIfWin: true);
        }

        /// <summary>
        /// Gets new stream from string.
        /// </summary>
        /// <remarks>Requires disposal using <see cref="IDisposable.Dispose"/></remarks>
        /// <param name="str"></param>
        /// <param name="enc">Specific encoding or null value to use UTF8 by default.</param>
        /// <returns></returns>
        public static Stream GetStream(this string str, Encoding enc = null)
        {
            return new MemoryStream((enc ?? Encoding.UTF8).GetBytes(str ?? string.Empty));
        }

        /// <param name="path">path to file</param>
        /// <returns>Platform independent file name without extension using `\` and `/` as a separator.</returns>
        internal static string GetFileNameWithoutExtension(this string path)
        {
            int a = path.IndexOfAny(['\\', '/']);
            int b = path.LastIndexOf('.');

            if(b <= a) b = path.Length;

            return path.Substring(++a, b - a);
        }

        /// <param name="path">path to file; null is possible</param>
        /// <returns>Either name from file (without extension) or its directory; trimmed; null is possible</returns>
        internal static string GetDirNameOrFileName(this string path)
        {
            //NOTE: Since `\`(backslash) is valid name for directories on Linux,
            //      Path.GetDirectoryName() will return wrong (for this case) empty string when "a\\b.c"; same to "\\ name" for Path.GetFileNameWithoutExtension()
            //      That's why we don't use System.IO.Path implementation here >_<

            if(string.IsNullOrEmpty(path)) return path;
            char[] sp = ['\\', '/'];

            int pos = path.LastIndexOfAny(sp);
            if(pos == -1) return path.GetFileNameWithoutExtension();

            // prevent possible double \\ triple \\\ ...
            int dirR = pos;
            while(dirR >= 0 && (path[dirR] == '\\' || path[dirR] == '/')) --dirR;

            if(dirR == -1) return path.Substring(pos).GetFileNameWithoutExtension();

            int dirL = path.LastIndexOfAny(sp, dirR);
            if(dirL == -1) return path.Substring(0, dirR + 1);

            return path.Substring(dirL + 1, dirR - dirL);
        }

        internal static string GetDirNameOrFileName(this string path, bool trim)
        {
            path = path.GetDirNameOrFileName();
            return trim ? path?.Trim() : path;
        }

        /// <summary>
        /// Adapt the path format to the current platform.
        /// </summary>
        internal static string AdaptPath(this string path, bool forceIfWin = false)
        {
            if(string.IsNullOrWhiteSpace(path)) return path;
            return IsUnixLikePath ? path.Replace('\\', '/') 
                     : forceIfWin ? path.Replace('/', '\\') : path;
        }

        internal static string Format(this string str, params object[] args)
            => string.IsNullOrWhiteSpace(str) ? str : string.Format(str, args);

        private static bool IsEndSlash(this string path)
        {
            if(path == null || path.Length < 1) return false;
            return path[path.Length - 1] is '\\' or '/';
        } 
    }
}