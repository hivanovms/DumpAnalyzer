using System;
using System.Collections.Generic;
using System.Text;
using DebugDiag.DotNet;
using DebugDiag.DotNet.AnalysisRules;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace DebugDiag.DumpAnalyzer
{
    class Program
    {
        static List<string> dumpFiles = new List<string>();
        static List<string> assemblyNames = new List<string>();
        static List<AnalysisRuleInfo> analysisRuleInfos = new List<AnalysisRuleInfo>();
       
        private static Stopwatch stopwatch = new Stopwatch();
        private static ArgsValidator _av;
        private static AnalysisJob _analysis;

        [STAThread]
        static void Main(string[] args)
        {
            //Console.ReadKey();
            _av = new ArgsValidator(args);
            if (!_av.ValidArguments) return;
            _analysis = _av.GetAnalysisJob;
            RunAnalysis();
        }
 
        static void RunAnalysis()
        {
            using (NetAnalyzer analyzer = new NetAnalyzer())
            {
                List<AnalysisRuleInfo> ari = _analysis.AnalisysRuleInfos;

                if (ari.Count < 1 && _analysis.Modules.Count < 1)
                {
                    ErrorHandler.ReportError("Could not find any rule that matches the parameters.\r\n");
                    ErrorHandler.ShowCommandLineOptions();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\r\nThe Dump Files being analyzed are: \n\r");
                Console.ResetColor();

                foreach (string dumpName in _analysis.DumpFiles)
                {
                    Console.WriteLine(dumpName);
                }
                Console.WriteLine();

                foreach (Assembly module in _analysis.Modules)
                    analyzer.AddAnalysisRulesToRunList(module);

                //analyzer.AnalysisRuleInfos = analysisRuleInfos;
                analyzer.AnalysisRuleInfos.AddRange(ari);
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("The rules being executed are: \n\r");
                Console.ResetColor();

                foreach (AnalysisRuleInfo ruleInfo in analyzer.AnalysisRuleInfos)
                {
                    Console.WriteLine(ruleInfo.DisplayName);
                }
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("The symbols sources are: \n\r");
                Console.ResetColor();

                Console.WriteLine(_analysis.Symbols);
                Console.WriteLine();

                //Add Dump list to Analizer object so we can analyze them with the debugger
                analyzer.AddDumpFiles(_analysis.DumpFiles, _analysis.Symbols);

                NetProgress np = new NetProgress();
                np.OnSetOverallStatusChanged += new EventHandler<SetOverallStatusEventArgs>(np_OnSetOverallStatusChanged);
                //np.OnSetCurrentPositionChanged += new EventHandler<SetCurrentPositionEventArgs>(np_OnSetCurrentPositionChanged);
                np.OnSetCurrentStatusChanged += new EventHandler<SetCurrentStatusEventArgs>(np_OnSetCurrentStatusChanged);
                np.OnEnd += new EventHandler(np_OnEnd);

                Console.WriteLine(string.Format("{0} - Start Analysis", DateTime.Now.ToLongTimeString()));
                stopwatch.Start();
                
                try
                {
                    analyzer.RunAnalysisRules(np, _analysis.Symbols, "", _analysis.ReportPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    stopwatch.Stop();
                }
                np.End();

                if (_analysis.ShowResults)
                    analyzer.ShowReportFiles();
            }
        }

        static void np_OnEnd(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("{0} - Finished" ,  DateTime.Now.ToLongTimeString()));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("\n\rTotal time of analysis: {0}", stopwatch.Elapsed.ToString()));
            Console.ResetColor();
        }

        static void np_OnSetCurrentStatusChanged(object sender, SetCurrentStatusEventArgs e)
        {
            if (e.NewStatus != string.Empty)
                Console.WriteLine(string.Format("{0} - {1}",  DateTime.Now.ToLongTimeString(), e.NewStatus));
        }

        static void np_OnSetOverallStatusChanged(object sender, SetOverallStatusEventArgs e)
        {
            if (e.NewStatus != string.Empty)
                Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToLongTimeString(), e.NewStatus));
        }
 
    }
}
