/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace net.r_eg.MvsSln.Exceptions
{
    [Serializable]
    public class UnloadException<T>: CommonException, ISerializable
    {
        public T UnloadedInstance
        {
            get;
            protected set;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnloadedInstance), UnloadedInstance);
        }

        public UnloadException(string message, T instance)
            : base(message)
        {
            UnloadedInstance = instance;
        }
    }
}