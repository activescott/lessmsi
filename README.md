lessmsi [![Build Status](https://secure.travis-ci.org/activescott/lessmsi.png?branch=master)](http://travis-ci.org/activescott/lessmsi) [![tip for next commit](http://tip4commit.com/projects/316.svg)](http://tip4commit.com/projects/316)
========

This is a utility with a graphical user interface and a command line interface that can be used to view and extract the contents of an MSI file. 

To extract from the command line:

     lessmsi x <msiFileName> [<outouptDir>]

For more command line usage see [CommandLine](https://github.com/activescott/lessmsi/wiki/Command-Line).

Features
==========
Windows Explorer Integration
-----
Lessmsi also integrates with Windows Explorer so that you can right-click on a Windows Installer file (.msi file) and select "Extract Files" to extract it into a folder right there:

![lessmsi Explorer Integration screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-explorerintegration.png)



Just select _Preferences_ from the _Edit_ menu to enable (or disable) the explorer integration:

![lessmsi Preferences Dialog screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-preferences.png)


GUI
-----
In addition to allowing you to extract files from the command line and from inside Windows Explorer, lessmsi has a graphical user interface that allows you to view detailed information about any MSI file. 

![lessmsi Files Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-filestab.png)


MSI Table Viewer
-----
Windows Installer (.msi files) are based on an internal database of tables. Lessmsi features a viewer for those tables. Useful for people who work a lot with installers.

![lessmsi Table Tab screenshot](https://raw.github.com/activescott/lessmsi/master/misc/screenshot-tabletab.png)


lessmsi on OS X using WINE
-----

all of the following approaches give errors on the command line, but seem to work regardless.

#### lessmsi command line using wine
use lessmsi on the command line using wine: (this example uses the wine.app that comes with winebottler)

    /Applications/Wine.app/Contents/Resources/bin/wine /Users/username/Downloads/lessmsi-v1.2.0/lessmsi.exe v /Users/username/Downloads/msi2xml-2.2.1.957.msi 

#### lessmsi GUI using wine

lessmsi GUI working with WineBottler.  http://winebottler.kronenberg.org/

You can download a zip compressed .app created with winebottler here:  https://dl.dropboxusercontent.com/u/640753/lessmsi.zip

#### You can make it yourself using the following screenshot & winebottler:
![screen shot 2015-03-13 at 3 02 08 pm](https://cloud.githubusercontent.com/assets/2439367/6645360/4f9a462e-c992-11e4-8e02-a161096e3699.png)


Suggestion? Problem? Comment?
=====
If you have a problem *please* submit it by clicking in the [Issue tracker](https://github.com/activescott/lessmsi/issues) and I'll look into it when I can.

Contributions
=====
Pull requests are welcome! Just make sure the Travis-CI build (compile only) passes and you run unit tests and I'll merge your contributions ASAP! The Issues app has an indication of some of the plans.

You Earn Bitcoin Tips for Contributing!
-----
We're now tipping committers with bitcoin: [![tip for next commit](http://tip4commit.com/projects/316.svg)](http://tip4commit.com/projects/316)

![Windows Developer Power Tools](http://www.windevpowertools.com/images/wdpt1.gif)


*Originally from Scott Willeke's blog http://blogs.pingpoet.com/overflow and http://blog.scott.willeke.com . It was also called Less Msiérables as well as lessmsi.*
