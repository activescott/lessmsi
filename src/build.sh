#!/bin/bash

# This is to compile the project on travis-ci which is linux only. Although we can't run tests on linux, at least a good compile on linux will help ensure code always compiles.
# using p:TargetFrameworkProfile= defaults to .NET 3.5 FULL profile instead of Client Profile. MOno does not support client profile!
xbuild /p:TargetFrameworkProfile= ./ExplorerShortcutHelper/ExplorerShortcutHelper.csproj
xbuild /p:TargetFrameworkProfile= ./LessMsi.Core/LessMsi.Core.csproj
xbuild /p:TargetFrameworkProfile= ./LessMsi.Cli/LessMsi.Cli.csproj
xbuild /p:TargetFrameworkProfile= ./LessMsi.Gui/LessMsi.Gui.csproj
# nuget is giving me errors on travis/xbuild so skipping this: xbuild /p:TargetFrameworkProfile= ./LessMsi.Tests/LessMsi.Tests.csproj