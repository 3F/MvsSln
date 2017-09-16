# [MvsSln](https://github.com/3F/MvsSln)

[![](https://raw.githubusercontent.com/3F/MvsSln/master/MvsSln/Resources/MvsSln_v1_96px.png)](https://github.com/3F/MvsSln)

MvsSln provides logic for complex support of the Visual Studio .sln files and its projects (.vcxproj, .csproj., ...).

It was as a part of the [vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent) projects, but now it extracted into the new specially for [DllExport](https://github.com/3F/DllExport/issues/38) and for others.

[![Build status](https://ci.appveyor.com/api/projects/status/if1t4rhhntpf6ut3/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/mvssln/branch/master)
[![release-src](https://img.shields.io/github/release/3F/MvsSln.svg)](https://github.com/3F/MvsSln/releases/latest)
[![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/MvsSln/blob/master/License.txt)
[![NuGet package](https://img.shields.io/nuget/v/MvsSln.svg)](https://www.nuget.org/packages/MvsSln/) 

**Download:** [/releases](https://github.com/3F/MvsSln/releases) [ **[latest](https://github.com/3F/MvsSln/releases/latest)** ]

[`gnt`](https://3f.github.io/GetNuTool/releases/latest/gnt/)` /p:ngpackages="MvsSln"` [[?](https://github.com/3F/GetNuTool)]

## License

The [MIT License (MIT)](https://github.com/3F/MvsSln/blob/master/License.txt)

```
Copyright (c) 2013-2017  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
```

## Why MvsSln ?

Because today it still is the most easy way for complex work with Visual Studio .sln files and its projects (.vcxproj, .csproj., ...). Because it's free, because it's open.

Even if you just need the basic access to information from the solution data, like: project Guids, paths, solution & projects configurations, calculating map of projects via build-order through our ProjectDependencies helpers, etc.

You can also control easily all your projects: Reference, ProjectReference, Properties, Import sections, and others.

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
                            new PropertyItem("CSharpTargetsPath", "$(MSBToolsLocal)\\CrossTargeting.targets")
                        }, 
                        "!Exists('$(MSBuildToolsPath)\\Microsoft.CSharp.targets')"
        );
        
        // ...
    }
    
    sln.Result.ProjectConfigs.Where(c => c.Sln.Configuration == "Debug"); // project-cfgs by solution-cfgs
    // ...
    
} // release all loaded projects
```

By the way, the any new solution handler can be easily added by our flexible architecture. Example of `LProject` handler (**reader**):

```csharp
public class LProject: LAbstract, ISlnHandler
{
    public override bool IsActivated(ISvc svc)
    {
        return ((svc.Sln.ResultType & SlnItems.Projects) == SlnItems.Projects);
    }

    public override bool Condition(RawText line)
    {
        return line.trimmed.StartsWith("Project(", StringComparison.Ordinal);
    }

    public override bool Positioned(ISvc svc, RawText line)
    {
        var pItem = GetProjectItem(line.trimmed, svc.Sln.SolutionDir);
        if(pItem.pGuid == null) {
            return false;
        }

        if(svc.Sln.ProjectItemList == null) {
            svc.Sln.ProjectItemList = new List<ProjectItem>();
        }

        svc.Sln.ProjectItemList.Add(pItem);
        return true;
    }
}
```

Example of `WSolutionConfigurationPlatforms` handler (**writer**):

```csharp
public class WSolutionConfigurationPlatforms: WAbstract, IObjHandler
{
    protected IEnumerable<IConfPlatform> configs;

    public override string Extract(object data)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{SP}GlobalSection(SolutionConfigurationPlatforms) = preSolution");

        configs.ForEach(cfg => sb.AppendLine($"{SP}{SP}{cfg} = {cfg}"));

        sb.Append($"{SP}EndGlobalSection");

        return sb.ToString();
    }

    public WSolutionConfigurationPlatforms(IEnumerable<IConfPlatform> configs)
    {
        this.configs = configs ?? throw new ArgumentNullException();
    }
}
```

## Examples of using

### DllExport configurator

The final draft-version of the new configurator for DllExport now fully works via MvsSln:

* https://github.com/3F/DllExport
    * https://github.com/3F/DllExport/issues/38


### Map of .sln & Writers

v2+ also may provides map of analyzed data. To enable this define a bit **0x0080** for type of operations to parser.

Parser will expose map through list of `ISection` for each line. For example:

```
...
"[] #4:'VisualStudioVersion = 14.0.25420.1'"
"[] #5:'MinimumVisualStudioVersion = 10.0.30319.1'"
"[LProject] #6:'Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"CIMLib\", \"CIMLib\\CIMLib.csproj\", \"...
"[LProjectDependencies] #7:'EndProject'"
"[LProject] #8:'Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"vsSolutionBuildEvent\", ....
"[LProjectDependencies] #9:'\tProjectSection(ProjectDependencies) = postProject'"
"[LProjectDependencies] #10:'\t\t{73919171-44B6-4536-B892-F1FCA653887C} = {73919171-44B6-4536-B892-F1FCA653887C}'"
"[LProjectDependencies] #11:'\t\t{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4} = {A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}'"
"[LProjectDependencies] #12:'\tEndProjectSection'"
...
"[LSolutionConfigurationPlatforms] #62:'\tGlobalSection(SolutionConfigurationPlatforms) = preSolution'"
"[LSolutionConfigurationPlatforms] #63:'\t\tCI_Debug_net45|Any CPU = CI_Debug_net45|Any CPU'"
"[LSolutionConfigurationPlatforms] #64:'\t\tCI_Debug|Any CPU = CI_Debug|Any CPU'"
.....
"[LSolutionConfigurationPlatforms] #70:'\t\tRelease|Any CPU = Release|Any CPU'"
"[LSolutionConfigurationPlatforms] #71:'\tEndGlobalSection'"
"[LProjectConfigurationPlatforms] #72:'\tGlobalSection(ProjectConfigurationPlatforms) = postSolution'"
"[LProjectConfigurationPlatforms] #73:'\t\t{A7BF1F9C-F18D-423E-9354-859DC3CFAFD4}.CI_Debug_net45|Any CPU.Activ...
...
```
* Each section contains handler which processes this line + simple access via RawText if not. 
* All this may be overloaded by any custom handlers (readers - `ISlnHandler`) if it's required by your environment.

This map may be used for modification / define new .sln data through writers (`IObjHandler`). For example:

```csharp
var data = new List<IConfPlatform>() {
    new ConfigSln("Debug", "Any CPU"),
    new ConfigSln("Release_net45", "x64"),
    new ConfigSln("Release", "Any CPU"),
};

var whandlers = new Dictionary<Type, HandlerValue>() {
    [typeof(LSolutionConfigurationPlatforms)] = new HandlerValue(new WSolutionConfigurationPlatforms(data)),
};

using(var w = new SlnWriter("<path_to>.sln", whandlers)) {
    w.Write(map);
}
```

## Did you know ?

### Projects

The 1 project instance means only the 1 project with specific configuration. That is, you should work with each instance separately if some project has 2 or more configurations:

```
First instance of project {4F8BB8CD-1116-4F07-9B8F-06D69FB8589B} with configuration 'Release_net45|Any CPU' that's related with solution cfg -> CI_Release_net45|Any CPU
Second instance of project {4F8BB8CD-1116-4F07-9B8F-06D69FB8589B} with configuration 'Debug|Any CPU' that's related with solution cfg -> Debug|Any CPU
...
```

For example, the [vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent) contains 8 projects and 8 solution configurations:

```
CI_Debug_net45|Any CPU
CI_Debug|Any CPU
CI_Release_net45|Any CPU
CI_Release|Any CPU
Debug_net45|Any CPU
Debug|Any CPU
Release_net45|Any CPU
Release|Any CPU
```

The all available configurations for each projects should be 8 * 8 = 64, i.e. 64 instances that can be loaded as each different projects. `EnvWithProjects` will load all available projects and you finally should see 64 different instances, as for vsSolutionBuildEvent above. 

However, if you only need to work with common data of selected project: you just need to use any available configuration. To load projects only with specific configuration, use for example `IEnvironment.LoadProjects`:

```csharp
// SlnItems.Env will initialize environment without loading projects.
using(var sln = new Sln(@"vsSolutionBuildEvent.sln", SlnItems.Env))
{
    ISlnResult data         = sln.Result;
    IConfPlatform slnCfg    = data.SolutionConfigs.FirstOrDefault(); // to get first available solution configuration
    data.Env.LoadProjects(
        // prepare final list of projects that should be loaded
        data.ProjectItemsConfigs.Where(p => p.solutionConfig == slnCfg)
    );    
    //... data.Env.Projects will contain instances only for Where(p => p.solutionConfig == slnCfg) i.e. 8 in total
}
```

With latest version should be also available `IEnvironment.LoadMinimalProjects` or `EnvWithMinimalProjects` flag.

### Adding Reference & Assembly name

```csharp
XProject.AddReference(lib, false);
```

```xml
<Reference Include="DllExport, Version=1.5.2.42159, Culture=neutral, PublicKeyToken=8337224c9ad9e356">
  <HintPath>..\packages\DllExport.1.5.2\gcache\metalib\DllExport.dll</HintPath>
  <Private>False</Private>
</Reference>
```

```csharp
XProject.AddReference("DllExport", lib, false);
```

```xml
<Reference Include="DllExport">
  <HintPath>..\packages\DllExport.1.5.2\gcache\metalib\DllExport.dll</HintPath>
  <Private>False</Private>
</Reference>
```

You can also specify it via `System.Reflection.Assembly` etc.


## How to get

Available variants:

* [GetNuTool](https://github.com/3F/GetNuTool): `msbuild gnt.core /p:ngpackages="MvsSln"` or **[gnt](https://3f.github.io/GetNuTool/releases/latest/gnt/)** /p:ngpackages="MvsSln"
* NuGet PM: `Install-Package MvsSln`
* NuGet Commandline: `nuget install MvsSln`
* [GitHub Releases](https://github.com/3F/MvsSln/releases) ( [latest](https://github.com/3F/MvsSln/releases/latest) )
* [Nightly builds](https://ci.appveyor.com/project/3Fs/mvssln/history) (`/artifacts` page). But remember: It can be unstable or not work at all. Use this for tests of latest changes.


## &

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=entry%2ereg%40gmail%2ecom&lc=US&item_name=3F%2dOpenSource%20%5b%20github%2ecom%2f3F&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)
