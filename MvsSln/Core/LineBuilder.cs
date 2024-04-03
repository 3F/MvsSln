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

        /// <summary>
        /// Tab character or equivalent used in the current instance in related operations.
        /// </summary>
        public string Tab
        {
            get => tab;
            set
            {
                tab = value ?? throw new ArgumentNullException(nameof(Tab));
                doubleTab = value + value;
            }
        }

        /// <summary>
        /// EOL character (or a sequence of characters) used for newline operations in the current instance.
        /// </summary>
        /// <remarks>null as set value causes the value to be set using <see cref="Environment.NewLine"/></remarks>
        public string NewLine
        {
            get => newline;
            set => newline = value ?? Environment.NewLine;
        }

        public int Length => sb.Length;

        /// <summary>
        /// Adds string value to the current character set.
        /// </summary>
        /// <param name="value">String value to be added to the current character set.</param>
        /// <returns>Self reference to continue the chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public LineBuilder Append(string value)
        {
            sb.Append(value ?? throw new ArgumentNullException(nameof(value)));
            return this;
        }

        /// <summary>
        /// Adds string value together with <see cref="NewLine"/> to the current character set.
        /// </summary>
        /// <inheritdoc cref="Append(string)"/>
        public LineBuilder AppendLine(string value) => Append(value).Append(newline);

        /// <summary>
        /// Adds <see cref="NewLine"/> to the current character set.
        /// </summary>
        /// <returns>Self reference to continue the chain.</returns>
        public LineBuilder AppendLine() => Append(newline);

        /// <summary>
        /// <see cref="Append(string)"/> using first level indentation.
        /// </summary>
        /// <inheritdoc cref="Append(string)"/>
        public LineBuilder AppendLv1(string value) => Append(tab).Append(value);

        /// <summary>
        /// <see cref="Append(string)"/> using second level indentation.
        /// </summary>
        /// <inheritdoc cref="AppendLv1(string)"/>
        public LineBuilder AppendLv2(string value) => Append(doubleTab).Append(value);

        /// <summary>
        /// <see cref="AppendLine(string)"/> using first level indentation.
        /// </summary>
        /// <inheritdoc cref="AppendLv1(string)"/>
        public LineBuilder AppendLv1Line(string value) => AppendLv1(value).AppendLine();

        /// <summary>
        /// <see cref="AppendLine(string)"/> using second level indentation.
        /// </summary>
        /// <inheritdoc cref="AppendLv1Line(string)"/>
        public LineBuilder AppendLv2Line(string value) => AppendLv2(value).AppendLine();

        /// <summary>
        /// Remove the last <see cref="NewLine"/> from the current instance if present.
        /// </summary>
        /// <returns>Self reference to continue the chain.</returns>
        public LineBuilder RemoveLastNewLine()
            => ContainsLast(newline) ? RemoveLast(newline.Length) : this;

        /// <summary>
        /// Remove the last characters from the current instance.
        /// </summary>
        /// <param name="length">Number of characters being removed.</param>
        /// <returns>Self reference to continue the chain.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public LineBuilder RemoveLast(int length)
        {
            if(length < 0 || length > sb.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return Remove(sb.Length - length, length);
        }

        /// <inheritdoc cref="StringBuilder.Remove(int, int)"/>
        public LineBuilder Remove(int startIndex, int length)
        {
            sb.Remove(startIndex, length);
            return this;
        }

        /// <summary>
        /// Removes all characters from the current instance.
        /// </summary>
        /// <inheritdoc cref="AppendLine()"/>
        public LineBuilder Clear()
        {
            sb.Clear();
            return this;
        }

        /// <summary>
        /// Checks whether there is a sequence from the passed value at the end.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if the value being tested is at the end of the sequence of the current instance.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool ContainsLast(string value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            if(sb.Length < value.Length || value == string.Empty) return false;
            return value == ToString(sb.Length - value.Length, value.Length);
        }

        /// <param name="noLastNewLine">If true, remove <see cref="NewLine"/> at the end of the resulting string if present.</param>
        /// <inheritdoc cref="ToString()"/>
        public string ToString(bool noLastNewLine)
        {
            if(!noLastNewLine) return sb.ToString();

            return ContainsLast(newline) ? sb.ToString(0, sb.Length - newline.Length) : sb.ToString();
        }

        /// <inheritdoc cref="StringBuilder.ToString(int, int)"/>
        public string ToString(int startIndex, int length) => sb.ToString(startIndex, length);

        /// <inheritdoc cref="StringBuilder.ToString()"/>
        public override string ToString() => sb.ToString();

        public LineBuilder()
            : this(newline: null)
        {

        }

        public LineBuilder(string newline, string tab = "\t")
        {
            Tab = tab;
            NewLine = newline;

            sb = new StringBuilder();
        }
    }
}
