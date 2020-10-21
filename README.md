# lessmsi #
[![Build Status](https://ci.appveyor.com/api/projects/status/github/activescott/lessmsi?branch=master&svg=true)](https://ci.appveyor.com/project/activescott/lessmsi) 
[![Chocolatey Downloads](https://img.shields.io/chocolatey/dt/lessmsi.svg?style=popout)](https://chocolatey.org/packages/lessmsi) 
[![chocolatey](https://img.shields.io/chocolatey/v/lessmsi.svg?maxAge=2592000)](https://chocolatey.org/packages/lessmsi)
[![GitHub forks](https://img.shields.io/github/forks/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/network)
[![GitHub stars](https://img.shields.io/github/stars/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/stargazers)
[![tip for next commit](https://tip4commit.com/projects/316.svg)](https://tip4commit.com/projects/316) 
[![GitHub issues](https://img.shields.io/github/issues/activescott/lessmsi.svg)](https://github.com/activescott/lessmsi/issues)

This is a utility with a graphical user interface and a command line interface that can be used to view and extract the contents of an MSI file. 

# Usage #
To extract from the command line:

     lessmsi x <msiFileName> [<outputDir>]

For more command line usage see [CommandLine](https://github.com/activescott/lessmsi/wiki/Command-Line).

# Installation #
Install [via Chocolatey](https://chocolatey.org/packages/lessmsi) (or [download a zip](https://github.com/activescott/lessmsi/releases/latest)).

# Features #
## Windows Explorer Integration ##
Lessmsi also integrates with Windows Explorer so that you can right-click on a Windows Installer file (.msi file) and select "Extract Files" to extract it into a folder right there:

![lessmsi Explorer Integration screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-explorerintegration.png)

Just select _Preferences_ from the _Edit_ menu to enable (or disable) the explorer integration:

![lessmsi Preferences Dialog screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-preferences.png)


## GUI ##
In addition to allowing you to extract files from the command line and from inside Windows Explorer, lessmsi has a graphical user interface that allows you to view detailed information about any MSI file. 

![lessmsi Files Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-filestab.png)


## MSI Table Viewer ##
Windows Installer (.msi files) are based on an internal database of tables. Lessmsi features a viewer for those tables. Useful for people who work a lot with installers.

![lessmsi Table Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-tabletab.png)

# Suggestion? Problem? Comment? #
If you have a problem *please* submit it by clicking in the [Issue tracker](https://github.com/activescott/lessmsi/issues) and I'll look into it when I can.

# Contributions #
Pull requests are welcome! Just make sure the Travis-CI build (compile only) passes and you run unit tests and I'll merge your contributions ASAP! The Issues app has an indication of some of the plans.

## You Earn Bitcoin Tips for Contributing! ##
We're now tipping committers with bitcoin: [![tip for next commit](http://tip4commit.com/projects/316.svg)](http://tip4commit.com/projects/316)

## You Earn a Bounty for Resolving Issues! ##
We're now [registered at IssueHunt](https://issuehunt.io/r/activescott/lessmsi) so contributors can earn the bounty on specific issues that users have deposited funds against.


# Donate to Support Open Source Contributors of lessmsi #
You can **donate** in two ways: 
* [Donate at Tip4Commit](https://tip4commit.com/github/activescott/lessmsi) (Bitcoin only) to fund a general fund. Each new commit to this repository receives a percentage of the available balance.
* [Donate at IssueHunt](https://issuehunt.io/r/activescott/lessmsi) to deposit your donation as a "bounty" against a specific issue or feature request. When a contributor resolves the issue they will earn the deposit for that issue.


# Deploying & Publishing New Versions #
New versions are published to GitHub Releases and Chocolatey via [semantic-release](https://github.com/semantic-release/semantic-release) to consistently release [semver](https://semver.org/)-compatible versions. Only the master branch is deployed.

To trigger a release just commit (or merge) to the master branch. All commits in master should use the Conventional Commits following [Angular Commit Message Conventions](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#-git-commit-guidelines).

Then the CI script in the repo at [/appveyor.yml](https://github.com/activescott/lessmsi/blob/master/appveyor.yml) should build, test the code and if the build & tests succeed deploy it first to github and then to Chocolatey. Release configuration via semantic-release is in [/release.config.js](https://github.com/activescott/lessmsi/blob/master/release.config.js) and the `semantic-release-*.cmd` files in the [/src/.build](https://github.com/activescott/lessmsi/tree/master/src/.build) folder.

------

*Originally from Scott Willeke's blog http://blogs.pingpoet.com/overflow and http://blog.scott.willeke.com. 
It was also called Less Msiérables as well as lessmsi.*

*Was featured in the book [Windows Developer Power Tools](https://www.oreilly.com/library/view/windows-developer-power/0596527543/) as Less MSIérables.*
