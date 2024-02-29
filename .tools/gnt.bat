@echo off
:: GetNuTool /shell/batch edition
:: Copyright (c) 2015-2024  Denis Kuzmin <x-3F@outlook.com> github/3F
:: https://github.com/3F/GetNuTool
set ee=gnt.core&set ef="%temp%\%ee%1.9.0%random%%random%"&if "%~1"=="-unpack" goto en
if "%~1"=="-msbuild" goto eo
set eg=%*&setlocal enableDelayedExpansion&set "eh=%~1 "&set ei=!eh:~0,1!&if "!ei!" NEQ " " if !ei! NEQ / set eg=/p:ngpackages=!eg!
set "ej=%msb.gnt.cmd%"&if defined ej goto ep
set ek=hMSBuild&if exist msb.gnt.cmd set ek=msb.gnt.cmd
for /F "tokens=*" %%i in ('%ek% -only-path 2^>^&1 ^&call echo %%^^ERRORLEVEL%%') do 2>nul (if not defined ej (set ej="%%i")else set el=%%i)
if .%el%==.0 if exist !ej! goto ep
for %%v in (4.0,14.0,12.0,3.5,2.0)do (for /F "usebackq tokens=2* skip=2" %%a in (`reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\%%v" /v MSBuildToolsPath 2^> nul`) do if exist %%b (set ej="%%~b\MSBuild.exe"
if exist !ej! (if %%v NEQ 3.5 if %%v NEQ 2.0 goto ep
echo Override engine or contact for legacy support %%v&exit/B120)))&echo Engine is not found. Try with hMSBuild 1>&2
exit/B2
:eo
echo This feature is disabled in current version >&2
exit/B120
:ep
set em=/noconlog&if "%debug%"=="true" set em=/v:q
call :eq&call :er "/help" "-help" "/h" "-h" "/?" "-?"&call !ej! %ef% /nologo /noautorsp !em! /p:wpath="%cd%/" !eg!&set el=!ERRORLEVEL!&del /Q/F %ef%&exit/B!el!
:en
set ef="%cd%\%ee%"&echo Generating a %ee% at %cd%\...
:eq
setlocal disableDelayedExpansion
<nul set/P="">%ef%&set -=ngconfig&set [=Condition&set ]=packages.config&set ;=ngserver&set .=package&set ,=GetNuTool&set :=wpath&set +=TaskCoreDllPath&set {=Exists&set }=MSBuildToolsPath&set _=Microsoft.Build.Tasks.&set a=MSBuildToolsVersion&set b=Target&set c=tmode&set d=ParameterGroup&set e=Reference&set f=System&set g=Namespace&set h=Console.WriteLine(&set i=string&set j=return&set k=Console.Error.WriteLine(&set l=string.IsNullOrEmpty(&set m=foreach&set n=Attribute&set o=Append&set p=Path&set q=Combine&set r=Length&set s=false&set t=ToString&set u=SecurityProtocolType&set v=ServicePointManager.SecurityProtocol&set w=Credentials&set x=Directory&set y=CreateDirectory&set z=Console.Write(&set $=using&set #=FileMode&set @=FileAccess&set `=StringComparison&set ?=StartsWith
<nul set/P=^<?xml version="1.0" encoding="utf-8"?^>^<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003"^>^<PropertyGroup^>^<%-% %[%="'$(%-%)'==''"^>%]%;.tools\%]%^</%-%^>^<%;% %[%="'$(%;%)'==''"^>https://www.nuget.org/api/v2/%.%/^</%;%^>^<ngpath %[%="'$(ngpath)'==''"^>packages^</ngpath^>^<%,%^>1.9.0.58814+bb83b59^</%,%^>^<%:% %[%="'$(%:%)'==''"^>$(MSBuildProjectDirectory)^</%:%^>^<%+% %[%="%{%('$(%}%)\%_%v$(%a%).dll')"^>$(%}%)\%_%v$(%a%).dll^</%+%^>^<%+% %[%="'$(%+%)'=='' and %{%('$(%}%)\%_%Core.dll')"^>$(%}%)\%_%Core.dll^</%+%^>^</PropertyGroup^>^<%b% Name="get" BeforeTargets="Build"^>^<d %c%="get"/^>^</%b%^>^<%b% Name="grab"^>^<d %c%="grab"/^>^</%b%^>^<%b% Name="pack"^>^<d %c%="pack"/^>^</%b%^>^<UsingTask TaskName="d" TaskFactory="CodeTaskFactory" AssemblyFile="$(%+%)"^>^<%d%^>^<%c%/^>^</%d%^>^<Task^>^<%e% Include="%f%.Xml"/^>^<%e% Include="%f%.Xml.Linq"/^>^<%e% Include="WindowsBase"/^>^<Using %g%="%f%"/^>^<Using %g%="%f%.IO"/^>^<Using %g%="%f%.IO.Packaging"/^>^<Using %g%="%f%.Linq"/^>^<Using %g%="%f%.Net"/^>^<Using %g%="%f%.Xml.Linq"/^>^<Code Type="Fragment" Language="cs"^>^<![CDATA[if("$(logo)"!="no")%h%"\nGetNuTool $(%,%)\n(c) 2015-2024  Denis Kuzmin <x-3F@outlook.com> github/3F\n");var d="{0} is not found ";var e=new %i%[]{"/_rels/","/%.%/","/[Content_Types].xml"};Action^<%i%,object^>f=(g,h)=^>{if("$(debug)".Trim()=="true")%h%g,h);};Func^<%i%,XElement^>i=j=^>{try{%j% XDocument.Load(j).Root;}catch(Exception k){%k%k.Message);throw;}};Func^<%i%,%i%[]^>l=m=^>m.Split(new[]{m.Contains('^|')?'^|':';'},(StringSplitOptions)1);if(%c%=="get"^|^|%c%=="grab"){var n=@"$(ngpackages)";var o=new StringBuilder();if(%l%n)){Action^<%i%^>p=q=^>{%m%(var r in i(q).Descendants("%.%")){var s=r.%n%("id");var t=r.%n%("version");var u=r.%n%("output");var v=r.%n%("sha1");if(s==null){%k%"{0} is corrupted",q);%j%;}o.%o%(s.Value);if(t!=null)o.%o%("/"+t.Value);if(v!=null)o.%o%("?"+v.Value);if(u!=null)o.%o%(":">>%ef%
<nul set/P=+u.Value);o.%o%(';');}};%m%(var q in l(@"$(%-%)")){var w=%p%.%q%(@"$(%:%)",q);if(File.%{%(w)){p(w);}else f(d,w);}if(o.%r%^<1){%k%"Empty .config + ngpackages");%j% %s%;}n=o.%t%();}var x=@"$(ngpath)";var y=@"$(proxycfg)";%m%(var z in Enum.GetValues(typeof(%u%)).Cast^<%u%^>()){try{%v%^|=z;}catch(NotSupportedException){}}if("$(ssl3)"!="true")%v%^&=~(%u%)(48^|192^|768);Func^<%i%,WebProxy^>D=q=^>{var E=q.Split('@');if(E.%r%^<=1)%j% new WebProxy(E[0],%s%);var F=E[0].Split(':');%j% new WebProxy(E[1],%s%){%w%=new NetworkCredential(F[0],F.%r%^>1?F[1]:null)};};Func^<%i%,%i%^>G=H=^>{var I=%p%.GetDirectoryName(H);if(!%x%.%{%(I))%x%.%y%(I);%j% H;};Func^<%i%,%i%,%i%,%i%,bool^>J=(K,L,H,v)=^>{var M=%p%.GetFullPath(%p%.%q%(@"$(%:%)",H??L??""));if(%x%.%{%(M)^|^|File.%{%(M)){%h%"{0} use {1}",L,M);%j% true;}%z%K+" ... ");var N=%c%=="grab";var O=N?G(M):%p%.%q%(%p%.GetTempPath(),Guid.NewGuid().%t%());%$%(var P=new WebClient()){try{if(!%l%y)){P.Proxy=D(y);}P.Headers.Add("User-Agent","%,%/$(%,%)");P.UseDefaultCredentials=true;if(P.Proxy!=null^&^&P.Proxy.%w%==null){P.Proxy.%w%=CredentialCache.DefaultCredentials;}P.DownloadFile(@"$(%;%)"+K,O);}catch(Exception k){%k%k.Message);%j% %s%;}}%h%M);if(v!=null){%z%"{0} ... ",v);%$%(var Q=%f%.Security.Cryptography.SHA1.Create()){o.Clear();%$%(var R=new FileStream(O,(%#%)3,(%@%)1))%m%(var S in Q.ComputeHash(R))o.%o%(S.%t%("x2"));%z%o.%t%());if(!o.%t%().Equals(v,(%`%)5)){%h%"[x]");%j% %s%;}%h%);}}if(N)%j% true;%$%(var r=ZipPackage.Open(O,(%#%)3,(%@%)1)){%m%(var T in r.GetParts()){var U=Uri.UnescapeDataString(T.Uri.OriginalString);if(e.Any(V=^>U.%?%(V,(%`%)4)))continue;var W=%p%.%q%(M,U.TrimStart('/'));f("- {0}",U);%$%(var X=T.GetStream((%#%)3,(%@%)1))%$%(var Y=File.OpenWrite(G(W))){try{X.CopyTo(Y);}catch(FileFormatException){f("[x]?crc: {0}",W);}}}}File.Delete(O);%j% true;};%m%(var r in l(n)){var Z=r.Split(new[]{':'},2);var K=Z[0].Split(new[]{'?'},2);var H=Z.%r%^>1?Z[1]:null;var L=K[0].Replace(>>%ef%
<nul set/P='/','.');if(!%l%x)){H=%p%.%q%(x,H??L);}if(!J(K[0],L,H,K.%r%^>1?K[1]:null)^&^&"$(break)".Trim()!="no")%j% %s%;}}else if(%c%=="pack"){var a=".nuspec";var b="metadata";var c="id";var A="version";var I=%p%.%q%(@"$(%:%)",@"$(ngin)");if(!%x%.%{%(I)){%k%d,I);%j% %s%;}var B=%x%.GetFiles(I,"*"+a).FirstOrDefault();if(B==null){%k%d+I,a);%j% %s%;}%h%"{0} use {1}",a,B);var C=i(B).Elements().FirstOrDefault(V=^>V.Name.LocalName==b);if(C==null){%k%d,b);%j% %s%;}var _=new %f%.Collections.Generic.Dictionary^<%i%,%i%^>();Func^<%i%,%i%^>dd=de=^>_.ContainsKey(de)?_[de]:"";%m%(var df in C.Elements())_[df.Name.LocalName.ToLower()]=df.Value;if(dd(c).%r%^>100^|^|!%f%.Text.RegularExpressions.Regex.IsMatch(dd(c),@"^\w+(?:[_.-]\w+)*$")){%k%"Invalid id");%j% %s%;}var dg=%i%.Format("{0}.{1}.nupkg",dd(c),dd(A));var dh=%p%.%q%(@"$(%:%)",@"$(ngout)");if(!%i%.IsNullOrWhiteSpace(dh)){if(!%x%.%{%(dh)){%x%.%y%(dh);}dg=%p%.%q%(dh,dg);}%h%"Creating %.% {0} ...",dg);%$%(var r=Package.Open(dg,(%#%)2)){var di=new Uri(%i%.Format("/{0}{1}",dd(c),a),(UriKind)2);r.CreateRelationship(di,0,"http://schemas.microsoft.com/packaging/2010/07/manifest");%m%(var dj in %x%.GetFiles(I,"*.*",(SearchOption)1)){if(e.Any(V=^>dj.%?%(%p%.%q%(I,V.Trim('/')),(%`%)4)))continue;var dk=dj.%?%(I,(%`%)5)?dj.Substring(I.%r%).TrimStart(%p%.DirectorySeparatorChar):dj;f("+ {0}",dk);var T=r.CreatePart(PackUriHelper.CreatePartUri(new Uri(%i%.Join("/",dk.Split('\\','/').Select(Uri.EscapeDataString)),(UriKind)2)),"application/octet",(CompressionOption)1);%$%(var dl=T.GetStream())%$%(var dm=new FileStream(dj,(%#%)3,(%@%)1)){dm.CopyTo(dl);}}var dn=r.PackageProperties;dn.Creator=dd("authors");dn.Description=dd("description");dn.Identifier=dd(c);dn.Version=dd(A);dn.Keywords=dd("tags");dn.Title=dd("title");dn.LastModifiedBy="%,%/$(%,%)";}}else %j% %s%;]]^>^</Code^>^</Task^>^</UsingTask^>^<%b% Name="Build" DependsOnTargets="%,%"/^>^</Project^>>>%ef%
endlocal&exit/B0
:er
if defined eg set eg=!eg:%~1=!
if "%~2" NEQ "" shift & goto er
exit/B0