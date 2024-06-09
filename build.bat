@echo off

call .tools\hMSBuild ~x -GetNuTool & if [%~1]==[#] exit /B 0

set "reltype=%~1" & if not defined reltype set reltype=Release
packages\vsSolutionBuildEvent\cim.cmd ~x ~c %reltype%