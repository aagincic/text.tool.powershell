using text.tool.core.EnumFromCsv;

namespace text.tool.core
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string sourceFolder = @"g:\My Drive\gipSoft\gip-soft notes\Partners\PanPek\Jupiter\csv\";
            string targetFolder = @"C:\Aleksandar\gipSoft\Source\my-projects\PanPekDemoData\DemoData\DataInfo\ColumnEnums\";
            string namespaceName = @"DemoData.DataInfo.ColumnEnums";

            CSVHeaderToEnumService cSVHeaderToEnumService = new CSVHeaderToEnumService(sourceFolder, targetFolder, namespaceName);
            string[] result = cSVHeaderToEnumService.DoWork();
        }
    }
}
