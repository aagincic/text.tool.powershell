using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace text.tool.core.Table
{
    public class TextToTable
    {
        #region properties
        public List<List<string>> ReadedContent { get; private set; }
        public Dictionary<int, int> Metrics { get; private set; }
        public TableConversionSettings Settings { get; private set; }
        public int SummaryLineCharNumber { get; private set; }
        #endregion

        #region ctor's
        public TextToTable()
        {
            ReadedContent = new List<System.Collections.Generic.List<string>>();
            Metrics = new Dictionary<int, int>();
            Settings = new TableConversionSettings();
        }


        public TextToTable(string[] inputFiles)
        {
            Settings = new TableConversionSettings();

            foreach(string inputFile in inputFiles)
            {
                ReadedContent = new List<System.Collections.Generic.List<string>>();
                Metrics = new Dictionary<int, int>();

                string outputFile =  Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + Settings.FileSuffix +
                Path.GetExtension(inputFile));

                LoadContentFromFile(inputFile);
                ProcessContent();
                WriteContentToFile(outputFile);
            }
        }
        #endregion

        #region loading part
        public void LoadContentFromFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                LoadContent(fs);
            }
        }

        public void LoadContentFromString(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                LoadContent(stream);
            }
        }

        public void LoadContent(Stream st)
        {
            string line = "";
            using (StreamReader sr = new StreamReader(st, Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> tmpLine = line.Split(Settings.CSVDelimiter).Where(c => !string.IsNullOrEmpty(c)).ToList();
                    ReadedContent.Add(tmpLine);
                }
            }
        }

        #endregion

        public void ProcessContent()
        {
            int columnsCount = ReadedContent.FirstOrDefault().Count();
            if (ReadedContent.Any(c => c.Count() != columnsCount))
            {
                throw new Exception("Column counts not match in all rows readed content!");
            }
            Metrics = ReadedContent.FirstOrDefault().ToDictionary(c => ReadedContent.FirstOrDefault().IndexOf(c), c => 0);
            foreach (List<string> row in ReadedContent)
            {
                for (int i = 0; i < columnsCount; i++)
                {
                    if (row[i].Trim().Length > Metrics[i])
                    {
                        Metrics[i] = row[i].Trim().Length;
                    }
                }
            }
            SummaryLineCharNumber = Metrics.Sum(c => c.Value);
            SummaryLineCharNumber += columnsCount * Settings.PadLeft;
            SummaryLineCharNumber += columnsCount * Settings.PadRight;
            if (Settings.VerticalLineChar != '\0')
            {
                SummaryLineCharNumber += columnsCount + 1;
            }
        }

        #region output part

        public void WriteContent(Stream st)
        {
            string line = "";
            using (StreamWriter sw = new StreamWriter(st, Encoding.UTF8, 1024, true))
            {
                foreach (List<string> row in ReadedContent)
                {
                    int rowIndex = ReadedContent.IndexOf(row);
                    if (Settings.TitleLineChar != '\0' && rowIndex == 0)
                    {
                        line = new String(Settings.TitleLineChar, SummaryLineCharNumber);
                        sw.WriteLine(line);
                        line = null;
                    }
                    foreach (string element in row)
                    {
                        int columnIndex = row.IndexOf(element);
                        if (columnIndex == 0)
                        {
                            line += Settings.VerticalLineChar;
                        }
                        int tmpPadRight = Metrics[columnIndex] + Settings.PadRight - element.Trim().Length;
                        line += new String(' ', Settings.PadLeft) + element.Trim() + new String(' ', tmpPadRight);
                        line += Settings.VerticalLineChar;
                    }
                    sw.WriteLine(line);
                    line = null;

                    if (Settings.TitleLineChar != '\0' && rowIndex == 0)
                    {
                        line = new String(Settings.TitleLineChar, SummaryLineCharNumber);
                        sw.WriteLine(line);
                        line = null;
                    }

                    if (Settings.HorizontalLineChar != '\0' && rowIndex != 0)
                    {
                        line = new String(Settings.HorizontalLineChar, SummaryLineCharNumber);
                        sw.WriteLine(line);
                        line = null;
                    }
                }
            }
        }


        public void WriteContentToFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                WriteContent(fs);
            }
        }

        public string WriteContentToString()
        {
            string content = "";
            using (MemoryStream ms = new MemoryStream())
            {
                WriteContent(ms);
                ms.Flush();
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }

        #endregion
    }
}
