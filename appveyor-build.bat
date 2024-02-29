@echo off

setlocal
    cd .tools
    call netfx4sdk -mode sys
endlocal

build %*