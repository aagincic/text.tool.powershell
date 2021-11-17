using HandlebarsDotNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using text.tool.core.EnumFromCsv.Model;

namespace text.tool.core.EnumFromCsv
{
    public class CSVHeaderToEnumService
    {

        #region const
        public const string Const_CSV_FileExtension = ".csv";
        public const string Const_CS_FileExtension = ".cs";
        public const string Const_TemplateName = "csv-enum-template.hbs";
        public const string Const_CSV_EnumSuffix = "Enum";
        #endregion

        #region DI
        public string SourceFolder { get; private set; }
        public string TargetFolder { get; private set; }
        public string NamespaceName { get; private set; }
        #endregion

        #region ctor's
        public CSVHeaderToEnumService(string sourceFolder, string targetFolder, string namespaceName)
        {
            SourceFolder = sourceFolder;
            TargetFolder = targetFolder;
            NamespaceName = namespaceName;
        }
        #endregion


        #region Mehtods

        public string[] DoWork()
        {
            List<string> files = new List<string>();
            if (Directory.Exists(SourceFolder) && Directory.Exists(TargetFolder))
            {
                string[] csvFiles = GetCsvFiles();
                foreach (string csvFile in csvFiles)
                {
                    string[] headers = GetCsvHeader(csvFile);
                    CSVEnumModel cSVEnumModel = GetCSVEnumModel(NamespaceName, csvFile, headers);
                    string targetFile = GetTargetFile(csvFile);
                    WriteTargetFile(cSVEnumModel, targetFile);
                    files.Add(Path.GetFileName(targetFile));
                }
            }
            return files.ToArray();
        }

        #endregion


        #region Helper methods

        private string GetTargetFile(string inputFile)
        {
            return Path.Combine(TargetFolder, Path.GetFileNameWithoutExtension(inputFile) + Const_CSV_EnumSuffix + Const_CS_FileExtension);
        }

        private void WriteTargetFile(CSVEnumModel cSVEnumModel, string fileName)
        {
            string templateContent = GetTemplateContent();
            HandlebarsTemplate<object, object> template = Handlebars.Compile(templateContent);
            string content = template(cSVEnumModel);
            File.WriteAllText(fileName, content);
        }

        private CSVEnumModel GetCSVEnumModel(string namespaceName, string fileName, string[] columns)
        {
            CSVEnumModel cSVEnumModel = new CSVEnumModel();
            cSVEnumModel.NameSpaceName = namespaceName;
            cSVEnumModel.EnumName = Path.GetFileNameWithoutExtension(fileName) + Const_CSV_EnumSuffix;
            cSVEnumModel.EnumValues = columns;
            return cSVEnumModel;
        }

        private string[] GetCsvHeader(string csvFile)
        {
            string line = "";
            using (FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs);
                line = sr.ReadLine();
            }

            List<string> columns = line.Split(';').ToList();
            if (columns.Any() && columns.Count > 1)
                for (int i = 0; i < columns.Count - 1; i++)
                {
                    columns[i] = columns[i] + ",";
                }
            return columns.ToArray();
        }

        private string[] GetCsvFiles()
        {
            DirectoryInfo di = new DirectoryInfo(SourceFolder);
            return di.EnumerateFiles().Where(c => c.Extension == ".csv").Select(c => c.FullName).ToArray();
        }

        private string GetTemplateContent()
        {
            string result = "";
            string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith(Const_TemplateName));
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        #endregion
    }
}
