using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using text.tool.core.JsonToClassMapper;

namespace text.tool.core
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string nmSpace = "gip.web.core";
            string rootFolder = @"c:\Aleksandar\Development\linqpad-scripts\Handlebars.net\";
            string[] jsonFiles = new string[] { "gip.web.core.translation.json" };
            JsonToClassMapperService testService = new JsonToClassMapperService(nmSpace, rootFolder, jsonFiles);
        }
    }
}
