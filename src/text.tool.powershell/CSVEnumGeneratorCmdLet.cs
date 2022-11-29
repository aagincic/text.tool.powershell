using System;
using System.Management.Automation;
using text.tool.core.EnumFromCsv;

namespace texttool.powershell
{
    [Cmdlet(VerbsCommon.Set, CmdLetSettings.Const_CSVToEnum)]
    internal class CSVEnumGeneratorCmdLet : Cmdlet
    {
        #region ctor's
        public CSVEnumGeneratorCmdLet()
        {
        }

        #endregion

        #region Params

        #region Params -> Mandatory

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string NamespaceName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string SourceFolder { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string TargetFolder { get; set; }

        #endregion

        #endregion

        #region Methods
        protected override void ProcessRecord()
        {
            CSVHeaderToEnumService cSVHeaderToEnumService = new CSVHeaderToEnumService(SourceFolder, TargetFolder, NamespaceName);
            string[] result = cSVHeaderToEnumService.DoWork();
            foreach (string file in result)
                Console.WriteLine(@"{0} generated!", file);
        }
        #endregion
    }
}
