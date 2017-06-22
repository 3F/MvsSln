/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
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

using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln
{
    public sealed class Sln
    {
        private SlnParser parser = new SlnParser();
        private string file;

        public SlnItems Handler
        {
            get;
            private set;
        }

        /// <summary>
        /// Prepared solution data.
        /// </summary>
        public SlnResult SlnResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Solution Project Dependencies handler.
        /// </summary>
        public ISlnProjectDependencies SlnProjectDependencies
        {
            get;
            private set;
        }

        /// <param name="file">.sln file</param>
        /// <param name="type">Type of handler.</param>
        public Sln(string file, SlnItems type)
        {
            Handler     = type;
            this.file   = file;

            SlnResult = parser.Parse(file, type);

            if((type & SlnItems.SlnProjectDependencies) != 0) {
                SlnProjectDependencies = new SlnProjectDependencies(file);
            }
        }
    }
}
