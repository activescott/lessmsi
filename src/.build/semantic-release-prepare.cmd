@echo off

REM This script called by semantic-release to "prepare" the build.
REM Since semantic-release determines the version number based on commits, we rebuild it with the new version number:

IF [%1]==[] (
    ECHO ERROR: Must supply build version as first parameter to this bat file! 1>&2
    EXIT 1
)
set _BUILD_VERSION=%1

ECHO Running msbuild...

msbuild .\src\.build\lessmsi.msbuild /p:TheVersion=%_BUILD_VERSION% /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

REM NOTE: ECHO does not clear/set errorlevel https://ss64.com/nt/errorlevel.htmls
ECHO Running msbuild complete.

EXIT %ERRORLEVEL%
