#!/usr/bin/env python
#
# Copyright 2016 Zebedee Mason
#
#  The author's copyright is expressed through the following notice, thus
#  giving effective rights to copy and use this software to anyone, as shown
#  in the license text.
#
# NOTICE:
#  This software is released under the terms of the GNU Lesser General Public
#  license; a copy of the text has been released with this package (see file
#  LICENSE, where the license text also appears), and can be found on the
#  GNU web site, at the following address:
#
#           http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
#
#  Please refer to the license text for any license information. This notice
#  has to be considered part of the license, and should be kept on every copy
#  integral or modified, of the source files. The removal of the reference to
#  the license will be considered an infringement of the license itself.
import sys, os, os.path, json

def PrintLine(filename, lineno):
	print(filename)
	index = 0
	for line in open(filename):
		index += 1
		if index == lineno:
			print(line.rstrip())
			break

for root, dirs, files in os.walk("."):
	for  name in files:
		if name != "AssemblyInfo.cs":
			continue
		filename = os.path.join(root, name)
		print(filename)
		for line in open(filename):
			if -1 == line.find("Version"):
				continue
			if line[:1] != '[':
				continue
			print(line.rstrip())
		
PrintLine("GUI\\RelNotes.txt", 1)
PrintLine("GUI\\License.txt", 1)
PrintLine("GUI\\Usage.txt", 1)
PrintLine("GUI\\source.extension.vsixmanifest", 4)
PrintLine("GUI\\DeepEndsPackage.cs", 49)
