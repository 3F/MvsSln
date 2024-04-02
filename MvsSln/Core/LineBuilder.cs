/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Text;

namespace net.r_eg.MvsSln.Core
{
    public sealed class LineBuilder
    {
        private readonly StringBuilder sb;

        private string tab;

        private string doubleTab;

        private string newline;

        public string Tab
        {
            get => tab;
            set
            {
                tab = value ?? throw new ArgumentNullException(nameof(Tab));
                doubleTab = value + value;
            }
        }

        public string NewLine
        {
            get => newline;
            set => newline = value ?? Environment.NewLine;
        }

        public int Length => sb.Length;

        public LineBuilder Append(string value)
        {
            sb.Append(value ?? throw new ArgumentNullException(nameof(value)));
            return this;
        }

        public LineBuilder AppendLine(string value) => Append(value).Append(newline);

        public LineBuilder AppendLine() => Append(newline);

        public LineBuilder AppendLv1(string value) => Append(tab).Append(value);

        public LineBuilder AppendLv2(string value) => Append(doubleTab).Append(value);

        public LineBuilder AppendLv1Line(string value) => AppendLv1(value).AppendLine();

        public LineBuilder AppendLv2Line(string value) => AppendLv2(value).AppendLine();

        public LineBuilder RemoveNewLine()
            => ContainsLast(newline) ? RemoveLast(newline.Length) : this;

        public LineBuilder RemoveLast(int length)
        {
            if(length < 0 || length > sb.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return Remove(sb.Length - length, length);
        }

        public LineBuilder Remove(int startIndex, int length)
        {
            sb.Remove(startIndex, length);
            return this;
        }

        public LineBuilder Clear()
        {
            sb.Clear();
            return this;
        }

        public bool ContainsLast(string value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            if(sb.Length < value.Length || value == string.Empty) return false;
            return value == ToString(sb.Length - value.Length, value.Length);
        }

        public string ToString(bool removeNewLine)
        {
            if(!removeNewLine) return sb.ToString();

            return ContainsLast(newline) ? sb.ToString(0, sb.Length - newline.Length) : sb.ToString();
        }

        public string ToString(int startIndex, int length) => sb.ToString(startIndex, length);

        public override string ToString() => sb.ToString();

        internal LineBuilder(string newline = "\r\n", string tab = "\t")
        {
            Tab = tab ?? throw new ArgumentNullException(nameof(tab));
            NewLine = newline;

            sb = new StringBuilder();
        }
    }
}
