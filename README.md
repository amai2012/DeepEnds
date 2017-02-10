# DeepEnds

Dive into architecture with DeepEnds...

Binaries are available as a Visual Studio extension (
[Marketplace](https://marketplace.visualstudio.com/items?itemName=ZebM.DeepEnds)
) and as a 
[NuGet package.](https://www.nuget.org/packages/DeepEnds.Console/)

## Introduction
DeepEnds analyses the dependencies between children of a node in a tree. 
The tree hierarchy may be formed from namespace and class, file name and filter or in a bespoke manner. 
The software is supplied as a Visual Studio 2015 extension that calls into a console application allowing for batch running.

## Input

### Visual C++
The software takes a list of `sln` and `vcxproj` files and will parse C++ source files for their `#include` statements, 
these source files then form the leaves of the tree. 
The hierarchy is composed from the filter as specified in the associated `vcxproj.filters` files.

### .NET
The software takes a list of `sln`, `csproj`, `vbproj` and .NET assemblies (`exe`, `dll`) and extracts a hierarchy based on namespace and class.

### Doxygen XML output
The software takes a single XML file path and scans its parent directory for all XML files. 
The software then reads any specified `compounddef` and `memberdef` items to form the leaves. 
This also allows the user to input data from their own parser where a leaf can be represented by XML of the form

```xml
  <compounddef id="class_f_e_a_1_1_labels_1_1_mapped" kind="class">
    <compoundname>FEA::Labels::Mapped</compoundname>
    <ref refid="class_f_e_a_1_1_labels_1_1_base"/>
  </compounddef>
```

where `id` and `refid` are identifiers for the leaves and the hierarchy is specified in `compoundname` using `::` as a separator.

### DGML
The software will read in a DGML file which may be produced by this software or another tool

## Output

### DGML graph

![DGML](https://github.com/zebmason/DeepEnds/raw/master/Doc/dgml.png)

This XML file format has a viewer within Visual Studio that displays nested graphs in an interactive editor.

### CSV table

For each graph a series of statistics are reported:

* Whether there is a cycle
* The Cyclomatic number
* The number of leaves which aren't a child, grandchild, etc. of the graph
* The sum total number of lines of code
* The average number of lines of code of the leaves

### HTML / Markdown report

In addition to the table contained in the CSV file a number of other tables are displayed.
These are described in the report itself and can be found in [example output](Doc/Generated/DeepEndsDoc.md).

### Doxygen comments

The contents of the HTML report are written out as equivalent comments to be parsed by Doxygen.
Additionally each graph is written out for processing by Dot to produce an embedded image of the graph.
Note that for a large project it is possible for this to overwhelm Doxygen.

## Further reading

### Architecture
* [As-Is Software Architecture](http://www.codeproject.com/Articles/1098935/As-Is-Software-Architecture)
* [Dive into Architecture with DeepEnds](http://htmlpreview.github.com/?https://github.com/zebmason/DeepEnds/blob/master/Doc/userguide.html) - now withdrawn
* [Dependency Analysis in Visual C++](http://www.codeproject.com/Articles/1137021/Dependency-Analysis-in-Visual-Cplusplus)
* [Dependency Analysis with Doxygen](https://www.codeproject.com/Articles/1155619/Dependency-Analysis-with-Doxygen)
* [Why Favour the Cyclomatic Number?](http://www.codeproject.com/Tips/1116433/Why-Favour-the-Cyclomatic-Number)
* [Big Design Up Front or Emergent Design?](https://www.codeproject.com/Tips/1158303/Big-Design-Up-Front-or-Emergent-Design)
* [Counting Lines of Code](http://www.codeproject.com/Tips/1136171/Counting-Lines-of-Code)
* [Dependencies in the Gang of Four examples](https://github.com/zebmason/GoFRefactored/blob/master/README.md)

### Design of this code
* [Generate a WPF frontend from a console application](https://www.codeproject.com/Articles/1147415/Generate-a-WPF-frontend-from-a-console-application)
* [Releasing a Visual Studio Extension](http://www.codeproject.com/Tips/1136303/Releasing-a-Visual-Studio-Extension)
