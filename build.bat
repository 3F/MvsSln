@echo off

call .tools\hMSBuild -GetNuTool & (
    if [%~1]==[#] exit /B 0
)

set "reltype=%~1" & if not defined reltype set reltype=Release
call packages\vsSolutionBuildEvent\cim.cmd /v:m /m:7 /p:Configuration=%reltype% || goto err
exit /B

:err
    echo Failed build>&2
exit /B 1