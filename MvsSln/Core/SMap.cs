/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;

namespace net.r_eg.MvsSln.Core
{
    public sealed class SMap: List<ISection>
    {
        public enum RawSectionType
        {
            Global,

            EndGlobal
        }

        public enum AddType
        {
            Before,

            After,
        }

        public bool Add(AddType where, Type clause, ISection value)
            => Add(where, FindSection(where, s => Compare(s, clause)), value);

        public bool Add(AddType where, RawText clause, ISection value)
            => Add(where, FindSection(where, s => Compare(s, clause)), value);

        public bool Add(AddType where, RawSectionType clause, ISection value)
            => Add(where, ExtactRaw(clause), value);

        public bool Add(AddType where, int index, ISection value)
            => Add(where, index, i => Insert(i, value));

        public bool Add(AddType where, Type clause, IEnumerable<ISection> values)
            => Add(where, FindSection(where, s => Compare(s, clause)), values);

        public bool Add(AddType where, RawText clause, IEnumerable<ISection> values)
            => Add(where, FindSection(where, s => Compare(s, clause)), values);

        public bool Add(AddType where, RawSectionType clause, IEnumerable<ISection> values)
            => Add(where, ExtactRaw(clause), values);

        public bool Add(AddType where, int index, IEnumerable<ISection> values)
            => Add(where, index, i => InsertRange(i, values));

        public bool Remove(Type handler)
            => RemoveAll(s => Compare(s, handler)) > 0;

        public bool Remove(RawText raw)
            => RemoveAll(s => Compare(s, raw)) > 0;

        public bool Remove(RawSectionType raw) => Remove(ExtactRaw(raw));

        public SMap(IEnumerable<ISection> collection)
            : base(collection)
        {

        }

        public SMap()
        {

        }

        private static RawText ExtactRaw(RawSectionType raw) => new
        (
            raw switch
            {
                RawSectionType.Global => Keywords.Global,
                RawSectionType.EndGlobal => Keywords.EndGlobal,
                _ => throw new ArgumentOutOfRangeException(nameof(raw))
            }
        );

        private bool Add(AddType type, int index, Action<int> cbInsert)
        {
            if(cbInsert == null) throw new ArgumentNullException(nameof(cbInsert));
            if(index == -1 || index >= Count) return false;

            if(type == AddType.Before) { }
            else if(type == AddType.After) ++index;
            else throw new ArgumentOutOfRangeException(nameof(type));

            cbInsert(index);
            return true;
        }

        private int FindSection(AddType type, Predicate<ISection> predicate)
        {
            if(type == AddType.Before) return FindIndex(predicate);

            if(type != AddType.After) throw new ArgumentOutOfRangeException(nameof(type));

            int found = -1;
            for(int i = 0; i < Count; ++i)
            {
                if(predicate(this[i]))
                {
                    found = i;
                    continue;
                }
                else if(found != -1) return found;
            }
            return found;
        }

        private static bool Compare(ISection section, RawText raw)
        {
            if(section == null) throw new ArgumentNullException();

            return section.Raw == raw;
        }

        private static bool Compare(ISection section, Type handler)
        {
            if(section == null || handler == null) throw new ArgumentNullException();

            return section.Handler?.GetType() == handler;
        }
    }
}
