using System.IO;
using System.Linq;
using System.Management.Automation;
using text.tool.core.JsonToClassMapper;

namespace texttool.powershell
{
    [Cmdlet(VerbsCommon.Get, "JsonMapper")]
    public class JsonMapperCmdLet : Cmdlet
    {

        #region ctor's
        public JsonMapperCmdLet()
        {
        }

        #endregion

        #region Params


        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string NamespaceName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string RootFolder { get; set; }



        #endregion

        #region Methods
        protected override void ProcessRecord()
        {
            string [] jsonFiles = new DirectoryInfo(RootFolder).GetFiles("*.json").Select(c=>c.Name).ToArray();
            new JsonToClassMapperService(NamespaceName, RootFolder, jsonFiles);
        }
        #endregion
    }
}
