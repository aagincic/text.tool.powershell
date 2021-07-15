using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.tool.core.JsonToClassMapper.Model
{
    public class ClassModel
    {
        public string FileName { get;set;}
        public string ClassName { get; set; }
        public string NameSpaceName { get; set; }

        public List<PropertyModel> Properties { get; set; }

        public string GeneratedClassContent { get;set;}
    }
}
