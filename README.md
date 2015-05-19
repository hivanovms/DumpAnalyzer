# DumpAnalyzer
Console application to automate dump analysis using Debugdiag 2.0 analysis engine

Requirements:
----------------------------

In order to use this project you will need to download and install DebugDiag 2.0 Update 1 or a most recent version of the tool, you can find it here:

http://www.microsoft.com/en-us/download/confirmation.aspx?id=42933

Use Visual Studio 2013 to download build the project


Setting up the Environment:
-----------------------------

The project has a reference to DebugDiag.DotNet.dll module that is found on the default directory where debugdiag was intalled, if you perfromed a default installation you will find the assembly on:

C:\program files\debugdiag\DebugDiag.DotNet.dll

By default the project output was set to C:\program files\debugdiag\ but you can build the console application on any directory, it is not necessary to have the assembly on the same directory where debugdiag has been installed.


How to run the console application:
--------------------------------------

From the command prompt you can run DumpAnalyzer.exe to see the help on how to use the command line tool:

DumpAnalyzer execution parameters:

-dumpFile|-folder : use -dumpFile to point to specific dump file name(s) to
                    analyze separated by ',' no spaces, inlcuding full path.
                    If you want to analyze all dumps contained on a folder,
                    and generate a single report file, use -folder option and
                    point to the path where the dump files are located.

                    Note: only files with extension (*.dmp) will be used

-out              : Path where the report will be saved

[-rules]          : Name of the rules you would like to execute on the dump
                    separated by ',' no spaces

                    If omitted, all rules from the assemblies specified on
                    -ruleAssemblies parameter will be executed. If no value
                    on -ruleAssemblies is given, only CrashHangAnalysis will
                    be executed as the default Rule
                    Default available rules on Debugdiag.AnalysisRules.dll are:
                    CrashHangAnalysis
                    DotNetMemoryAnalysis
                    MemoryAnalysis
                    PerfAnalysis

                    For custom rules use the class name that implements the
                    rule including the namespace information.

[-symbols]        : Symbols servers used for analysis

[-ruleAssemblies] : Assemblies that contain custom rules


[-ShowResults]    : Shows the report on IE once it finishes.

Examples:
DumpAnalyzer.exe -dumpFile c:\w3wp.dmp -symbols srv*c:\symsrv*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\

DumpAnalyzer.exe -folder c:\dumpfiles\ -symbols srv*c:\symsrv*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\reports\ -showResults
