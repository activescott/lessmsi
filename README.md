LessMsi [![Build Status](https://secure.travis-ci.org/activescott/lessmsi.png?branch=master)](http://travis-ci.org/activescott/lessmsi)
========

This is a utility with a graphical user interface and a command line interface that can be used to view and extract the contents of an MSI file. 

To extract from the command line:

     lessmsi x &lt;msiFileName&gt; [&lt;outouptDir&gt;]

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



Suggestion? Problem? Comment?
=====
If you have a problem *please* submit it by clicking in the [Issue tracker](https://github.com/activescott/lessmsi/issues) and I'll look into it when I can.

Contributions
=====
Contributions are welcome! There is a `!readme.txt` file included with the source that contains a TODO list and plans. Submit new issues on the issues tab of this project with with patches and I'll merge your contributions ASAP.

[![Windows Developer Power Tools](http://www.windevpowertools.com/images/wdpt1.gif)]

#<wiki:gadget url="http://www.ohloh.net/p/480392/widgets/project_cocomo.xml" height="240" border="0"/>
#<wiki:gadget url="http://www.ohloh.net/p/480392/widgets/project_languages.xml" border="1"/>

*Originally from Scott Willeke's blog http://blogs.pingpoet.com/overflow and http://blog.scott.willeke.com . It was also called Less Msiérables as well as lessmsi.*
