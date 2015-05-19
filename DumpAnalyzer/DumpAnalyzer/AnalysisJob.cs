using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DebugDiag.DotNet.AnalysisRules;
using System.Reflection;

namespace DebugDiag.DumpAnalyzer
{
    public class AnalysisJob
    {
        private const string PUBLICSYMBOLS = "srv*c:\\symsrv*http://msdl.microsoft.com/download/symbols";
        private List<string> _dumpFiles = new List<string>();
        private List<string> _assemblyNames = new List<string>();
        private List<AnalysisRuleInfo> _analysisRuleInfos = new List<AnalysisRuleInfo>();
        private string _symbols;
        private string _reportPath;
        private string[] _rules = null;
        private bool _showResults = false;
        private RulesWizard _rw = null;

        public List<string> DumpFiles
        {
            get { return _dumpFiles; }
        }

        public List<string> AssemblyNames
        {
            get { return _assemblyNames; }
        }

        public bool AddDumpFile(string dumpName)
        {
            string parsedDumpName = dumpName.Replace("\"", "");
            if (FileValidator.Validate(parsedDumpName, true))
                _dumpFiles.Add(parsedDumpName);
            else
            {
                ErrorHandler.ReportError("The following dump file was not be found and will not be analyzed:", parsedDumpName);
                return false;
            }
            return true;
        }

        public string Symbols
        { 
            get
            {
                if (String.IsNullOrEmpty(_symbols)) return PUBLICSYMBOLS;
                else return _symbols;
            }
            set
            {
                //TODO: create validation Logic for this
                _symbols = value;
            }
        }

        public string ReportPath
        {
            get { return _reportPath; }
            set { _reportPath = value; }
        }

        public bool ShowResults
        {
            get { return _showResults; }
            set { _showResults = value; }
        }

        public string[] Rules
        {
            set { _rules = value; }
            get
            {
                if (_rules != null) return _rules;
                else return null;
            }
        }

        public bool AddAssembly(string AssemblyName)
        {
            if (FileValidator.Validate(AssemblyName, true))
                _assemblyNames.Add(AssemblyName);
            else
            {
                ErrorHandler.ReportError("The following Assembly file was not be found and rules based on it will not be executed", AssemblyName);
                return false;
            }
            return true;
        }

        public List<AnalysisRuleInfo> AnalisysRuleInfos
        {
            get
            {
                if (_rw == null) 
                    _rw = new RulesWizard(_rules, _assemblyNames);
                return _rw.AnalysisRulesInfos;
            }
        }

        public List<Assembly> Modules
        {
            get
            {
                if (_rw == null)
                    _rw = new RulesWizard(_rules, _assemblyNames);
                return _rw.Modules;

            }
        }


    }
}
