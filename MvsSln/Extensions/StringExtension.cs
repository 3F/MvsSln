/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2018  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace net.r_eg.MvsSln.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Gets Guid from hash by any string.
        /// </summary>
        /// <param name="str">String for calculating.</param>
        /// <returns></returns>
        public static Guid Guid(this string str)
        {
            if(str == null) {
                str = String.Empty;
            }
            using(MD5 md5 = MD5.Create()) {
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
            }
        }

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