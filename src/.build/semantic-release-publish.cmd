@echo off

REM note: Nodejs configured and semantic-release insalled in appveyor.yml
REM       This script is called /by/ semantic-release to publish to chocolatey

set THIS_DIR=%~dp0

IF [%1]==[] (
    ECHO ERROR: Must supply build version as first parameter to this bat file! 1>&2
    EXIT 1
)
set _BUILD_VERSION=%1

ECHO Running choco push...

choco push --source https://push.chocolatey.org/ --api-key=%CHOCO_KEY% "%THIS_DIR%..\.deploy\chocolateypackage\lessmsi.%_BUILD_VERSION%.nupkg"

REM NOTE: ECHO does not clear/set errorlevel https://ss64.com/nt/errorlevel.htmls
ECHO Running choco push complete. Errorlevel was %ERRORLEVEL%

EXIT %ERRORLEVEL%
