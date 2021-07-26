using HandlebarsDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using text.tool.core.JsonToClassMapper.Model;

namespace text.tool.core.JsonToClassMapper
{
    public class JsonToClassMapperService
    {

        #region const
        public const string Const_ReservedKeywords =
@"abstract
as
base
bool
break
byte
case
catch
char
checked
class
const
continue
decimal
default
delegate
do
double
event
explicit
extern
false
finally
fixed
float
for
foreach
goto
if
implicit
in
int
interface
internal
is
lock
new
null
object
operator
out
override
params
private
protected
public
readonly
ref
return
sbyte
sealed
short
sizeof
stackalloc
struct
switch
this
throw
true
try
typeof
uint
ulong
unchecked
unsafe
ushort
using
virtual
void
volatile
new";

        public const string Const_Suffix = @".translation.json";
        #endregion


        #region DI
        public string RootFolder { get; private set; }
        public List<ClassModel> ClassModels { get; private set; }

        public string TemplateContent { get; private set; }
        public HandlebarsTemplate<object, object> Template { get; private set; }
        #endregion

        #region ctor's
        public JsonToClassMapperService(string rootFolder, string[] jsonFiles, string nmSpace = null, string prefix = null, string suffix = null)
        {
            RootFolder = rootFolder;
            if (string.IsNullOrEmpty(suffix))
                suffix = Const_Suffix;

            TemplateContent = GetTemplateContent();
            Template = Handlebars.Compile(TemplateContent);

            ClassModels = new List<ClassModel>();
            foreach (var jsonFile in jsonFiles)
            {
                NameSpaceModel nameSpaceModel = GetNameSpaceModel(nmSpace, prefix, jsonFile, suffix);
                ClassModel classModel = GetClassModel(jsonFile, nameSpaceModel);
                ClassModels.Add(classModel);
            }

            foreach (ClassModel classModel in ClassModels)
            {
                string fileName = Path.Combine(RootFolder, classModel.FileName + ".cs");
                File.WriteAllText(fileName, classModel.GeneratedClassContent);
            }
        }

        private NameSpaceModel GetNameSpaceModel(string nmSpace, string prefix, string jsonFile, string sufix)
        {
            NameSpaceModel nameSpaceModel = new NameSpaceModel()
            {
                NameSpace = nmSpace,
                Prefix = prefix
            };
            if (!string.IsNullOrEmpty(nameSpaceModel.NameSpace))
            {
                if (string.IsNullOrEmpty(nameSpaceModel.Prefix))
                    nameSpaceModel.Prefix = nmSpace;
            }
            else
            {
                string fileName = jsonFile;
                fileName = fileName.Replace(sufix, "");
                string[] parts = fileName.Split('.');
                if (parts.Length > 2)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (i < 3)
                        {
                            nameSpaceModel.NameSpace += parts[i];
                            nameSpaceModel.NameSpace += ".";
                        }
                        else
                        {
                            nameSpaceModel.Prefix += parts[i];
                            nameSpaceModel.Prefix += ".";
                        }
                    }
                    nameSpaceModel.NameSpace = nameSpaceModel.NameSpace.TrimEnd('.');
                    nameSpaceModel.Prefix = nameSpaceModel.NameSpace + "." + nameSpaceModel.Prefix.TrimEnd('.');
                }
            }
            return nameSpaceModel;
        }

        private string GetTemplateContent()
        {
            string result = "";
            string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("translation-template.hbs"));
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private ClassModel GetClassModel(string jsonFile, NameSpaceModel nameSpaceModel)
        {
            ClassModel classModel = new ClassModel();
            classModel.FileName = Path.GetFileNameWithoutExtension(jsonFile);
            classModel.NameSpaceName = nameSpaceModel.NameSpace + ".JsonTranslation";
            classModel.ClassName = classModel.FileName.Replace(".", "_");

            List<Label> labels = GetLabels(Path.Combine(RootFolder, jsonFile));
            classModel.Properties = GetPropertyModel(nameSpaceModel.Prefix, labels);
            GenerateClassTemplate(classModel);
            return classModel;
        }

        private void GenerateClassTemplate(ClassModel classModel)
        {
            classModel.GeneratedClassContent = Template(classModel);
        }
        #endregion

        #region Methods


        private List<Label> GetLabels(string fileName)
        {
            string jsonContent = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<List<Label>>(jsonContent);
        }

        private List<PropertyModel> GetPropertyModel(string prefix, List<Label> labels)
        {
            return labels.Select(c => new PropertyModel()
            {
                PropertyName = GetPropertyName(c.LabelNo, prefix),
                PropertyValue = c.LabelNo
            }).ToList();
        }

        private string GetPropertyName(string labelNo, string prefix)
        {
            string propertyName = labelNo.Replace(prefix + ".", "");
            if (Const_ReservedKeywords.Contains(propertyName))
                propertyName = "_" + propertyName;
            propertyName = propertyName.Replace(".", "_");
            return propertyName;
        }
        #endregion
    }
}
