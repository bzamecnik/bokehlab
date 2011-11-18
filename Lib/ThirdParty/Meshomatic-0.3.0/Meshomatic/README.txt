This is Meshomatic, a collection of open-source loader classes for 3D models.

Goal:
Provide flexible and complete mesh loaders for a variety of 3D model formats,
including but not limited to .obj, Collada, Milkshape3D.
Other things to think about: 3DSMax, Wings3D...

Principles:
Low overhead and low levels of integration.  We should provide the minimum 
required to load models, and leave how to draw them and manipulate them up
to the user.  They should not have to bend over backwards to integrate with
our system.  Our job is just to provide the data to them, as simply and
cleanly as possible.

Toward this purpose, we rely on no external libraries beyond what comes with
.NET 3.5.

Also, at least for the moment, we provide geometry ONLY.  No texture or 
material info, no skeleton or animation info, etc.  KISS.

Contents:
Meshomatic - The library itself.
Meshomatic.DumpMesh - A simple test program to load and print a mesh file.
Meshomatic.DisplayMesh - A simple mesh viewer.  Run without arguments

Status:
Milkshape 3D and .obj loaders both work on test models.  Test programs also
work, better than they used to anyway.
The library should be thread-safe; ie, you can create any number of loaders
and have each one load a different model in a different thread with no
synchronization issues.

Installation:
1) Open the Meshomatic.sln file in MonoDevelop or Visual Studio.
2) Fix any broken assembly references (Meshomatic.DisplayMesh depends on
OpenTK, etc) and compile the project.
3) Run Meshomatic.DisplayMesh.  Use the a and z keys to zoom in and out, 
and escape to quit.

To do:
*Fix bugs involving . vs , in file localization in obj's.  One issue here is,
does .obj actually specify . or ,?
*Better documentation; people need to look at the source code to figure out how
to use the bloody thing, so at least some API docs would be good.
*More loader modules --Collada, .X, Cal3D, .FBX in order of importance
*Better verification, error checking and error handling.
*Actual installation scripts?

Limitations:
It does not load skeletons, textures, etc.  Does load UV coordinates though.
It does not load non-triangular shapes.  Any ngon where n>3 gets automatically
divided up into triangles.

Thanks to:
Drake Wilson
David Gibson
Everyone in the OpenTK community using this

Contact info:
Use the OpenTK project bug tracker at http://www.opentk.com/project/Meshomatic
and/or email at snheath@gmail.com