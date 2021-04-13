using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.tool.core.SQL
{
    public class CleanUpSQLManagmentScript
    {

        #region const's 
        public string[] avoidRowsStartWith = new string[] { "GO", "COMMIT", "BEGIN", "SET" };
        public string[] avoidRowsContains = new string[] { "LOCK_ESCALATION" };
        public string[] newLineRows = new string[] { "ALTER", "CREATE", "DROP TABLE", "DELETE", "IF EXISTS", "EXEC" };
        public string OutputSuffix = "-processed";
        #endregion


        #region ctor's

        public CleanUpSQLManagmentScript( string inputFile)
        {
            ProcessFile(Path.Combine(inputFile));
        }


        public CleanUpSQLManagmentScript(string rootDir, string[] inputFiles)
        {
            foreach (string inputFile in inputFiles)
                ProcessFile(inputFile);
        }


        #endregion

        #region Helpers

        public void ProcessFile(string inputFile)
        {
            string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + OutputSuffix +
                Path.GetExtension(inputFile));
            using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate))
                {

                    using (StreamReader sr = new StreamReader(inputStream))
                    {
                        string line = null;
                        using (StreamWriter sw = new StreamWriter(outputStream))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                bool includeRow = !StartsWith(line, avoidRowsStartWith) && !Contains(line, avoidRowsContains);
                                if (includeRow)
                                {
                                    string processed = line.Trim();
                                    if (StartsWith(processed, newLineRows))
                                    {
                                        sw.Write(";");
                                        sw.Write(Environment.NewLine);
                                    }

                                    else
                                        processed = " " + processed;
                                    if (processed.EndsWith(","))
                                        processed += " ";
                                    sw.Write(processed);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool StartsWith(string line, string[] startPatternItems)
        {
            bool isStartWith = false;
            foreach (var element in startPatternItems)
            {
                if (line.StartsWith(element))
                {
                    isStartWith = true;
                    break;
                }
            }
            return isStartWith;
        }

        public bool Contains(string line, string[] startPatternItems)
        {
            bool isStartWith = false;
            foreach (var element in startPatternItems)
            {
                if (line.Contains(element))
                {
                    isStartWith = true;
                    break;
                }
            }
            return isStartWith;
        }
        #endregion
    }
}
