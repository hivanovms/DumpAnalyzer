using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DebugDiag.DumpAnalyzer
{
    /// <summary>
    /// Validates correctness of a path and checks if the path is accessible or not by the application
    /// </summary>
    public static class FileValidator
    {

        public static bool Validate(string pathValue, bool mustExist = false)
        {
            try
            {
                FileInfo fi = new System.IO.FileInfo(pathValue);
                if (mustExist)
                {
                    if (!fi.Exists)
                    {
                        ErrorHandler.ReportError(string.Format("{0} does not exists", pathValue));
                        return false;
                    }
                }
                return true;
            }
            catch (ArgumentException)
            {
                ErrorHandler.ReportError(string.Format("{0} is not valid.", pathValue));
                return false;
            }
            catch (System.IO.PathTooLongException)
            {
                ErrorHandler.ReportError(string.Format("The name {0} is too long to be parsed, try renaming it", pathValue));
                return false;
            }
            catch (System.UnauthorizedAccessException)
            {
                ErrorHandler.ReportError(string.Format("You are not authorized to access {0}", pathValue));
                return false;
            }
            catch (NotSupportedException)
            {
                ErrorHandler.ReportError(string.Format("The path {0} generated an unsupported Exception", pathValue));
                return false;
            }

        }
    }
}
