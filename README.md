BeamAnalysis App:

This C# UWP App uses the Finite Element Method (FEM) to analyze 2D beams.  Beams can be configured with multiple support and load configurations.

One or more supports can be entered.  Displacement and rotation at each support can be restrained.
One or more concentrated loads can be entered.  Each concentrated load can contain a vertical force and/or moment value.
One or more uniform loads can be entered.  Rectangular, triangular, and trapezoidal uniform loads are supported by Application.  Each uniform load can be applied to whole beam or portion of beam.  Uniform loads can overlap each other.

Uniform loads are simulated with a series of concentrated loads.  The spread distance of simulated loads can be adjusted.  Smaller spread distances will produce more simulated loads and more accurate results but will require more time to calculate results.

This App requires library files LibMainPageCommon.cs and LibNumerics.cs located in Library-Files-for-other-Repository-Apps to be linked under folder named 'Libraries' to compile.

Underlying FEM C# code used by this application was developed by Staskolukasz.  He placed his code on GitHub.com via ‘fob2d’, ‘Simple implementation of the Finite Element Method in C#.  Single span steel I beam under concentrated loads.’
Staskolukasz’s underlying code has been updated considerably by programmer to run within the UWP environment using latest Visual Studio releases.
His code has also been customized considerably to allow input and output for custom beam support and load configurations entered by User.

I no longer intend to support this App so placing code on GitHub so others can use if useful.
