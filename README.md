# DeepEnds

## Introduction

DeepEnds seeks to bridge the code-model gap as described by [George Fairbanks](http://georgefairbanks.com/book/), and thus may be considered an Architecture Development Environment. The software accompanies a series of articles on CodeProject:
* [As-Is Software Architecture](http://www.codeproject.com/Articles/1098935/As-Is-Software-Architecture)
* [Dive into Architecture with DeepEnds](http://htmlpreview.github.com/?https://github.com/zebmason/DeepEnds/blob/master/Doc/userguide.html) - now withdrawn
* [Why Favour the Cyclomatic Number?](http://www.codeproject.com/Tips/1116433/Why-Favour-the-Cyclomatic-Number)

The basis of the software is the manipulation of dependencies between units of code. The directly supported units are

* Visual C/C++ source files - uses the .filters file
* .NET assemblies, C# and VB source

For output options are

* a DGML file with a hierarchical dependency graph
* a HTML report detailing the Cyclomatic Number and the source of the dependencies

## Related projects
* [Python script on which this was based](https://github.com/zebmason/itdepends)
* [.NET assembly reader](https://github.com/zebmason/netdepends)
