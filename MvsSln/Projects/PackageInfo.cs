/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
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
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{DbgDisplay()}")]
    public sealed class PackageInfo: IPackageInfo
    {
        internal const string ATTR_ID    = "id";
        internal const string ATTR_VER   = "version";
        internal const string ATTR_TFM   = "targetFramework";
        internal const string ATTR_OUT   = "output";

        private readonly IPackagesConfig packagesConfig;
        private readonly Lazy<Version> _lversion;

        public string Id { get; }

        public string Version { get; }

        public Version VersionParsed => _lversion.Value;

        public IDictionary<string, string> Meta { get; }

        public string MetaTFM => Meta.GetOrDefault(ATTR_TFM);

        public string MetaOutput => Meta.GetOrDefault(ATTR_OUT);

        public IPackagesConfig Remove()
        {
            if(packagesConfig == null) throw new InvalidOperationException(nameof(PackageInfo) + "is not fully initialized.");

            packagesConfig.RemovePackage(this);
            return packagesConfig;
        }

        public static bool operator ==(PackageInfo a, PackageInfo b) => a.Equals(b);

        public static bool operator !=(PackageInfo a, PackageInfo b) => !(a == b);

        public override bool Equals(object obj)
        {
            if(obj is null || obj is not PackageInfo b) return false;

            if(Id != b.Id || Version != b.Version) return false;

            if((Meta is null || Meta.Count < 1) 
                && (b.Meta is null || b.Meta.Count < 1)) return true;

            if(Meta?.Count != b.Meta?.Count) return false;

            return !Meta.Except(b.Meta).Any();
        }

        public override int GetHashCode() => 0.CalculateHashCode
        (
            Id,
            Version,
            packagesConfig,
            Meta,
            MetaTFM,
            MetaOutput
        );

        public PackageInfo(string id, string version, IDictionary<string, string> meta = null)
        {
            Id          = id ?? throw new ArgumentNullException(nameof(id));
            Version     = version ?? throw new ArgumentNullException(nameof(version));
            _lversion   = new Lazy<Version>(() => new(version));

            Meta = meta;
        }

        public PackageInfo(XElement x)
            : this(x?.Attribute(ATTR_ID).Value, x?.Attribute(ATTR_VER).Value)
        {
            Meta = new Dictionary<string, string>();
            DefineAllMeta(x);
        }

        public PackageInfo(IPackagesConfig link, XElement x)
            : this(x)
        {
            packagesConfig = link ?? throw new ArgumentNullException(nameof(link));
        }

        private void DefineAllMeta(XElement x)
        {
            SetMeta(ATTR_TFM, x);
            SetMeta(ATTR_OUT, x);
        }

        private bool SetMeta(string name, XElement x)
        {
            XAttribute a = x.Attribute(name);
            if(a == null) return false;

            Meta[name] = a.Value;
            return true;
        }

        #region DebuggerDisplay

        private string DbgDisplay() => $"{Id} {Version}";

        #endregion
    }
}