A Simple - Yet Quite Powerful - Palette Quantizer Library
=========================================================
- Converted to a class library by Leonard T. Erwine, October, 2014.

This contains source code which was taken from a Code Project Article and modified as a class library.

NOTE: A different license (The Code Project Open License) applies to the code in this folder.

A bug was discovered in the Octree quantizer, which I fixed using locking and thread-safe collections from the LTEToolkitLibrary C# project.

Dependency Information
----------------------

The modifications to this project depend upon thread-safe collections which have been ported into in the Rimss.Common project.

Summary of modifications
------------------------

- Changed root namespace from "SimplePaletteQuantizer" to "Erwine.Leonard.T.GDIPlus.Palette" to make it uniform with the rest of the solution.

- Encountered frequent System.NullReferenceException errors due to null nodes existing in the Levels collections
	(Quantizers/Octree/OctreeQuantizer.cs, Method: OnGetPalette, while ording the level nodes by pixel presence). It did not seem that having null nodes in those
	collections was a desired behavior, so I modified Quantizers/Octree/OctreeQuantizer.cs to use thread-safe collections for the root node and level node collections.
	Also, since the level node collection was of a fixed length, changed it to use a read-only thread-safe collection.
	Lastly, I added a call to Thread.Sleep after a new root node was created, to give other threads a chance to catch up, in case this was the cause
	of the System.NullReferenceException error.

Original source code information
--------------------------------

| Name                                          | Description                                                                                     |
| --------------------------------------------: | :---------------------------------------------------------------------------------------------- |
|                <strong>Article name:</strong> | A Simple - Yet Quite Powerful - Palette Quantizer in C#                                         |
|                     <strong>License:</strong> | Code Project Open License (CPOL) 1.02                                                           |
| <strong>Code Project Article Number:</strong> | 66341                                                                                           |
|    <strong>Code Project Author Name:</strong> | Smart K8                                                                                        |
|        <strong>Author email address:</strong> | smartk8@gmail.com                                                                               |
|                 <strong>Project URL:</strong> | http://www.codeproject.com/Articles/66341/A-Simple-Yet-Quite-Powerful-Palette-Quantizer-in-C    |
|         <strong>Source Code Version:</strong> | 5.0                                                                                             |
|               <strong>Download Date:</strong> | 10/16/2014                                                                                      |
|                <strong>Download URL:</strong> | http://www.codeproject.com/KB/recipes/SimplePaletteQuantizer/SimplePaletteQuantizerVS2010v5.zip |
