/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// UUID using SHA-1 or MD5 hashing for string.
        /// </summary>
        /// <param name="str">Any string data for calculation.</param>
        public static Guid Guid(this string str)
        {
            if(System.Guid.TryParse(str, out Guid res)) {
                return res;
            }

            const int _FMT = 16; // The UUID format is 16 octets

            // FIPS https://github.com/3F/DllExport/issues/171#issuecomment-752043556
            const int _V // UUID version
#if DXP_EDITION
                = 5; // 5 points to SHA-1 hashing
#else
                = 3; // 3 points to MD5 hashing
#endif
            /* rfc4122

                0     0     1     1        3     The name-based version
                                                 specified in this document
                                                 that uses MD5 hashing.

                0     1     0     0        4     The randomly or pseudo-
                                                 randomly generated version
                                                 specified in this document.

                0     1     0     1        5     The name-based version
                                                 specified in this document
                                                 that uses SHA-1 hashing.
             */

            var bytes = Encoding.UTF8.GetBytes(str ?? string.Empty);

            using HashAlgorithm alg
#if DXP_EDITION
                = SHA1.Create();
#else
                = MD5.Create();
#endif

#if DXP_EDITION
            alg.TransformBlock(Guids.mvssln.ToByteArray(), 0, _FMT, null, 0);
#endif
            alg.TransformFinalBlock(bytes, 0, bytes.Length);

            var ret = new byte[_FMT];
            Array.Copy(alg.Hash, 0, ret, 0, _FMT);

#if DXP_EDITION
            // 6-7 octets, the high field of the timestamp multiplexed with the version number;
            // *local byte order
            ret[7] &= 0x0F;
            ret[7] |= _V << 4;

            // 8 octet, the high field of the clock sequence multiplexed with the variant;
            // *reserved
            ret[8] &= 0x3F;
            ret[8] |= 0x80;
#endif

            return new Guid(ret);
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

            if(!Uri.TryCreate(root.DirectoryPathFormat(), UriKind.Absolute, out Uri uriRoot)) {
                return null;
            }

            if(!Uri.TryCreate(path, UriKind.Absolute, out Uri uriPath)) {
                uriPath = new Uri(uriRoot, new Uri(path, UriKind.Relative));
            }

            Uri urirel  = uriRoot.MakeRelativeUri(uriPath);
            string ret  = Uri.UnescapeDataString(urirel.IsAbsoluteUri ? urirel.LocalPath : urirel.ToString());

            return ret.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets stream from string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enc">Specific encoding or null value to use UTF8 by default.</param>
        /// <returns></returns>
        public static Stream GetStream(this string str, Encoding enc = null)
        {
            return new MemoryStream((enc ?? Encoding.UTF8)
                            .GetBytes(str ?? String.Empty));
        }

        private static bool IsEndSlash(this string path)
        {
            if(path == null || path.Length < 1) {
                return false;
            }

            char c = path[path.Length - 1];
            if(c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) {
                return true;
            }
            return false;
        } 
    }
}