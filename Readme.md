[![](https://raw.githubusercontent.com/3F/MvsSln/master/MvsSln/Resources/MvsSln_v1_96px.png)](https://github.com/3F/MvsSln) [**MvsSln**](https://github.com/3F/MvsSln)

Customizable VisualStudio .sln parser with project support (.vcxproj, .csproj., …). Pluggable lightweight r/w handlers at runtime, and more …

```r
Copyright (c) 2013-2024  Denis Kuzmin <x-3F@outlook.com> github/3F
```

[ 「 ❤ 」 ](https://3F.github.io/fund) [![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/MvsSln/blob/master/License.txt)

[*MvsSln*](https://github.com/3F/MvsSln) is waiting for your awesome contributions! https://github.com/3F/MvsSln/graphs/contributors

| Download    | Windows | Linux
|-------------|---------|--------
| [![NuGet](https://img.shields.io/nuget/v/MvsSln.svg)](https://www.nuget.org/packages/MvsSln/) <br/> [`gnt MvsSln`](https://github.com/3F/GetNuTool#getnutool) | [![status](https://ci.appveyor.com/api/projects/status/6uunsds889rhkpo2/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/mvssln-fxjnf/branch/master) | [![status](https://ci.appveyor.com/api/projects/status/vdt3taxswrxo37tt/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/mvssln-2d2c2/branch/master)
| [![release](https://img.shields.io/github/release/3F/MvsSln.svg)](https://github.com/3F/MvsSln/releases/latest) | [![Tests](https://img.shields.io/appveyor/tests/3Fs/mvssln-fxjnf/master.svg)](https://ci.appveyor.com/project/3Fs/mvssln-fxjnf/build/tests) | [![Tests](https://img.shields.io/appveyor/tests/3Fs/mvssln-2d2c2/master.svg)](https://ci.appveyor.com/project/3Fs/mvssln-2d2c2/build/tests)

## Why MvsSln

MvsSln provides the easiest way to complex work with Visual Studio .sln files and referenced projects (.vcxproj, .csproj., ...). Merge, Manage, Attach custom handlers and more. Because it's free, because it's open.

🌌 The most convenient work with projects, dependencies, their lazy loading, any folders, any items, references and much more in these different worlds;

💡 We are customizable and extensible library at runtime! Make **your custom** .sln and its parsing for everything you like at the moment you need just in a few steps;

🚀 We were born from other popular project to be more loyal for your preferences on the fly. Hello from 2013;

Even if you just need the basic access to information or more complex work through our readers and writers.

Create/modify/or just use parsed folders, projects, and other. 

Safely compare anything, 

```csharp
if(new ProjectItem(...) == new ProjectItem(...)) { ... }
if(new SolutionFolder(...) == new SolutionFolder(...)) { ... }
if(new ConfigItem(...) == new ConfigItem(...)) { ... }
if(new PackageInfo(...) == new PackageInfo(...)) { ... }
...
````


Use 📂 Subdirectories,

```csharp
new SolutionFolder("dir1", 
    new SolutionFolder("dir2", 
        new SolutionFolder("dir3", "hMSBuild.bat", "DllExport.bat")
    )
);
...
new SolutionFolder("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "MyFolder2", dir1, ".gnt\\gnt.core");
...
```

Projects and Folders,

```csharp
new ProjectItem("Project1", ProjectType.Cs);
new ProjectItem("Project1", ProjectType.Cs, new SolutionFolder("dir1"));
new ProjectItem("Project2", ProjectType.Vc, "path 1");
new ProjectItem("{EE7DD6B7-56F4-478D-8745-3D204D915473}", "Project1", ProjectType.Cs, dir2);
...
```

Detect the real\* project types,

```csharp
* IsCs() - Checking both legacy `ProjectType.Cs` and modern `ProjectType.CsSdk` types.
. . .
* IsSdk() - While ProjectType cannot inform the actual use of the modern Sdk style in projects,
            current method will try to detect this by using the extended logic:
            https://github.com/dotnet/project-system/blob/master/docs/opening-with-new-project-system.md
```

Load only what is needed at the moment,

```csharp
// https://github.com/3F/MvsSln/discussions/49

using var sln = new Sln("Input.sln", SlnItems.Env);

sln.Result.Env
    .LoadProjects(sln.Result.ProjectItemsConfigs.Where(p => p.project.IsCs()))
    .ForEach(xp =>
    {
        xp.AddItem("Compile", @"financial\Invoice.cs");
    });
```

Modify *.sln* at runtime,

https://github.com/3F/MvsSln/discussions/43#discussioncomment-371185

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
```

Manage [packages.config](https://github.com/3F/MvsSln/pull/30),

```csharp
// 2.6+
using Sln l = new("Input.sln", SlnItems.AllNoLoad | SlnItems.PackagesConfig);

IPackageInfo found = l.Result.PackagesConfigs
                                .SelectMany(s => s.Packages)
                                .FirstOrDefault(p => p.Id.StartsWith("Microsoft."));
// found.MetaTFM ...

Version v = l.Result.PackagesConfigs.First().GetPackage("LX4Cnh")?.VersionParsed;
```

Easily create files [from scratch](https://github.com/3F/MvsSln/wiki/Creating-from-scratch),

```csharp
// 2.7+
LhDataHelper hdata = new();
hdata.SetHeader(SlnHeader.MakeDefault())
        .SetProjects(projects)
        .SetProjectConfigs(prjConfs)
        .SetSolutionConfigs(slnConf);

using(SlnWriter w = new(solutionFile, hdata))
{
    w.Options = SlnWriterOptions.CreateProjectsIfNotExist;
    w.Write();
}

using Sln sln = new(solutionFile, SlnItems.EnvWithMinimalProjects);
IXProject xp = sln.Result.Env.Projects.First();

xp.SetProperties(new Dictionary<string, string>()
{
    { "OutputType", "EXE" },
    { "TargetFramework", "net8.0" },
    { "Platforms", "x64" }
});
xp.Save();
```

Everything at hand,

```csharp
using(var sln = new Sln(@"D:\projects\Conari\Conari.sln", SlnItems.All & ~SlnItems.ProjectDependencies))
{
    //sln.Result.Env.XProjectByGuid(
    //    sln.Result.ProjectDependencies.FirstBy(BuildType.Rebuild).pGuid,
    //    new ConfigItem("Debug", "Any CPU")
    //);

    var p = sln.Result.Env.GetOrLoadProject(
        sln.ProjectItems.FirstOrDefault(p => p.name == name)
    );

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
        xp.SetProperties
        (
            new[]
            {
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

And something more,

```csharp
// https://github.com/3F/MvsSln/discussions/42

using var sln = new Sln("TestStruct.sln", SlnItems.Env);
//...
sln.Result.Env.Projects.ForEach(xp => 
    xp.Project.Xml.PropertyGroups.Where(p => p.Condition.Contains("'$(Configuration)|$(Platform)'"))
    .Where(p =>
        sln.Result.ProjectItemsConfigs.All(s => 
            !p.Condition.Contains($"'{s.projectConfig.ConfigurationByRule}|{s.projectConfig.PlatformByRule}'")
        )
    ).ForEach(p => p.Parent?.RemoveChild(p))
);
```

The any new solution handler (reader or writer) can be easily added because of our flexible architecture.

Control anything and have fun !

> MvsSln was specially extracted and re-licensed from *vsSolutionBuildEvent* projects (GPL -> MIT) for https://github.com/3F/DllExport and others! Join us 🎈

## High quality Project Icons. Visual Studio

Since Microsoft officially distributes [5,000 high quality free icons and bitmaps](https://twitter.com/GitHub3F/status/1219348325729816578) from products like Visual Studio,

You can also use related project icons together with MvsSln like it was already for .NET DllExport project:

![](https://github.com/3F/MvsSln/blob/master/resources/DllExport_1.7.png?raw=true)

Follow License Terms for icons and Find implementation in original repo: [https://github.com/3F/DllExport](https://github.com/3F/DllExport)

## How or Where is used

Let's consider examples of use in real projects below.

### .NET DllExport

DllExport project finally changed distribution of the packages starting with v1.6 release. The final manager now fully works via MvsSln:

* https://github.com/3F/DllExport/wiki/DllExport-Manager

![](https://raw.githubusercontent.com/3F/MvsSln/master/resources/MvsSln_DllExport_example.png)

MvsSln also is **a core logic** in *Post-Processing* feature [[?]](https://github.com/3F/DllExport/pull/148)

![](https://github.com/3F/MvsSln/blob/master/resources/MvsSln_and_DllExport_PostProc.png?raw=true)

### vsSolutionBuildEvent

vsSolutionBuildEvent now is completely integrated with MvsSln [[?](https://github.com/3F/vsSolutionBuildEvent/pull/53)]

Fully removed original parser and replaced related processing from Environment/IsolatedEnv/MSBuild/CIM. Now it just calls the corresponding modern methods from MvsSln.

https://github.com/3F/vsSolutionBuildEvent

![](https://github.com/3F/MvsSln/blob/master/resources/vsSBE_and_MvsSln.png?raw=true)

![](https://github.com/3F/MvsSln/blob/master/resources/vsSBE_and_MvsSln_VS.png?raw=true)


## Map & handlers

2.0+ can optionally provide a [map of the analyzed data](https://github.com/3F/MvsSln/wiki/SlnWriter#about-skeleton-and-map). To enable this, define a [0x0080 bit](https://github.com/3F/MvsSln/blob/a00e3b341bbddc559a5af618e4a6e520b7bbb2d6/MvsSln/SlnItems.cs#L92).

MvsSln's parser will fill the map using handlers that processed line and/or raw access through wrapper, like:

[![](https://raw.githubusercontent.com/3F/MvsSln/master/resources/MvsSln_v2.0_Map.png)](https://github.com/3F/MvsSln/wiki/SlnWriter#about-skeleton-and-map)

The map may be used for any modifications or creating a new solution or project files through other handlers etc. 

Read more about map, skeleton, and *SlnWriter* [**here**](https://github.com/3F/MvsSln/wiki/SlnWriter)

## Projects. Adding References

```csharp
XProject.AddPackageReference("Conari", "1.5.0");
```

```csharp
xp.AddProjectReference(projects.First());
xp.AddProjectReference(new ProjectItem(ProjectType.Cs, @$"{projName}\src.csproj"));
```

```csharp
xp.AddReference(Assembly.GetExecutingAssembly());
```

```csharp
XProject.AddReference("DllExport", lib, AddReferenceOptions.MakeRelativePath);
```

```csharp
xp.AddReference(
    pathToDll,
    AddReferenceOptions.DefaultResolve | AddReferenceOptions.OmitVersion | AddReferenceOptions.HidePrivate
);
```

```xml
<Reference Include="MvsSln, PublicKeyToken=4bbd2ef743db151e" />
```

```xml
<Reference Include="DllExport, Version=1.6.4.15293, Culture=neutral, PublicKeyToken=8337224c9ad9e356">
  <HintPath>..\packages\DllExport.1.6.4\gcache\metalib\DllExport.dll</HintPath>
  <Private>False</Private>
</Reference>
```

## Example of extending (your custom handlers)

Example of `LProject` handler (**reader**):

```csharp
using static net.r_eg.MvsSln.Core.Keywords;

public class LProject: LAbstract, ISlnHandler
{
    public override ICollection<Type> CoHandlers { get; protected set; }
        = [typeof(LProjectDependencies)];

    public override bool IsActivated(ISvc svc)
        => (svc.Sln.ResultType & SlnItems.Projects) == SlnItems.Projects;

    public override bool Condition(RawText line)
        => line.trimmed.StartsWith(Project_, StringComparison.Ordinal);

    public override bool Positioned(ISvc svc, RawText line)
    {
        ProjectItem pItem = GetProjectItem(line.trimmed, svc.Sln.SolutionDir);
        if(pItem.pGuid == null) return false;
        if(svc.Sln.ProjectItemList == null) svc.Sln.ProjectItemList = [];

        svc.Sln.ProjectItemList.Add(pItem);
        return true;
    }
}
```

Example of `WSolutionConfigurationPlatforms` handler (**writer**):

```csharp
using static net.r_eg.MvsSln.Core.Keywords;

public class WSolutionConfigurationPlatforms(IEnumerable<IConfPlatform> configs)
    : WAbstract, IObjHandler
{
    protected IEnumerable<IConfPlatform> configs = configs;

    public override string Extract(object data)
    {
        if(configs == null) return null;

        lbuilder.Clear();
        lbuilder.AppendLv1Line(SolutionConfigurationPlatformsPreSolution);

        configs.ForEach(cfg => lbuilder.AppendLv2Line($"{cfg} = {cfg}"));

        return lbuilder.AppendLv1(EndGlobalSection).ToString();
    }
}
```

## Build MvsSln from source

```bat
git clone https://github.com/3F/MvsSln.git MvsSln
cd MvsSln
```

### Windows. Visual Studio / MSBuild

```bat
.\build Release
```
or together with configured [netfx4sdk](https://github.com/3F/netfx4sdk)

```bat
.\build-CI Release
```

### Ubuntu. vscode / dotnet

```sh
dotnet build -c Release
```

### run unit tests

```sh
dotnet test -c Release --no-build --no-restore
```
