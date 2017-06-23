@echo off

set reltype=%1
set _msbuild=tools/hMSBuild
set cimdll=packages\vsSBE.CI.MSBuild\bin\CI.MSBuild.dll
set _gnt=tools/gnt

if "%reltype%"=="" (
    set reltype=Release
)

call %_gnt% /p:wpath="%cd%" /p:ngconfig="packages.config;MvsSlnTest\packages.config" /nologo /v:m /m:4 || goto err
call %_msbuild% -notamd64 "MvsSln.sln" /v:normal /l:"%cimdll%" /m:4 /t:Build /p:Configuration="%reltype%" || goto err

goto exit

:err

echo. Build failed. 1>&2
exit /B 1

:exit
exit /B 0
