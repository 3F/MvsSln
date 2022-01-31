@echo off
:: netfx4sdk 1.0.0.3166+a9b5fd9
:: Copyright (c) 2021  Denis Kuzmin [ x-3F@outlook.com ] github/3F
:: Copyright (c) netfx4sdk contributors https://github.com/3F/netfx4sdk
set "aa=%~dp0"
set ab=%*
set /a ac=0
setlocal enableDelayedExpansion
if not defined ab goto a2
set ad=!ab:/?=-h!
call :a3 a0 ad a1
goto a4
:a2
echo.
@echo netfx4sdk 1.0.0.3166+a9b5fd9
@echo Copyright (c) 2021  Denis Kuzmin [ x-3F@outlook.com ] github/3F
@echo Copyright (c) netfx4sdk contributors https://github.com/3F/netfx4sdk
echo.
echo .........
echo Arguments
echo.
echo  -mode {value}
echo    * sys       - (Recommended) Hack using assemblies for windows.
echo    * package   - Apply obsolete remote package. Read [About modes] below.
echo.
echo  -force        - Aggressive behavior when applying etc.
echo  -rollback     - Rollback applied modifications.
echo  -debug        - To show debug information.
echo  -version      - Display version of %~nx0.
echo  -help         - Display this help. Aliases: -help -h -?
echo.
echo ...........
echo About modes
echo.
echo `-mode sys` highly recommended because of
echo  [++] All modules are under windows support.
echo  [+] It does not require internet connection (portable).
echo  [+] No decompression required (faster) compared to package mode.
echo  [-] This is behavior-based hack;
echo      Report or please fix us if something:
echo      https://github.com/3F/netfx4sdk
echo.
echo `-mode package` will try to apply obsolete package to the environment.
echo  [-] Officially dropped support since VS2022.
echo  [-] Requires internet connection to receive ~30 MB via GetNuTool.
echo  [-] Requires decompression of received data to 178 MB before use.
echo  [+] Well known official behavior.
echo.
echo .......
echo Samples
echo.
echo netfx4sdk -mode sys
echo netfx4sdk -rollback
echo netfx4sdk -debug -force -mode package
goto a5
:a4
set "ae=v4.0"
set "af=1.0.2"
set "ag="
set "ah="
set "ai="
set "aj="
set /a ak=0
:a6
set al=!a0[%ak%]!
if [!al!]==[-help] ( goto a2 ) else if [!al!]==[-h] ( goto a2 ) else if [!al!]==[-?] ( goto a2 )
if [!al!]==[-debug] (
set ag=1
goto a7
) else if [!al!]==[-mode] ( set /a "ak+=1" & call :a8 a0[!ak!] v
if not "!v!"=="sys" if not "!v!"=="package" goto a9
set ah=!v!
goto a7
) else if [!al!]==[-rollback] (
set ai=1
goto a7
) else if [!al!]==[-version] (
@echo 1.0.0.3166+a9b5fd9
goto a5
) else if [!al!]==[-force] (
set aj=1
goto a7
) else (
:a9
call :a_ "Incorrect key or value for `!al!`"
set /a ac=1
goto a5
)
:a7
set /a "ak+=1" & if %ak% LSS !a1! goto a6
:ba
call :bb "run action... " ah aj
set am=%ProgramFiles(x86)%
if not exist "%am%" set am=%ProgramFiles%
set am=%am%\Reference Assemblies\Microsoft\Framework\.NETFramework\
set an=%am%%ae%
set ao=%an%.%~nx0
if defined ai (
if not exist "%ao%" (
echo There's nothing to rollback.
goto a5
)
rmdir /Q/S "%an%" 2>nul
call :bb "ren " ao ae
( ren "%ao%" %ae% 2>nul ) || ( set /a ac=1100 & goto a5 )
echo Rollback completed.
goto a5
)
if exist "%ao%" (
echo %~nx0 has already been applied before. There's nothing to do anymore.
echo Use `-rollback` key to re-apply with another mode if needed.
exit/B 0
)
if exist "%an%\mscorlib.dll" (
if not defined aj (
echo The Developer Pack was found successfully. There's nothing to do here at all.
echo Use `-force` key to suppress the restriction if you really know what you are doing.
set /a ac=0 & goto a5
)
call :bb "Suppress found SDK " an
)
if not defined ah ( set /a ac=1000 & goto a5 )
if "%ah%"=="sys" (
echo Apply hack using assemblies for windows ...
call :bc "%an%" "%ao%"
for /F "tokens=*" %%i in ('hMSBuild -no-vswhere -no-vs -only-path -notamd64') do set ap=%%i
set ap=!ap:msbuild.exe=!
call :bb "lDir " ap
mkdir "%an%" 2>nul
for /F "tokens=*" %%i in ('dir /B "!ap!*.dll"') do mklink "%an%\%%i" "!ap!%%i" >nul 2>nul
for /F "tokens=*" %%i in ('dir /B "!ap!WPF\*.dll"') do mklink "%an%\%%i" "!ap!WPF\%%i" >nul 2>nul
set "aq=%an%\RedistList" & mkdir "!aq!" 2>nul
echo ^<?xml version="1.0" encoding="utf-8"?^>^<FileList Redist="Microsoft-Windows-CLRCoreComp.4.0" Name=".NET Framework 4" RuntimeVersion="4.0" ToolsVersion="4.0" /^>> "!aq!\FrameworkList.xml"
set "aq=%an%\PermissionSets" & mkdir "!aq!" 2>nul
echo ^<PermissionSet ar="1" class="System.Security.PermissionSet" Unrestricted="true" /^>> "!aq!\FullTrust.xml"
) else if "%ah%"=="package" (
set as=Microsoft.NETFramework.ReferenceAssemblies.net40
echo Apply `!as!` package ...
call .\hMSBuild -GetNuTool /p:ngpackages="!as!/%af%:%~nx0"
set "at=packages\%~nx0\build\.NETFramework\%ae%"
call :bb "dpkg " at
if not exist "!at!" (
set /a ac=1001 & goto a5
)
ren "%an%" %ae%.%~nx0 2>nul
mklink /J "%an%" "!at!"
)
echo Done.
set /a ac=0
goto a5
:a5
if not !ac!==0 call :a_ "Failed: !ac!"
exit/B !ac!
:bc
set "au=%~1" & set "av=%~2"
call :bb "xcp " au av
xcopy /E/I/Q/H/K/O/X/B "%au%" "%av%" >nul || ( set /a ac=1001 & goto a5 )
exit/B 0
:a_
echo   [*] WARN: %~1
exit/B 0
:bb
if defined ag (
set aw=%1
set aw=!aw:~0,-1!
set aw=!aw:~1!
echo.[%TIME% ] !aw! !%2! !%3!
)
exit/B 0
:a3
set ax=!%2!
:bd
for /F "tokens=1* delims==" %%a in ("!ax!") do (
if "%%~b"=="" (
call :be %1 !ax! %3
exit/B 0
)
set ax=%%a #__b_EQ## %%b
)
goto bd
:be
set "ay=%~1"
set /a ak=-1
:bf
set /a ak+=1
set %ay%[!ak!]=%~2
set %ay%{!ak!}=%2
shift & if not "%~3"=="" goto bf
set /a ak-=1
set %1=!ak!
exit/B 0
:a8
set az=!%1!
set %2=!az!
exit/B 0
:: :eval