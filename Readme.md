# [MvsSln](https://github.com/3F/MvsSln)

MvsSln provides logic for complex support of the Visual Studio .sln files and its projects.

It was as a part of the [vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent) projects, but now it extracted into the new specially for [DllExport](https://github.com/3F/DllExport/issues/38) and for others.

[![Build status](https://ci.appveyor.com/api/projects/status/if1t4rhhntpf6ut3/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/mvssln/branch/master)
[![release-src](https://img.shields.io/github/release/3F/MvsSln.svg)](https://github.com/3F/MvsSln/releases/latest)
[![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/MvsSln/blob/master/License.txt)

**Download:** [/releases](https://github.com/3F/MvsSln/releases) ( [latest](https://github.com/3F/MvsSln/releases/latest) )

## License

The [MIT License (MIT)](https://github.com/3F/MvsSln/blob/master/License.txt)

```
Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
```

## Why MvsSln ?

Because today it still the most easy way for complex work with Visual Studio .sln files and its projects. Because it's free, because it's open.

Even if you just need basic accessing to information from the solution data, like: project Guids, paths, solution & projects configurations... or to calcualte map of projects via build order throught our ProjectDependencies helpers.

Control also easily your projects: Reference, ProjectReference, Properties, Import sections, and others.

Moreover, it has been re-licensed now (LGPLv3 -> MIT) from the vsSolutionBuildEvent projects, so, enjoy with us now.

```csharp
using(var sln = new Sln(@"D:\projects\Conari\Conari.sln", SlnItems.All & ~SlnItems.ProjectDependencies))
{
    //sln.Result.Env.XProjectByGuid(
    //    sln.Result.ProjectDependencies.FirstBy(BuildType.Rebuild).pGuid, 
    //    new ConfigItem("Debug", "Any CPU")
    //);

    var paths = sln.Result.ProjectItems
                            .Select(p => new { p.pGuid, p.fullPath })
                            .ToDictionary(p => p.pGuid, p => p.fullPath);

    // {[{27152FD4-7B94-4AF0-A7ED-BE7E7A196D57}, D:\projects\Conari\Conari\Conari.csproj]}
    // {[{0AEEC49E-07A5-4A55-9673-9346C3A7BC03}, D:\projects\Conari\ConariTest\ConariTest.csproj]}

    foreach(IXProject xp in sln.Result.Env.Projects)
    {
        xp.AddReference(typeof(JsonConverter).Assembly, true);
        xp.AddReference("System.Core");

        ProjectItem prj = ...
        xp.AddProjectReference(prj);

        xp.AddImport("../packages/DllExport.1.5.1/tools/net.r_eg.DllExport.targets", true);

        xp.SetProperty("JsonConverter", "30ad4fe6b2a6aeed", "'$(Configuration)' == 'Debug'");
        xp.SetProperties(new[] {
                            new PropertyItem("IsCrossTargetingBuild", "true"),
                            new PropertyItem("CSharpTargetsPath", "$(MSBToolsLocal)\\CSharp.CrossTargeting.targets")
                        }, 
                        "!Exists('$(MSBuildToolsPath)\\Microsoft.CSharp.targets')"
        );
        
        // ...
    }
    
    sln.Result.ProjectConfigs.Where(c => c.Sln.Configuration == "Debug"); // project-cfgs by solution-cfgs <- "Debug"
    // ...
    
} // release all loaded projects
```

By the way, the any new solution handler can be easily added by our flexible architecture. Example of `LProject` handler:

```csharp
public class LProject: LAbstract, ISlnHandler
{
    public override void Positioned(StreamReader stream, string line, SlnResult rsln)
    {
        if((rsln.ResultType & SlnItems.Projects) != SlnItems.Projects) {
            return;
        }

        if(!line.StartsWith("Project(", StringComparison.Ordinal)) {
            return;
        }

        if(rsln.ProjectItemList == null) {
            rsln.ProjectItemList = new List<ProjectItem>();
        }

        var pItem = new ProjectItem(line, rsln.SolutionDir);
        if(pItem.pGuid == null) {
            LSender.Send(this, $"LProject: The Guid is null or empty for line :: '{line}'", Message.Level.Error);
            return;
        }

        if(String.Equals(Guids.SLN_FOLDER, pItem.pType, StringComparison.OrdinalIgnoreCase)) {
            LSender.Send(this, $"{pItem.name} has been ignored as solution-folder :: '{line}'", Message.Level.Debug);
            return;
        }

        rsln.ProjectItemList.Add(pItem);
    }
}
```

## How to get

Available variants:

* [GetNuTool](https://github.com/3F/GetNuTool): `msbuild gnt.core /p:ngpackages="MvsSln"` or **[gnt](https://github.com/3F/GetNuTool/releases/download/v1.6/gnt.bat)** /p:ngpackages="MvsSln"
* NuGet PM: `Install-Package MvsSln`
* NuGet Commandline: `nuget install MvsSln`
* [GitHub Releases](https://github.com/3F/MvsSln/releases) ( [latest](https://github.com/3F/MvsSln/releases/latest) )
* [Nightly builds](https://ci.appveyor.com/project/3Fs/mvssln/history) (`/artifacts` page). But remember: It can be unstable or not work at all. Use this for tests of latest changes.


## &

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=entry%2ereg%40gmail%2ecom&lc=US&item_name=3F%2dOpenSource%20%5b%20github%2ecom%2f3F&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)
