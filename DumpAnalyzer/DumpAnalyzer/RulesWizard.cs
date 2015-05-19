using System;
using System.Collections.Generic;
using System.Text;
using DebugDiag.DotNet.AnalysisRules;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace DebugDiag.DumpAnalyzer
{
    class RulesWizard
    {
        private string[] _defaultRules = new string[] { "CrashHangAnalysis" };
        private List<AnalysisRuleInfo> _analysisRuleInfos = new List<AnalysisRuleInfo>();
        private List<RuleModule> _assemblies = new List<RuleModule>();
        private bool _ddv2InstallDirSearchAttempted;
        private string ddv2InstallDir;
        private Assembly _defaultAssembly = null;

        public RulesWizard(string[] rules, List<string> AssemblyNames)
        {
            if (rules == null) //Execute all rules on assemblies specified, or default rules if not assemblies
            {
                if (AssemblyNames.Count == 0) //Only default rules should be executed
                    AddDefaultRules(_defaultRules);
                else
                    LoadAssemblies(AssemblyNames); //Execute all rules found on assembly
            }
            else 
            {
                if (AssemblyNames.Count == 0) //No assemblies specified so look for default rules on default assembly that match the specified rules
                {
                    _defaultAssembly = GetDefaultAssembly();

                    if (_defaultAssembly != null)
                    {
                        foreach (string rule in rules)
                        {
                            if (IsDefaultRuleName(rule))
                                AddDefaultRule(_defaultAssembly, rule);
                            else
                                ErrorHandler.ReportError(String.Format("No assembly was specified for rule {0}, the rule will not be executed.", rule));
                        }
                        if (_analysisRuleInfos.Count > 0)
                            _assemblies.Add(new RuleModule(_defaultAssembly,true));
                    }
                }
                else
                {
                    ResolveRules(rules, AssemblyNames);
                }
            }
        }

        private void ResolveRules(string[] rules, List<string> AssemblyNames)
        {
            foreach (string rule in rules)
            {
                if (IsDefaultRuleName(rule))
                {
                    _defaultAssembly = GetDefaultAssembly();

                    if (_defaultAssembly != null)
                    {
                        AddDefaultRule(_defaultAssembly, rule);
                        _assemblies.Add(new RuleModule(_defaultAssembly, true));
                    }
                }
                else
                {
                    string assemblyName = FindAssemblyForRule(rule, AssemblyNames);
                    if (string.IsNullOrEmpty(assemblyName))
                        ErrorHandler.ReportError(String.Format("No assembly was found that contains a valid DebugDiag rule {0}, this rule will not be executed.", rule));
                }
            }
        }

        private string FindAssemblyForRule(string rule, List<string> AssemblyNames)
        {
            //If assemblies have been loaded find the assembly where the type is loaded
            //Since there should not be many assemblies or rule this loop will not be extensive but is not the optimal approach to find the assembly
            //A configuration file will help to match assemblies and rules to make this more efficient
            foreach (RuleModule asmbly in _assemblies)
            {
                Type ruleType = asmbly.Module.GetType(rule);
                if (ruleType != null)
                {
                    if (IsTypeAnAnalysisRule(ruleType))
                    {
                        _analysisRuleInfos.Add(new CodeAnalysisRuleInfo(ruleType, asmbly.Module.Location));
                        asmbly.RuleInList = true;
                        return asmbly.Module.Location;
                    }
                    else
                        ErrorHandler.PrintWarning("A Type that matches a rule name was found on Assembly {1} but could not be loaded because is not a valid Debugdiag Rule\r\n\tType:\t{0}", rule, asmbly.Module.Location);
                }
            }

            //If modules have been loaded the type was not found so return null, if no modules have been loaded the code below will load the modules and call this function recursively to return the value
            if (_assemblies.Count > 0 || AssemblyNames.Count == 0)
                return null;

            LoadAssemblies(AssemblyNames);

            return FindAssemblyForRule(rule, AssemblyNames);
        }

        private void LoadAssemblies(List<string> AssemblyNames)
        {
            List<string> _removeAssembly = new List<string>();

            foreach (string assemblyName in AssemblyNames)
            {
                try
                {
                    Assembly module = Assembly.LoadFile(assemblyName);
                    _assemblies.Add(new RuleModule(module, false));
                }
                catch (FileLoadException ex)
                {
                    ErrorHandler.PrintExceptionError("Failed to load {0}, rules in this assembly will not be executed", assemblyName, ex.ToString());
                    _removeAssembly.Add(assemblyName);
                }
                catch (BadImageFormatException ex)
                {
                    ErrorHandler.PrintExceptionError("Failed to load {0}, rules in this assembly will not be executed", assemblyName, ex.ToString());
                    _removeAssembly.Add(assemblyName);
                }
                catch (FileNotFoundException ex)
                {
                    ErrorHandler.PrintExceptionError("File {0} was not found, rules in this assembly will not be executed", assemblyName, ex.ToString());
                    _removeAssembly.Add(assemblyName);
                }
                catch (Exception ex)
                {
                    ErrorHandler.PrintExceptionError("Unexpected exception loading {0}", assemblyName, ex.ToString());
                    _removeAssembly.Add(assemblyName);
                }
            }

            //remove assemblies that failed to load since we will not be able to use them anyway
            foreach (string assemblyName in _removeAssembly)
                AssemblyNames.Remove(assemblyName);
        }

        private void AddDefaultRules(string[] rules)
        {
            
            Assembly ruleAssembly = GetDefaultAssembly();

            if (ruleAssembly != null)
            {
                foreach (string rule in rules)
                    AddDefaultRule(ruleAssembly, rule);
                _assemblies.Add(new RuleModule(ruleAssembly, true));
            }
        }

        private void AddDefaultRule(Assembly ruleAssembly, string rule)
        {
            Type ruleType = null;

            switch (rule.ToLower())
            {
                case "crashhanganalysis":
                    ruleType = ruleAssembly.GetType("DebugDiag.AnalysisRules.CrashHangAnalysis");
                    break;
                case "dotnetmemoryanalysis":
                    ruleType = ruleAssembly.GetType("DebugDiag.AnalysisRules.DotNetMemoryAnalysis");
                    break;
                case "memoryanalysis":
                    ruleType = ruleAssembly.GetType("DebugDiag.AnalysisRules.MemoryAnalysis");
                    break;
                case "perfanalysis":
                    ruleType = ruleAssembly.GetType("DebugDiag.AnalysisRules.PerfAnalysis");
                    break;
                case "clrperfanalysis":
                    ruleType = ruleAssembly.GetType("DebugDiag.AnalysisRules.ClrPerfAnalysis");
                    break;
            }
            if (ruleType != null)
                _analysisRuleInfos.Add(new CodeAnalysisRuleInfo(ruleType, null));
            else
                ErrorHandler.PrintExceptionError("Type could not be loaded\r\n\tType:\t{0}",
                    rule, Path.Combine(DDv2InstallDir(), "AnalysisRules", "DebugDiag.AnalysisRules.dll").ToString());
        }

        private string GetDefaultAssemblyFullPath()
        {
            return Path.Combine(DDv2InstallDir(), "AnalysisRules", "DebugDiag.AnalysisRules.dll");
        }

        private Assembly GetDefaultAssembly()
        {
            if (_defaultAssembly == null)
            {
                try
                {
                    _defaultAssembly = Assembly.LoadFile(GetDefaultAssemblyFullPath());
                    if (_defaultAssembly != null) return _defaultAssembly;
                }
                catch (FileLoadException ex)
                {
                    ErrorHandler.PrintExceptionError("Failed to load {0}, default rules will not be executed", GetDefaultAssemblyFullPath(), ex.ToString());
                }
                catch (FileNotFoundException ex)
                {
                    ErrorHandler.PrintExceptionError("File not found {0}, default rules will not be executed", GetDefaultAssemblyFullPath(), ex.ToString());
                }
                return null;
            }
            else
                return _defaultAssembly;
        }

        public static bool IsDefaultRuleInList(string[] rules)
        {
            foreach (string rule in rules)
                if (IsDefaultRuleName(rule)) return true;
            return false;
        }

        public static bool IsDefaultRuleName(string ruleName)
        {
            switch (ruleName.ToLower())
            {
                case "crashhanganalysis":
                    return true;
                case "dotnetmemoryanalysis":
                    return true;
                case "memoryanalysis":
                    return true;
                case "perfanalysis":
                    return true;
                case "clrperfanalysis":
                    return true;
            }
            return false;
        }

        public List<AnalysisRuleInfo> AnalysisRulesInfos
        { 
            get 
            {
                return _analysisRuleInfos;
            }
        }

        public List<Assembly> Modules
        {
            get
            {
                List<Assembly> Result = new List<Assembly>();

                if (_assemblies.Count == 0)
                    return Result;

                foreach (RuleModule module in _assemblies)
                {
                    if (!module.RuleInList)
                        Result.Add(module.Module);
                }

                return Result;
            }
        }

        private string GetDefaultRegVal(string keyName)
        {
            return (string)Registry.GetValue(keyName, "", string.Empty);
        }


        private string DDv2InstallDir()
        {
            if (!_ddv2InstallDirSearchAttempted)
            {
                string keyName = @"HKEY_CLASSES_ROOT\DbgLib.DbgControl\CLSID";
                string regVal = GetDefaultRegVal(keyName);

                try
                {
                    if (regVal != string.Empty)
                    {
                        keyName = string.Format(@"HKEY_CLASSES_ROOT\CLSID\{0}\InprocServer32", regVal);
                        regVal = GetDefaultRegVal(keyName);
                        if (!string.IsNullOrEmpty(regVal))
                        {
                            ddv2InstallDir = Path.GetDirectoryName(regVal);
                            _ddv2InstallDirSearchAttempted = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("An exception occurred while finding the v2 analysis runtime.\r\n\tKeyPath:  {0}\r\n\tValueName:  {1}\r\n\tMessage:  {2}\r\nStack Trace:\r\n{3}",
                        keyName, regVal, ex.Message, ex.StackTrace);
                    ErrorHandler.ReportError(msg);
                }
            }

            return ddv2InstallDir;
        }

        private static bool IsTypeAnAnalysisRule(Type ruleInstanceType)
        {
            // Specify the TypeFilter delegate that compares the interfaces against filter criteria.

            // check to see if the type implements any of the analysis rule interfaces (except the base IAnalysisRuleMetadata, that's not good enough)
            for (int i = 0; i < _possibleAnalysisRuleInterfacesList.Length; i++)
            {
                Type[] myInterfaces = ruleInstanceType.FindInterfaces(_analysisRuleInterfaceFilter, _possibleAnalysisRuleInterfacesList[i]);
                if (myInterfaces.Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static String[] _possibleAnalysisRuleInterfacesList = new String[4]  {
                                                                            "DebugDiag.DotNet.AnalysisRules.IExceptionThreadRule",
                                                                            "DebugDiag.DotNet.AnalysisRules.IHangDumpRule",
                                                                            "DebugDiag.DotNet.AnalysisRules.IHangThreadRule",
                                                                            "DebugDiag.DotNet.AnalysisRules.IMultiDumpRule",
                                                                        };

        static TypeFilter _analysisRuleInterfaceFilter = new TypeFilter(AnalysisRuleInterfaceFilter);

        private static bool AnalysisRuleInterfaceFilter(Type ruleInstanceTypeObj, Object criteriaObj)
        {
            if (ruleInstanceTypeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        private class RuleModule
        {
            public Assembly Module;
            public bool RuleInList;

            public RuleModule(Assembly module, bool onList)
            {
                Module = module;
                RuleInList = onList;
            }

        }
    }
}
