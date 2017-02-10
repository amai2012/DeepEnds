# Documentation


This report was written by 
[DeepEnds](https://github.com/zebmason/deepends)
 which is distributed as both a 
[Visual Studio extension](https://marketplace.visualstudio.com/items?itemName=ZebM.DeepEnds)
 and as a 
[NuGet package.](https://www.nuget.org/packages/DeepEnds.Console/)
Source code is available from GitHub, there are a number of articles, available online, about its usage:



* [Dive into Architecture with DeepEnds](http://htmlpreview.github.com/?https://github.com/zebmason/DeepEnds/blob/master/Doc/userguide.html)

* [Dependency Analysis in Visual C++](http://www.codeproject.com/Articles/1137021/Dependency-Analysis-in-Visual-Cplusplus)

* [Dependency Analysis with Doxygen](https://www.codeproject.com/Articles/1155619/Dependency-Analysis-with-Doxygen)




There are also some articles on the motivation and theoretical foundations of the software:



* [As-Is Software Architecture](http://www.codeproject.com/Articles/1098935/As-Is-Software-Architecture)

* [Big Design Up Front or Emergent Design?](https://www.codeproject.com/Tips/1158303/Big-Design-Up-Front-or-Emergent-Design)

* [Why Favour the Cyclomatic Number?](https://www.codeproject.com/Tips/1116433/Why-Favour-the-Cyclomatic-Number)

* [Counting Lines of Code](https://www.codeproject.com/Tips/1136171/Counting-Lines-of-Code)




The following sections refer to the individual sections for each level.


## Table



The table is grouped by columns. The summary table is sorted on the value of 
the value of (E+P-N)/N, subsequent instances are sorted on the section name.


### Section



The node label of the graph (with the obvious exception of "Top level").





### Cycle



If a cycle in the graph (circular dependency) occurs then the word cycle will appear as the value otherwise it 
is left blank.





### Cyclomatic Number



The larger the value of (E + P) / N then the more complex the directed graph is, where




* E: Number of edges

* P: Number of parts

* N: Number of nodes



The value of (E + P - N) / N varies between 0 and N. A strictly layered architecture will have a value of 0.



The sum refers to the sum of the value to its left plus all the values at its child nodes, recursively.





### Externals



Externals refers to the number of dependencies which aren't children.
The following max refers to the maximum of that value and the value at any child nodes.





### SLOC



SLOC stands for source lines of code; whilst reading the source an attempt may have 
been made not to count blank or comment lines. The max and sum are calculated over 
the child nodes recursively down to the leaf nodes.





### Probability of SLOC



An attempt has been made to fit the log-normal distribution to SLOC which has then 
been used to calculate the expected file size, this is not displayed if there is only 
one value to fit (so as to avoid domination of the statistic by boilerplate). This 
value is bracketed by the lower and upper limits of the 90% confidence interval if 
the upper limit is less than the maximal value of SLOC on a leaf, otherwise it is 
left blank. The following max refers to the maximum of the expected value and the 
value at any child nodes.





## Lines of code


This table is a list of leaf nodes versus the
number of lines of code sorted on decreasing size.


## Usage


This table is an alphabetically sorted list of the leaf nodes in this level
versus their usage at this level and all higher levels.
Once the maximal value has been achieved for a row the subsequent columns
are filled with blanks.


## Interface


This table is an alphabetically sorted list of the leaf nodes that aren't used
at a higher level.


## Externals


This table is an alphabetically sorted list of the leaf nodes that are dependencies
but aren't children of this level.


## Internals


This table is an alphabetically sorted list of the leaf nodes that constitute the dependencies
from which the edges are formed. Note that dependent nodes are not included unless they
also happen to be depended upon.


## Edge definitions


The edges are defined by underlying dependencies.
For each edge a table lists all the pairs of leaf nodes
that the edge is composed of.



## Structure Matrix


The structure matrix represents the dependency of one node upon another in the level.
To work out what a node depends on read along a row until the value of 1 is encountered
then read vertically to the diagonal (represented by a backslash).
The row containing that diagonal is the corresponding dependency.

The rows of the matrix have been sorted in the attempt to ensure that the 1's are
below the diagonal. If this is not the case then a cycle exists in the corresponding
graph. i.e. a circular dependency exists.



