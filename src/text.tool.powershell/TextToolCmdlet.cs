using System.IO;
using System.Management.Automation;
using text.tool.core;
using text.tool.core.SQL;
using text.tool.core.Table;

namespace texttool.powershell
{
    [Cmdlet(VerbsCommon.Format, CmdLetSettings.Const_TextToolName)]
    public class TextToolCmdlet : Cmdlet
    {

        #region ctor's
        public TextToolCmdlet()
        {
        }

        #endregion

        #region Params

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public ToolEnum Tool { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string[] FileNames { get; set; }

        #endregion



        #region Process

        protected override void ProcessRecord()
        {
            string rootPath = Directory.GetCurrentDirectory();
            switch(Tool)
            {
                case ToolEnum.CleanUpSQLManagmentScript:
                    new CleanUpSQLManagmentScript(rootPath, FileNames);
                    break;
                case ToolEnum.TextToTable:
                    new TextToTable(FileNames);
                    break;
            }
        }
        #endregion
    }
}
