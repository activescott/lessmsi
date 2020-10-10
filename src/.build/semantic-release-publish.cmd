@echo off

REM note: Nodejs configured and semantic-release insalled in appveyor.yml
REM       This script is called /by/ semantic-release to publish to chocolatey

IF [%1]==[] (
    ECHO ERROR: Must supply build version as first parameter to this bat file! 1>&2
    EXIT 1
)
set _BUILD_VERSION=%1

ECHO Running choco push...

choco push ..\lessmsi.%_BUILD_VERSION%.nupkg --api-key=%CHOCO_KEY%

ECHO Running choco push complete.