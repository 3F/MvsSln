/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Diagnostics;
using System.Text;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Core
{
    [DebuggerDisplay("{DbgDisplay}")]
    public struct RawText
    {
        public Encoding encoding;

        public string data;

        /// <summary>
        /// data without whitespace characters.
        /// </summary>
        public string trimmed;

        public static implicit operator string(RawText raw) => raw.ToString();

        public static implicit operator RawText(string str) => new RawText(str);

        public static bool operator ==(RawText a, RawText b) => a.Equals(b);

        public static bool operator !=(RawText a, RawText b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || !(obj is RawText)) {
                return false;
            }

            var b = (RawText)obj;

            return data == b.data
                    && trimmed == b.trimmed
                    && encoding == b.encoding;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                data,
                trimmed,
                encoding
            );
        }

        public override string ToString()
        {
            return data;
        }

        /// <param name="data"></param>
        public RawText(string data)
            : this(data, Encoding.UTF8)
        {

        }

        /// <param name="data"></param>
        /// <param name="enc"></param>
        public RawText(string data, Encoding enc)
        {
            this.data   = data;
            encoding    = enc;
            trimmed     = data?.Trim();
        }

        #region DebuggerDisplay

        private string DbgDisplay
        {
            get => data;
        }

        #endregion
    }
}