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
    public class CoHandlerRuleException: NotFoundException
    {
        public CoHandlerRuleException(string message)
            : base(message)
        {

        }
    }
}