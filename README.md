# PonySFM Workshop
[![Build status](https://ci.appveyor.com/api/projects/status/ib8fqs0ghean9103?svg=true)](https://ci.appveyor.com/project/Nuke928/workshop)

A .NET-powered app to help you manage your Source Filmmaker mod-base.

![Workshop](https://user-images.githubusercontent.com/4589491/41930666-0194afda-797c-11e8-9ab3-eafdfcf426ec.png)

## Requirements

* Visual Studio 2017
* .NET Framework 4.5.2
* Source Filmmaker

## File layout

| Name           | Contents                                                                |
| ---------------| ------------------------------------------------------------------------|
|CoreLib         |Provides core functionality and classes that can be separated from the UI|
|CoreLibTest     |Unit tests of CoreLib classes                                            |
|PonySFM Workshop|Main executable and UI parts                                             |
|UITest          |Unit tests of some UI-related classes                                    |

## Building

Use Visual Studio 2017 to open the project file. Right click on the solution and click "Restore NuGet Packages".
