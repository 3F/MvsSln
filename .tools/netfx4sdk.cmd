@echo off
:: netfx4sdk 1.1.0.56188+a5bc965
:: Copyright (c) 2021-2023  Denis Kuzmin {x-3F@outlook.com} github/3F
:: Copyright (c) netfx4sdk contributors https://github.com/3F/netfx4sdk
set "aa=%~dp0"
set ab=%*
set /a ac=0
setlocal enableDelayedExpansion
if not defined ab goto a8
set ad=!ab:/?=-h!
call :a9 a4 ad a5
goto a_
:a8
echo.
@echo netfx4sdk 1.1.0.56188+a5bc965
@echo Copyright (c) 2021-2023  Denis Kuzmin ^<x-3F@outlook.com^> github/3F
@echo Copyright (c) netfx4sdk contributors https://github.com/3F/netfx4sdk
echo.
echo .........
echo Arguments
echo.
echo  -mode {value}
echo   * system   - (Recommended) Hack using assemblies for windows.
echo   * package  - Apply obsolete remote package. Read [About modes] below.
echo   * sys      - Alias to `system`
echo   * pkg      - Alias to `package`
echo.
echo  -force    - Aggressive behavior when applying etc.
echo  -rollback - Rollback applied modifications.
echo  -global   - To use the global toolset, like hMSBuild.
echo.
echo  -pkg-version {arg} - Specific package version. Where {arg}:
echo      * 1.0.2 ...
echo      * latest - (keyword) To use latest version;
echo.
echo  -debug    - To show debug information.
echo  -version  - Display version of %~nx0.
echo  -help     - Display this help. Aliases: -help -h -?
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
echo netfx4sdk -mode pkg -pkg-version 1.0.2
goto a7
:a_
set "ae=v4.0"
set "af=1.0.3"
set "ag="
set "ah="
set "ai="
set "aj="
set "ak="
set /a al=0
:ba
set am=!a4[%al%]!
if [!am!]==[-help] ( goto a8 ) else if [!am!]==[-h] ( goto a8 ) else if [!am!]==[-?] ( goto a8 )
if [!am!]==[-debug] (
set ag=1
goto bb
) else if [!am!]==[-mode] ( set /a "al+=1" & call :bc a4[!al!] v
if not "!v!"=="sys" if not "!v!"=="system" if not "!v!"=="pkg" if not "!v!"=="package" goto bd
if "!v!"=="system" ( set "ah=sys" ) else if "!v!"=="package" ( set "ah=pkg" ) else set "ah=!v!"
goto bb
) else if [!am!]==[-rollback] (
set ai=1
goto bb
) else if [!am!]==[-pkg-version] ( set /a "al+=1" & call :bc a4[!al!] v
set af=!v!
call :be "set package version:" v
goto bb
) else if [!am!]==[-version] (
@echo 1.1.0.56188+a5bc965
goto a7
) else if [!am!]==[-global] (
set ak=1
goto bb
) else if [!am!]==[-force] (
set aj=1
goto bb
) else (
:bd
call :bf "Incorrect key or value for `!am!`"
set /a ac=1
goto a7
)
:bb
set /a "al+=1" & if %al% LSS !a5! goto ba
:bg
call :be "run action... " ah aj
set an=%ProgramFiles(x86)%
if not exist "%an%" set an=%ProgramFiles%
set an=%an%\Reference Assemblies\Microsoft\Framework\.NETFramework\
set ao=%an%%ae%
set ap=%ao%.%~nx0
if defined ai (
if not exist "%ap%" (
echo There's nothing to rollback.
goto a7
)
rmdir /Q/S "%ao%" 2>nul
call :be "ren " ap ae
( ren "%ap%" %ae% 2>nul ) || ( set /a ac=1100 & goto a7 )
echo Rollback completed.
goto a7
)
if exist "%ap%" (
echo %~nx0 has already been applied before. There's nothing to do anymore.
echo Use `-rollback` key to re-apply with another mode if needed.
exit/B 0
)
if exist "%ao%\mscorlib.dll" (
if not defined aj (
echo The Developer Pack was found successfully. There's nothing to do here at all.
echo Use `-force` key to suppress the restriction if you really know what you are doing.
set /a ac=0 & goto a7
)
call :be "Suppress found SDK " ao
)
if not defined ah ( set /a ac=1000 & goto a7 )
if "%ah%"=="sys" (
echo Apply hack using assemblies for windows ...
set "aq="
for /F "tokens=*" %%i in ('hMSBuild -no-vswhere -no-vs -only-path -notamd64 2^>^&1 ^& call echo %%^^ERRORLEVEL%%') do (
if not defined aq ( set aq=%%i ) else set /a ac=%%i
)
if not !ac! == 0 goto a7
call :bh "%ao%" "%ap%" || a6 a7
set aq=!aq:msbuild.exe=!
call :be "lDir " aq
if not exist "%aq%" ( set /a ac=3 & goto a7 )
mkdir "%ao%" 2>nul
for /F "tokens=*" %%i in ('dir /B "!aq!*.dll"') do mklink "%ao%\%%i" "!aq!%%i" >nul 2>nul
for /F "tokens=*" %%i in ('dir /B "!aq!WPF\*.dll"') do mklink "%ao%\%%i" "!aq!WPF\%%i" >nul 2>nul
set "ar=%ao%\RedistList" & mkdir "!ar!" 2>nul
echo ^<?xml version="1.0" encoding="utf-8"?^>^<FileList Redist="Microsoft-Windows-CLRCoreComp.4.0" Name=".NET Framework 4" RuntimeVersion="4.0" ToolsVersion="4.0" /^>> "!ar!\FrameworkList.xml"
set "ar=%ao%\PermissionSets" & mkdir "!ar!" 2>nul
echo ^<PermissionSet as="1" class="System.Security.PermissionSet" Unrestricted="true" /^>> "!ar!\FullTrust.xml"
) else if "%ah%"=="pkg" (
set at=Microsoft.NETFramework.ReferenceAssemblies.net40
echo Apply `!at!` package ...
set au=%~nx0.%af%
if "%af%"=="latest" ( set "af=" ) else set af=/%af%
if defined ak ( set "av=hMSBuild" ) else set av="%~dp0hMSBuild"
if defined ag set av=!av! -debug
call !av! -GetNuTool /p:ngpackages="!at!!af!:!au!"
set "aw=packages\!au!\build\.NETFramework\%ae%"
call :be "dpkg " aw
if not exist "!aw!" (
set /a ac=1001 & goto a7
)
ren "%ao%" %ae%.%~nx0 2>nul
mklink /J "%ao%" "!aw!"
)
echo Done.
set /a ac=0
goto a7
:a7
if not !ac!==0 call :bf "Failed: !ac!"
exit/B !ac!
:bh
set "ax=%~1" & set "ay=%~2"
call :be "xcp " ax ay
set az=xcopy "%ax%" "%ay%" /E/I/Q/H/K/O/X
%az%/B 2>nul>nul || %az% >nul || exit/B 1001
exit/B 0
:bf
echo   [*] WARN: %~1 >&2
exit/B 0
:be
if defined ag (
set a0=%1
set a0=!a0:~0,-1!
set a0=!a0:~1!
echo.[%TIME% ] !a0! !%2! !%3!
)
exit/B 0
:a9
set a1=!%2!
:bi
for /F "tokens=1* delims==" %%a in ("!a1!") do (
if "%%~b"=="" (
call :bj %1 !a1! %3
exit/B 0
)
set a1=%%a #__b_EQ## %%b
)
goto bi
:bj
set "a2=%~1"
set /a al=-1
:bk
set /a al+=1
set %a2%[!al!]=%~2
set %a2%{!al!}=%2
shift & if not "%~3"=="" goto bk
set /a al-=1
set %1=!al!
exit/B 0
:bc
set a3=!%1!
set %2=!a3!
exit/B 0
:: :eval