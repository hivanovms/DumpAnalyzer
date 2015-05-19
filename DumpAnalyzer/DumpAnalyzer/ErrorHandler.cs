using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugDiag.DumpAnalyzer
{
    public static class ErrorHandler
    {
        public static void ReportError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\r\n{0}", errorMessage);
            Console.ResetColor();
        }

        public static void ReportError(string errorMessage, string source)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
            Console.WriteLine(source);
            Console.WriteLine();
        }

        public static void ShowCommandLineOptions()
        {
            Console.WriteLine("DumpAnalyzer execution parameters: \r\n");
            Console.WriteLine("-dumpFile|-folder : use -dumpFile to point to specific dump file name(s) to");
            Console.WriteLine("                    analyze separated by ',' no spaces, inlcuding full path.");
            Console.WriteLine("                    If you want to analyze all dumps contained on a folder, ");
            Console.WriteLine("                    and generate a single report file, use -folder option and");
            Console.WriteLine("                    point to the path where the dump files are located.\r\n");
            Console.WriteLine("                    Note: only files with extension (*.dmp) will be used\r\n");
            Console.WriteLine("-out              : Path where the report will be saved \r\n");
            Console.WriteLine("[-rules]          : Name of the rules you would like to execute on the dump");
            Console.WriteLine("                    separated by ',' no spaces\r\n");
            Console.WriteLine("                    If omitted, all rules from the assemblies specified on");
            Console.WriteLine("                    -ruleAssemblies parameter will be executed. If no value");
            Console.WriteLine("                    on -ruleAssemblies is given, only CrashHangAnalysis will");
            Console.WriteLine("                    be executed as the default Rule");
            Console.WriteLine("                    Default available rules on Debugdiag.AnalysisRules.dll are:");
            Console.WriteLine("                    CrashHangAnalysis");
            Console.WriteLine("                    DotNetMemoryAnalysis");
            Console.WriteLine("                    MemoryAnalysis");
            Console.WriteLine("                    PerfAnalysis\r\n");
            Console.WriteLine("                    For custom rules use the class name that implements the");
            Console.WriteLine("                    rule including the namespace information.\r\n");
            Console.WriteLine("[-symbols]        : Symbols servers used for analysis\r\n");
            Console.WriteLine("[-ruleAssemblies] : Assemblies that contain custom rules\r\n");
            Console.WriteLine("[-dumpsFolder]    : Assemblies that contain custom rules\r\n");
            Console.WriteLine("[-ShowResults]    : Shows the report on IE once it finishes. \r\n");
            Console.WriteLine("Examples: \r\nDumpAnalyzer.exe -dumpFile c:\\w3wp.dmp -symbols srv*c:\\symsrv*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\\");
            Console.WriteLine("\r\nDumpAnalyzer.exe -folder c:\\dumpfiles\\ -symbols srv*c:\\symsrv*http://msdl.microsoft.com/download/symbols -rules CrashHangAnalysis -out c:\\reports\\ -showResults");
        }

        public static void PrintExceptionError(string errorDescription, string assemblyName, string exceptionError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(errorDescription, assemblyName));
            Console.ResetColor();
            Console.WriteLine(exceptionError);
            Console.WriteLine();
        }

        public static void PrintWarning(string errorDescription, string rule, string assemblyName)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(errorDescription, rule, assemblyName));
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
