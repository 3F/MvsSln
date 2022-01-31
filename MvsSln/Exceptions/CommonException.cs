/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;

namespace net.r_eg.MvsSln.Exceptions
{
    [Serializable]
    public class CommonException: Exception
    {
        public CommonException(string message, Exception innerException, params object[] args)
            : base(Format(ref message, args), innerException)
        {

        }

        public CommonException(string message, params object[] args)
            : base(Format(ref message, args))
        {

        }

        public CommonException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public CommonException(string message)
            : base(message)
        {

        }

        public CommonException()
        {

        }

        protected static string Format(ref string message, params object[] args)
        {
            return String.Format(message, args);
        }
    }
}