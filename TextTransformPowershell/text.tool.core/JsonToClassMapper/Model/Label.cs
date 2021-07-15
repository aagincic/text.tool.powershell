using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.tool.core.JsonToClassMapper.Model
{
    public class Label
    {
        public string LabelNo { get; set; }
        public List<LabelTranslation> Label_LabelTranslations { get; set; }
    }
}
