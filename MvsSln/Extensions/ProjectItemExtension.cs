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

using System.IO;
using System.Linq;
using System.Xml.Linq;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSln.Extensions
{
    public static class ProjectItemExtension
    {
        /// <summary>
        /// Is it C# ? Checking both legacy <see cref="ProjectType.Cs"/> and modern <see cref="ProjectType.CsSdk"/> types.
        /// </summary>
        public static bool IsCs(this ProjectItem prj) => prj.EpType == ProjectType.Cs || prj.EpType == ProjectType.CsSdk;

        /// <summary>
        /// Is it F# ? Checking both legacy <see cref="ProjectType.Fs"/> and modern <see cref="ProjectType.FsSdk"/> types.
        /// </summary>
        public static bool IsFs(this ProjectItem prj) => prj.EpType == ProjectType.Fs || prj.EpType == ProjectType.FsSdk;

        /// <summary>
        /// Is it Visual Basic ? Checking both legacy <see cref="ProjectType.Vb"/> and modern <see cref="ProjectType.VbSdk"/> types.
        /// </summary>
        public static bool IsVb(this ProjectItem prj) => prj.EpType == ProjectType.Vb || prj.EpType == ProjectType.VbSdk;

        /// <summary>
        /// While <see cref="ProjectType"/> cannot inform the actual use of the modern Sdk style in projects,
        /// current method will try to detect this by using the extended logic:
        /// https://github.com/dotnet/project-system/blob/master/docs/opening-with-new-project-system.md
        /// </summary>
        /// <returns>Returns false if this is a legacy style or if <see cref="ProjectItem.fullPath"/> is null or empty or path is not accessible. Otherwise true.</returns>
        /// <exception cref="System.Xml.XmlException">Requires valid XML data.</exception>
        public static bool IsSdk(this ProjectItem prj)
        {
            if(string.IsNullOrWhiteSpace(prj.fullPath) || !File.Exists(prj.fullPath))
            {
                return false;
            }

            /*
             Sdk-style, if:

               * C# and Visual Basic only
                 <= 16.3 raw text for </TargetFramework> or </TargetFrameworks>
                 >= 16.4 look for a <TargetFramework> or <TargetFrameworks> parented by a <PropertyGroup>
             
               * F# in 16.3 and earlier, and to F#, C#, and Visual Basic in 16.4 and later
                 Look for an Sdk attribute within a <Project> or <Import> element.
                   <Project Sdk="Microsoft.NET.Sdk">
                   or
                   <Project>
                     <Import Project="Sdk.props" Sdk="Microsoft.NET.Sdk" />
                     <Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />

                * In addition, for C# and Visual Basic projects only: 
                   <Sdk> element parented by a <Project> element
                   eg.
                    <Project>
                        <Sdk Name="Microsoft.NET.Sdk" Version="1.2.3" />
            */

            var xml = XDocument.Load(prj.fullPath);

            bool isCsOrVb = prj.IsCs() || prj.IsVb();

            if(isCsOrVb && xml.Descendants().Any(x => x.Name.LocalName == "TargetFramework" || x.Name.LocalName == "TargetFrameworks"))
            {
                //TODO: ??? what about <= 16.3 when raw text if comment <!-- ... </TargetFramework> -->
                return true;
            }

            if(xml.Descendants().Any(x => (x.Name.LocalName == "Project" || x.Name.LocalName == "Import") 
                                        && x.Attributes().Any(a => a.Name.LocalName == "Sdk")))
            {
                return true;
            }

            if(isCsOrVb && xml.Descendants().Any(x => x.Name.LocalName == "Sdk" && x.Parent?.Name.LocalName == "Project"))
            {
                return true;
            }

            return false;
        }
    }
}