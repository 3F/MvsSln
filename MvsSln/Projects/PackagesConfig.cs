/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace net.r_eg.MvsSln.Projects
{
    [DebuggerDisplay("{DbgDisplay()}")]
    public class PackagesConfig: IPackagesConfig
    {
        internal const string FNAME = "packages.config";
        internal const string ROOT  = "packages";
        internal const string ELEM  = "package";

        protected readonly string file;
        protected XDocument xml;
        protected XElement root;

        public bool AutoCommit { get; set; }

        public bool IsNew { get; protected set; }

        public string File => file;

        public IEnumerable<IPackageInfo> Packages
            => root.Elements().Select(l => new PackageInfo(this, l));

        public string DefaultTfm { get; set; } = "net40";

        public Exception FailedLoading { get; protected set; }

        public bool AddPackage(IPackageInfo package)
        {
            if(Locate(package?.Id) != null) return false;

            return NewPackage(package.Id, package.Version, package.Meta);
        }

        public bool AddPackage(string id, string version, string targetFramework = null)
        {
            if(Locate(id) != null) return false;

            return NewPackage(id, version, PackageInfo.ATTR_TFM, GetTfm(targetFramework));
        }

        public bool AddOrUpdatePackage(IPackageInfo package)
        {
            if(!AddPackage(package))
            {
                UpdatePackage(Locate(package.Id), package.Version, package.Meta);
                return false;
            }
            return true;
        }

        public bool AddOrUpdatePackage(string id, string version, string targetFramework = null)
        {
            string tfm = GetTfm(targetFramework);

            if(!AddPackage(id, version, tfm))
            {
                UpdatePackage(Locate(id), version, PackageInfo.ATTR_TFM, tfm);
                return false;
            }
            return true;
        }

        public bool AddGntPackage(string id, string version, string output = null)
        {
            if(Locate(id) != null) return false;

            return NewPackage(id, version, PackageInfo.ATTR_OUT, output);
        }

        public bool AddOrUpdateGntPackage(string id, string version, string output = null)
        {
            if(!AddGntPackage(id, version, output))
            {
                UpdatePackage(Locate(id), version, PackageInfo.ATTR_OUT, output);
                return false;
            }
            return true;
        }

        public void Commit()
        {
            xml.Save(file);
        }

        public IPackageInfo GetPackage(string id, bool icase = false)
        {
            XElement xe = Locate(id, icase);

            if(xe == null) return null;
            return new PackageInfo(this, xe);
        }

        public bool RemovePackage(IPackageInfo package) => RemovePackage(package?.Id);

        public bool RemovePackage(string id)
        {
            XElement xe = Locate(id);
            if(xe == null) return false;

            xe.Remove();
            UseAutoCommit();
            return true;
        }

        public void Rollback()
        {
            xml = Load(PackagesConfigOptions.LoadOrNew | PackagesConfigOptions.SilentLoading);
        }

        public bool UpdatePackage(IPackageInfo package)
        {
            return UpdatePackage(Locate(package?.Id), package.Version, package.Meta);
        }

        public bool UpdatePackage(string id, string version, string targetFramework = null)
        {
            return UpdatePackage(Locate(id), version, PackageInfo.ATTR_TFM, GetTfm(targetFramework));
        }

        public bool UpdateGntPackage(string id, string version, string output = null)
        {
            return UpdatePackage(Locate(id), version, PackageInfo.ATTR_OUT, output);
        }

        /// <param name="path">The path to the directory where the config is located (or must be located if new).</param>
        /// <param name="options">Configure initialization using <see cref="PackagesConfigOptions"/>.</param>
        public PackagesConfig(string path, PackagesConfigOptions options = PackagesConfigOptions.Default)
        {
            if(path == null) throw new ArgumentNullException(nameof(path));

            file = options.HasFlag(PackagesConfigOptions.PathToStorage) ? path : Path.Combine(path, FNAME);

            if((options & (PackagesConfigOptions.Load | PackagesConfigOptions.LoadOrNew)) == 0)
            {
                throw new NotSupportedException();
            }

            xml = Load(options);
        }

        protected PackagesConfig() { }

        protected XDocument Load(PackagesConfigOptions options)
        {
            xml     = LoadFile(options);
            root    = xml.Element(ROOT);
            return xml;
        }

        protected XDocument LoadFile(PackagesConfigOptions options)
        {
            if(!System.IO.File.Exists(file))
            {
                if(!options.HasFlag(PackagesConfigOptions.LoadOrNew))
                {
                    throw new FileNotFoundException(nameof(file));
                }
                return NewStorage();
            }

            if(!options.HasFlag(PackagesConfigOptions.SilentLoading))
            {
                return XDocument.Load(file);
            }

            try
            {
                return XDocument.Load(file);
            }
            catch(Exception ex)
            {
                FailedLoading = ex;
                return NewStorage();
            }
        }

        protected XElement Locate(string id, bool icase = false)
        {
            if(id == null) throw new ArgumentNullException(nameof(id));

            StringComparison flag = icase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return root.Elements().FirstOrDefault(p =>
                id.Equals(GetAttr(PackageInfo.ATTR_ID, p), flag)
            );
        }

        protected virtual XElement NewPackage(string id, string version)
        {
            XElement xe = new
            (
                ELEM,
                new XAttribute(PackageInfo.ATTR_ID, id),
                new XAttribute(PackageInfo.ATTR_VER, version)
            );

            root.Add(xe);
            return xe;
        }

        /// <returns>always true</returns>
        protected bool NewPackage(string id, string version, IDictionary<string, string> meta)
        {
            UpdatePackageMeta(NewPackage(id, version), meta);
            UseAutoCommit();
            return true;
        }

        /// <returns>always true</returns>
        protected bool NewPackage(string id, string version, string metaKey, string metaValue)
        {
            UpdatePackageMeta(NewPackage(id, version), metaKey, metaValue);
            UseAutoCommit();
            return true;
        }

        protected virtual XElement UpdatePackage(XElement xe, string version)
        {
            xe.SetAttributeValue(PackageInfo.ATTR_VER, version);
            return xe;
        }

        protected bool UpdatePackage(XElement xe, string version, IDictionary<string, string> meta)
        {
            if(xe != null)
            {
                UpdatePackageMeta(UpdatePackage(xe, version), meta);
                UseAutoCommit();
                return true;
            }
            return false;
        }

        protected virtual void UpdatePackageMeta(XElement xe, IDictionary<string, string> meta)
        {
            if(meta != null)
            {
                foreach(var m in meta) xe.SetAttributeValue(m.Key, m.Value);
            }
        }

        protected bool UpdatePackage(XElement xe, string version, string metaKey, string metaValue)
        {
            if(xe != null)
            {
                UpdatePackageMeta(UpdatePackage(xe, version), metaKey, metaValue);
                UseAutoCommit();
                return true;
            }
            return false;
        }

        protected virtual void UpdatePackageMeta(XElement xe, string key, string value)
        {
            if(key != null)
            {
                xe.SetAttributeValue(key, value);
            }
        }

        protected void UseAutoCommit()
        {
            if(AutoCommit)
            {
                Commit();
            }
        }

        private string GetAttr(string name, XElement xe) => xe.Attribute(name).Value;

        private string GetTfm(string input) => input ?? DefaultTfm;

        private XDocument NewStorage()
        {
            IsNew = true;
            return new(new XElement(ROOT));
        }

        #region DebuggerDisplay

        private string DbgDisplay() => $"{file}  ({nameof(IsNew)}={IsNew}; {nameof(AutoCommit)}={AutoCommit})";

        #endregion
    }
}