/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System.Collections.Generic;

namespace net.r_eg.MvsSln.Projects
{
    public interface IPackagesConfig
    {
        /// <summary>
        /// Use auto-commit for each adding/updating/removing.
        /// </summary>
        bool AutoCommit { get; set; }

        /// <summary>
        /// Flag of the new created storage.
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// Selected file.
        /// </summary>
        string File { get; }

        /// <summary>
        /// List all packages from storage.
        /// </summary>
        IEnumerable<IPackageInfo> Packages { get; }

        /// <summary>
        /// Get specific package by its id.
        /// </summary>
        /// <param name="id">Package id. Eg. "Conari"; "7z.Libs"; "regXwild"; "MvsSln"; "vsSolutionBuildEvent"; ...</param>
        /// <param name="icase">Ignore case for id if true.</param>
        /// <returns>null if not found.</returns>
        IPackageInfo GetPackage(string id, bool icase = false);

        /// <summary>
        /// Commit changes made by adding/updating/removing.
        /// </summary>
        void Commit();

        /// <summary>
        /// Roll back changes made by adding/updating/removing if not yet committed.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Add package by using <see cref="IPackageInfo"/> instance.
        /// Package cannot be added if the same <see cref="IPackageInfo.Id"/> is already exists in active storage.
        /// </summary>
        /// <returns>True, if the package has been successfully added.</returns>
        bool AddPackage(IPackageInfo package);

        /// <summary>
        /// Add package or Update an existing by using <see cref="IPackageInfo"/> instance.
        /// </summary>
        /// <returns>True, if the package has been added. False, if the package has been updated.</returns>
        bool AddOrUpdatePackage(IPackageInfo package);

        /// <summary>
        /// Update an existing package by using <see cref="IPackageInfo"/> instance.
        /// </summary>
        /// <returns>True, if the package has been successfully updated. False, if the package does not exist.</returns>
        bool UpdatePackage(IPackageInfo package);

        /// <summary>
        /// Add package.
        /// Package cannot be added if the same <see cref="IPackageInfo.Id"/> is already exists in active storage.
        /// </summary>
        /// <param name="id"><see cref="IPackageInfo.Id"/></param>
        /// <param name="version"><see cref="IPackageInfo.Version"/></param>
        /// <param name="targetFramework"><see cref="IPackageInfo.Meta"/> information for `targetFramework`. Use null to set the default value.</param>
        /// <returns>True, if the package has been successfully added.</returns>
        bool AddPackage(string id, string version, string targetFramework = null);

        /// <summary>
        /// Add package or Update an existing.
        /// </summary>
        /// <returns>True, if the package has been added. False, if the package has been updated.</returns>
        /// <inheritdoc cref="AddPackage(string, string, string)"/>
        bool AddOrUpdatePackage(string id, string version, string targetFramework = null);

        /// <summary>
        /// Update an existing package.
        /// </summary>
        /// <returns>True, if the package has been successfully updated. False, if the package does not exist.</returns>
        /// <inheritdoc cref="AddPackage(string, string, string)"/>
        bool UpdatePackage(string id, string version, string targetFramework = null);

        /// <summary>
        /// Add GetNuTool compatible package.
        /// </summary>
        /// <param name="id"><see cref="IPackageInfo.Id"/></param>
        /// <param name="version"><see cref="IPackageInfo.Version"/></param>
        /// <param name="output"><see cref="IPackageInfo.Meta"/> information for `output` https://github.com/3F/GetNuTool#format-of-packages-list </param>
        /// <returns>True, if the package has been successfully added.</returns>
        /// <remarks>https://github.com/3F/GetNuTool</remarks>
        bool AddGntPackage(string id, string version, string output = null);

        /// <summary>
        /// Add GetNuTool compatible package or Update an existing.
        /// </summary>
        /// <returns>True, if the package has been added. False, if the package has been updated.</returns>
        /// <inheritdoc cref="AddGntPackage(string, string, string)"/>
        bool AddOrUpdateGntPackage(string id, string version, string output = null);

        /// <summary>
        /// Update an existing GetNuTool compatible package.
        /// </summary>
        /// <returns>True, if the package has been successfully updated. False, if the package does not exist.</returns>
        /// <inheritdoc cref="AddGntPackage(string, string, string)"/>
        bool UpdateGntPackage(string id, string version, string output = null);

        /// <summary>
        /// Remove package by using <see cref="IPackageInfo"/> instance.
        /// </summary>
        /// <returns>True, if it was successfully deleted. False, if it does not exist.</returns>
        bool RemovePackage(IPackageInfo package);

        /// <summary>
        /// Remove package.
        /// </summary>
        /// <param name="id"><see cref="IPackageInfo.Id"/></param>
        /// <returns>True, if it was successfully deleted. False, if it does not exist.</returns>
        bool RemovePackage(string id);
    }
}