# DumpAnalyzer
Console application to automate dump analysis using Debugdiag 2.0 analysis engine

What is this application and Why should I use it?
---------------------------------------------------

Microsoft Support Services, created a tool that helps diagnosing common problems found on Windows user processes and applications called Debug Diagnostics (debugdiag). The tool has a rich UI that allows IT professionals to create rules to gather memory dumps from production applications. It also provides a WPF UI to run an automated set of rules, on the memory dumps that have been collected, to create an HTML report with with a detailed analysis.

While the UI is very helpfull and complete, it was not designed for helping on automate the process of creating the analysis of collected dumps. Therefore I created this tool in order to have a way to automate the process of creating reports by running this simple command line tool.

A version of this tool is currently used to generate automated analysis for Azure Web Apps on a extension called Diagnostics as a Service http://azure.microsoft.com/blog/2014/07/08/daas/

If you are looking for a command line tool to automate the process of creating reports for dump files, you may find this tool usefull.

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
------------------

DumpAnalyzer.exe -dumpFile c:\w3wp.dmp -symbols srv\*c:\symsrv\*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\

DumpAnalyzer.exe -folder c:\dumpfiles\ -symbols srv\*c:\symsrv\*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\reports\ -showResults
