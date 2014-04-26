#!/bin/bash

# http://dlafferty.blogspot.ru/2013/08/building-your-microsoft-solution-with.html
# Mono ships with an empty certificate store. The store needs to be populated with common certificates in order for HTTPS to work.
# Use the mozroots tool to do this.
mozroots --import --sync

# This is to compile the project on travis-ci which is linux only. Although we can't run tests on linux, at least a good compile on linux will help ensure code always compiles.
# using p:TargetFrameworkProfile= defaults to .NET 3.5 FULL profile instead of Client Profile. MOno does not support client profile!

CONFIG=Release
PROPS="/p:OS=mono /p:TargetFrameworkProfile= /p:Configuration=$CONFIG"

# compile projects
xbuild $PROPS ./ExplorerShortcutHelper/ExplorerShortcutHelper.csproj
xbuild $PROPS ./LessMsi.Core/LessMsi.Core.csproj
xbuild $PROPS ./LessMsi.Cli/LessMsi.Cli.csproj
xbuild $PROPS ./LessMsi.Gui/LessMsi.Gui.csproj

# compile tests
# clear PostBuildEvent since the commands in LessMsi.Tests.csproj file  are windows specific
xbuild $PROPS /p:PostBuildEvent='' ./Lessmsi.Tests/LessMsi.Tests.csproj

# copy test files
# cp -R ./Lessmsi.Tests/TestFiles ./Lessmsi.Tests/bin/$CONFIG/
# running tests on linux does not work since lessmsi interops with msi.dll which available only on windows
# nunit-console ./Lessmsi.Tests/bin/$CONFIG/LessMsi.Tests.dll
