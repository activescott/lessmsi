@echo on

REM note: Nodejs configured and semantic-release insalled in appveyor.yml
REM       This script is called /by/ semantic-release to publish to chocolatey

IF [%1]==[] (
    ECHO ERROR: Must supply build version as first parameter to this bat file! 1>&2
    EXIT 1
)
set _BUILD_VERSION=%1


choco push ..\lessmsi.%_BUILD_VERSION%.nupkg --api-key=%CHOCO_KEY%