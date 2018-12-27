# [MvsSln](https://github.com/3F/MvsSln)

[![](https://raw.githubusercontent.com/3F/MvsSln/master/MvsSln/Resources/MvsSln_v1_96px.png)](https://github.com/3F/MvsSln)

MvsSln provides complex support (sln parser, r/w handlers, ...) of the Visual Studio .sln files and its projects (.vcxproj, .csproj., ...).

It was part of the [vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent) projects, but now it extracted into the new (specially for [DllExport](https://github.com/3F/DllExport) and for others).

[![Build status](https://ci.appveyor.com/api/projects/status/if1t4rhhntpf6ut3/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/mvssln/branch/master)
[![release-src](https://img.shields.io/github/release/3F/MvsSln.svg)](https://github.com/3F/MvsSln/releases/latest)
[![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/MvsSln/blob/master/License.txt)
[![NuGet package](https://img.shields.io/nuget/v/MvsSln.svg)](https://www.nuget.org/packages/MvsSln/)

**Download:** [/releases](https://github.com/3F/MvsSln/releases) [ **[latest](https://github.com/3F/MvsSln/releases/latest)** ]

[`gnt`](https://3f.github.io/GetNuTool/releases/latest/gnt/)` /p:ngpackages="MvsSln"` [[?](https://github.com/3F/GetNuTool)]

## License

The [MIT License (MIT)](https://github.com/3F/MvsSln/blob/master/License.txt)

```
Copyright (c) 2013-2018  Denis Kuzmin < entry.reg@gmail.com > :: github.com/3F
```

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif) ☕](https://3F.github.io/Donation/) 

## Why MvsSln ?

Because today it still is the most easy way for complex work with Visual Studio .sln files and its projects (.vcxproj, .csproj., ...). Because it's free, because it's open.

Even if you just need the basic access to information or more complex work through our readers and writers. 

* You can also easily control all your projects data (Reference, ProjectReference, Properties, Import sections, ...).
* Or even create your **custom sln parsing** of anything **in a few steps.**

Moreover, it has been re-licensed from vsSolutionBuildEvent projects (LGPLv3 -> MIT), so, enjoy with us.

```csharp
using(var sln = new Sln(@"D:\projects\Conari\Conari.sln", SlnItems.All &~ SlnItems.ProjectDependencies))
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

By the way, the any new solution handler (reader or writer) can be easily added by our flexible architecture. *See below.*

Control anything and have fun !

## Examples of using

### DllExport Manager

DllExport project finally changed distribution of the packages starting with v1.6 release. The final manager now fully works via MvsSln:

* https://github.com/3F/DllExport/wiki/DllExport-Manager

![](https://raw.githubusercontent.com/3F/MvsSln/master/resources/MvsSln_DllExport_example.png)


### Map of .sln & Writers

v2+ now also may provide map of analyzed data. To enable this, define a bit **0x0080** for type of operations to parser.

Parser will expose map through list of `ISection` for each line. For example:

![](https://raw.githubusercontent.com/3F/MvsSln/master/resources/MvsSln_v2.0_Map.png)

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

### How to modify .sln file

https://github.com/3F/MvsSln/issues/3

```csharp
// new collection from available projects but without project 'UnLib'
var projects = sln.Result.ProjectItems.Where(p => p.name != "UnLib");

// prepare write-handlers
var whandlers = new Dictionary<Type, HandlerValue>() {
    [typeof(LProject)] = new HandlerValue(new WProject(projects, sln.Result.ProjectDependencies)),
};

// save result
using(var w = new SlnWriter(@"modified.sln", whandlers)) {
    w.Write(sln.Result.Map);
}
// That's all. You should get 'modified.sln' without `UnLib` project.
```

## Did you know 

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

## Example of extending (your custom handlers)

Example of `LProject` handler (**reader**):

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

## How to get MvsSln

Available variants:

* [GetNuTool](https://github.com/3F/GetNuTool): `msbuild gnt.core /p:ngpackages="MvsSln"` or **[gnt](https://3f.github.io/GetNuTool/releases/latest/gnt/)** /p:ngpackages="MvsSln"
* NuGet PM: `Install-Package MvsSln`
* NuGet Commandline: `nuget install MvsSln`
* [GitHub Releases](https://github.com/3F/MvsSln/releases) [ [latest](https://github.com/3F/MvsSln/releases/latest) ]
* [Nightly builds](https://ci.appveyor.com/project/3Fs/mvssln/history) (`/artifacts` page). It can be unstable or not work at all. Use this for tests of latest changes.
  * Artifacts [older than 6 months](https://www.appveyor.com/docs/packaging-artifacts/#artifacts-retention-policy) you can also find as `Pre-release` with mark `🎲 Nightly build` on [GitHub Releases](https://github.com/3F/MvsSln/releases) page.

