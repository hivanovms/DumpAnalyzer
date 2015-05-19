using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DebugDiag.DumpAnalyzer
{
    public class ArgsValidator
    {
        private string[] _args;
        private AnalysisJob _aj = new AnalysisJob();
        private bool _validArgs = false;

        public ArgsValidator(string[] Args)
        {
            _args = Args;
            _validArgs = Validate();
        }

        public bool ValidArguments
        {
            get { return _validArgs; }
        }

        public AnalysisJob GetAnalysisJob
        { 
            get 
            {
                if (_validArgs)
                    return _aj;
                else
                    return null;
            }
            
        }

        private bool Validate()
        {
            // the minimum number of arguments are 4:
            // -dumpfile and the name(s) of the dumpfile(s)
            // -out and the path where the report should be created
            if (_args.Length < 4 || !ParseArguments())
            {
                // If not enough arguments or the arguments cannot be parsed show the command line options;
                ErrorHandler.ShowCommandLineOptions();
                return false;
            }
            return true; 
        }

        private bool ParseArguments()
        {
            for (int i = 0; i < _args.Length; i += 2)
            {
                switch (_args[i].ToLower())
                {
                    case "-dumpfile":
                    case "-folder":
                        if (!ValidateDumpFiles(_args[i + 1]))
                            return false;
                        break;
                    case "-symbols":
                        _aj.Symbols = _args[i + 1];
                        break;
                    case "-rules":
                        string rulesString = _args[i + 1];
                        _aj.Rules = rulesString.Split(',');
                        break;
                    case "-out":
                        if (!FileValidator.Validate(_args[i + 1]))
                            return false;
                        _aj.ReportPath = _args[i + 1];
                        break;
                    case "-ruleassemblies":
                        string assemblylist = _args[i + 1];
                        if (!ValidateAssemblies(assemblylist))
                            return false;
                        break;
                    case "-showresults":
                        _aj.ShowResults = true;
                        i--;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Receives a list of dump file names with the path separated by commas, no spaces between the paths
        /// try to add the dumpnames to the list of dumps to be analyzed if the dumpname is invalid it wont be added
        /// </summary>
        /// <param name="dumpFilesPaths"></param>
        /// <returns>Boolean indicating if there is at least 1 valid file to process</returns>
        private bool ValidateDumpFiles(string dumpFilesPaths)
        {
            foreach (string dumpName in dumpFilesPaths.Split(','))
            {
                FileAttributes attr = File.GetAttributes(dumpName);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string dumpFiles = string.Join(",",Directory.GetFiles(dumpName, "*.dmp"));
                    ValidateDumpFiles(dumpFiles);
                }
                else
                    _aj.AddDumpFile(dumpName);
            }
            if (_aj.DumpFiles.Count > 0) return true; // we have at least one file to analyze, so we can continue 
            return false;
        }

        private bool ValidateAssemblies(string assembliesToLoad)
        {
            foreach (string assemblyName in assembliesToLoad.Split(','))
            {
                _aj.AddAssembly(assemblyName);
            }
            if (_aj.AssemblyNames.Count > 0 || _aj.Rules != null) return true; //no default rules specified and the assembly is invalid so we should not continue
            return false;
        }  
    }

}
