namespace text.tool.core.Table
{
    public class TableConversionSettings
    {
        public TableConversionSettings()
        {
            CSVDelimiter = '\t';
            HorizontalLineChar = '-';
            VerticalLineChar = '|';
            TitleLineChar = '=';
            PadLeft = 1;
            PadRight = 1;
            HaveTitle = true;

            FileSuffix = "-formatted";
        }


        public char CSVDelimiter { get; set; }
        public bool HaveTitle { get; set; }
        public char HorizontalLineChar { get; set; }
        public char VerticalLineChar { get; set; }
        public char TitleLineChar { get; set; }
        public int PadLeft { get; set; }
        public int PadRight { get; set; }

        public string FileSuffix { get; set; }
    }
}
