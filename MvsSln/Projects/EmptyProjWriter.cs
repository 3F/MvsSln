/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) MvsSln contributors https://github.com/3F/MvsSln/graphs/contributors
 * Licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/MvsSln
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Construction;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.MvsSln.Projects
{
    internal class EmptyProjWriter(Encoding encoding): IProjectsToucher
    {
        protected Encoding encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

        public void Touch(IEnumerable<ProjectItem> projects, bool strict = false)
            => projects?.ForEach(p => Touch(p, strict));

        public void Touch(ProjectItem project, bool strict = false)
        {
            if(string.IsNullOrEmpty(project.fullPath))
            {
                if(strict) throw new ArgumentNullException(nameof(project.fullPath));
                return;
            }

            if(File.Exists(project.fullPath)) return;

            switch(project.EpType)
            {
                case ProjectType.Cs: WriteCs(project); return;
                case ProjectType.CsSdk: WriteCsSdk(project); return;
                case ProjectType.Vc: WriteVc(project); return;
                case ProjectType.Vb: WriteVb(project); return;
                case ProjectType.VbSdk: WriteVbSdk(project); return;
                case ProjectType.Fs: WriteFs(project); return;
                case ProjectType.FsSdk: WriteFsSdk(project); return;
            }

            if(strict) throw new NotSupportedException();
        }

        public EmptyProjWriter()
            : this(Encoding.UTF8)
        {

        }

        protected void WriteVc(ProjectItem project)
        {
            ProjectRootElement root = CreateLegacyStyleProject(project.fullPath);

            root.AddImport("$(VCTargetsPath)\\Microsoft.Cpp.props");
            root.AddImport("$(VCTargetsPath)\\Microsoft.Cpp.targets");

            Save(root);
        }

        protected void WriteCs(ProjectItem project)
        {
            ProjectRootElement root = CreateLegacyStyleProject(project.fullPath);

            AddMicrosoftCommonProps(root);
            root.AddImport("$(MSBuildToolsPath)\\Microsoft.CSharp.targets");

            Save(root);
        }

        protected void WriteFs(ProjectItem project)
        {
            ProjectRootElement root = CreateLegacyStyleProject(project.fullPath);

            AddMicrosoftCommonProps(root);
            // TODO: import ???.targets

            Save(root);
        }

        protected void WriteVb(ProjectItem project)
        {
            ProjectRootElement root = CreateLegacyStyleProject(project.fullPath);

            AddMicrosoftCommonProps(root);
            root.AddImport("$(MSBuildToolsPath)\\Microsoft.VisualBasic.targets");

            Save(root);
        }

        protected void WriteCsSdk(ProjectItem project)
            => Save(GetMicrosoftNETSdkProject(), project.fullPath);

        protected void WriteVbSdk(ProjectItem project)
            => Save(GetMicrosoftNETSdkProject(), project.fullPath);

        protected void WriteFsSdk(ProjectItem project)
            => Save(GetMicrosoftNETSdkProject(), project.fullPath);

        protected virtual void Save(string data, string destination)
        {
            CreatePathIfNotExist(destination.GetDirectoryFromFile());
            File.WriteAllText
            (
                destination ?? throw new ArgumentNullException(nameof(destination)),
                data ?? throw new ArgumentNullException(nameof(data)),
                encoding
            );
        }

        protected virtual void Save(ProjectRootElement data)
        {
            CreatePathIfNotExist(data.FullPath.GetDirectoryFromFile());
            data.Save(encoding);
        }

        protected virtual ProjectRootElement CreateLegacyStyleProject(string path)
        {
            ProjectRootElement root = ProjectRootElement.Create(path);
            root.ToolsVersion = "4.0";
            root.DefaultTargets = "Build";
            return root;
        }

        protected string GetMicrosoftNETSdkProject()
            => CreateSdkStyleProject("Microsoft.NET.Sdk").ToString();

        protected XDocument CreateSdkStyleProject(string sdk)
        {
            // Sdk related options are available only for modern target platforms
            XElement root = new("Project", string.Empty);
            root.SetAttributeValue("Sdk", sdk);

            return new(root);
        }

        private static void CreatePathIfNotExist(string path)
        {
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private static void AddMicrosoftCommonProps(ProjectRootElement root)
        {
            ProjectImportElement props = root.AddImport("$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
            props.Condition = $"Exists('{props.Project}')";
        }
    }
}
