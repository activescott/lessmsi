# lessmsi

[![Build Status](https://ci.appveyor.com/api/projects/status/github/activescott/lessmsi?branch=master&svg=true)](https://ci.appveyor.com/project/activescott/lessmsi)
[![Chocolatey Downloads](https://img.shields.io/chocolatey/dt/lessmsi.svg?style=popout)](https://chocolatey.org/packages/lessmsi)
[![chocolatey](https://img.shields.io/chocolatey/v/lessmsi.svg?maxAge=2592000)](https://chocolatey.org/packages/lessmsi)
[![GitHub forks](https://img.shields.io/github/forks/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/network)
[![GitHub stars](https://img.shields.io/github/stars/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/stargazers)
[![tip for next commit](https://tip4commit.com/projects/316.svg)](https://tip4commit.com/projects/316)
[![GitHub issues](https://img.shields.io/github/issues/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/issues)

This is a utility with a graphical user interface and a command line interface that can be used to view and extract the contents of an MSI file.

# Usage

To extract from the command line:

     lessmsi x <msiFileName> [<outputDir>]

For more command line usage see [CommandLine](https://github.com/activescott/lessmsi/wiki/Command-Line).

# Installation

Install [via Chocolatey](https://chocolatey.org/packages/lessmsi) (or [download a zip](https://github.com/activescott/lessmsi/releases/latest)).

# Features

## Windows Explorer Integration

Lessmsi also integrates with Windows Explorer so that you can right-click on a Windows Installer file (.msi file) and select "Extract Files" to extract it into a folder right there:

![lessmsi Explorer Integration screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-explorerintegration.png)

Just select _Preferences_ from the _Edit_ menu to enable (or disable) the explorer integration:

![lessmsi Preferences Dialog screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-preferences.png)

## GUI

In addition to allowing you to extract files from the command line and from inside Windows Explorer, lessmsi has a graphical user interface that allows you to view detailed information about any MSI file.

![lessmsi Files Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-filestab.png)

## MSI Table Viewer

Windows Installer (.msi files) are based on an internal database of tables. Lessmsi features a viewer for those tables. Useful for people who work a lot with installers.

![lessmsi Table Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-tabletab.png)

# Suggestion? Problem? Comment?

If you have a problem _please_ submit it by clicking in the [Issue tracker](https://github.com/activescott/lessmsi/issues) and I'll look into it when I can.

# Donate to Support Open Source Contributors of lessmsi

You can **donate** in two ways:

- [Donate at Tip4Commit](https://tip4commit.com/github/activescott/lessmsi) (Bitcoin only) to fund a general fund. Each new commit to this repository receives a percentage of the available balance.
- [Donate at IssueHunt](https://issuehunt.io/r/activescott/lessmsi) to deposit your donation as a "bounty" against a specific issue or feature request. When a contributor resolves the issue they will earn the deposit for that issue.

# Contributing

Pull requests are welcome! Just make sure the Travis-CI build (compile only) passes and you run unit tests and I'll merge your contributions ASAP! The Issues app has an indication of some of the plans.

## You Earn Bitcoin Tips for Contributing!

We're now tipping committers with bitcoin: [![tip for next commit](http://tip4commit.com/projects/316.svg)](http://tip4commit.com/projects/316)

## You Earn a Bounty for Resolving Issues!

We're now [registered at IssueHunt](https://issuehunt.io/r/activescott/lessmsi) so contributors can earn the bounty on specific issues that users have deposited funds against.

## Development Environment

To set up a _free_ Windows development environment that works with Lessmsi:

1. Install [VMWare Fusion Player](https://www.vmware.com/products/fusion.html) (using free "Player" version, only needed if developing on MacOS - [homebrew also has a vmware-fusion formula](https://formulae.brew.sh/cask/vmware-fusion))
2. Download a Virtual Machine for Windows development that Microsoft provides at https://developer.microsoft.com/en-us/windows/downloads/virtual-machines/. These VMs include virtual machine with the latest versions of Windows, the developer tools, SDKs, and samples ready to go.
3. Install chocolatey as described at https://chocolatey.org/install
4. Install some things using the "Command Prompt" (`cmd` rather than "Power Shell"/`ps`) and `winget` here):

```sh
# install git for windows (this also installs "bash" via "Git Bash")
$ winget install git.git

# install chocolatey (this is required to run the MSBuild script and create the chocolatey package that is deployed by CI)
$ winget install chocolatey

# NOTE: you'll need to close this window and open a new one to get the cpack and git on the path
```

Then you can switch to bash by typing "bash" in the start menu and selecting "Git Bash" and follow these steps:

```sh
# in Git Bash..., generate and ssh key for github:
$ ssh-keygen
# follow prompts...

# Print *public* ssh key to console, and register the printed value at github at https://github.com/settings/ssh/new by copying the key printed from the prior step into that box
$ cat ~/.ssh/id_ed25519.pub

# create directories for code and clone the repo:
$ mkdir /c/src
$ cd /c/src
$ git clone git@github.com:activescott/lessmsi.git
```

Now I switch back to a "Developer Command Prompt for Visual Studio" (i.e. `cmd` with helpful PATH) to do a build since it is more CI-like:

```sh
$ cd \src\lessmsi\src
$ .\build.bat

# I usually start with 0.0.1 when prompted for a version number...

```

## Supported Windows Versions

The latest versions of Lessmsi should support the oldest version of Windows that Microsoft still officially supports according to https://learn.microsoft.com/en-us/windows/release-health/supported-versions-windows-client
That means we need to also target a .NET Framework version that is included in the oldest version Windows that Microsoft still supports which can be found at https://en.wikipedia.org/wiki/.NET_Framework_version_history

For example as of 2024, Windows 10 is the oldest version of Windows still supported and according to the Wikipedia article, .NET Framework 4.8 is included in Windows 10 (some reasonably current update to Windows 10), so targeting .NET Framework 4.8 is ideal since any reasonably up-to-date version of Windows 10 will have .NET Framework 4.8. Going further, as of April 2024, we see that while .NET Framework 4.8.1 is also available, it is only included in Widnows 11, and not included in any Windows 10 version, so we should *not* target .NET Framework 4.8.1 as some of our Windows 10 users may not yet have 4.8.1 installed.

## Deploying & Publishing New Versions

New versions are published to GitHub Releases and Chocolatey via [semantic-release](https://github.com/semantic-release/semantic-release) to consistently release [semver](https://semver.org/)-compatible versions. Only the master branch is deployed.

To trigger a release just commit (or merge) to the master branch. All commits in master should use the Conventional Commits following [Angular Commit Message Conventions](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#-git-commit-guidelines).

Then the CI script in the repo at [/appveyor.yml](https://github.com/activescott/lessmsi/blob/master/appveyor.yml) should build, test the code and if the build & tests succeed deploy it first to github and then to Chocolatey. Release configuration via semantic-release is in [/release.config.js](https://github.com/activescott/lessmsi/blob/master/release.config.js) and the `semantic-release-*.cmd` files in the [/src/.build](https://github.com/activescott/lessmsi/tree/master/src/.build) folder.

---

_Originally from Scott Willeke's blog http://blogs.pingpoet.com/overflow and http://blog.scott.willeke.com.
It was also called Less Msiérables as well as lessmsi._

_Was featured in the book [Windows Developer Power Tools](https://www.oreilly.com/library/view/windows-developer-power/0596527543/) as Less MSIérables._

```

```
