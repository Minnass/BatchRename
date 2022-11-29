using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BasicCore
{
    public interface IRuleHandler : ICloneable
    {
        public bool isEditable();

        public string getRuleType();
        public void setParameter(TypedParameter parameter);

        public TypedParameter getParameter();


        public string Handle(string fileName, bool isFileType);

        public void HandleInputParameter();


        public void getEditor(StackPanel frame);

        public string raiseParameterInputError();

        public void Done();
    }
 

    public class Rule
    {
        public TypedParameter parameter = new TypedParameter();
    }

    public class RuleJsonFormat
    {
        public string RuleType { get; set; }
        public List<string> InputStrings { get; set; }
    }
    public class PresentedItemJsonFormat
    {
        public string currentName { get; set; }
        public string newName { get; set; }
        public string path { get; set; }
        public string error { get; set; }



    }
    public class ProjectJsonFormat
    {
        public string type { get; set; }
        public List<RuleJsonFormat> ruleJsonFormats { get; set; }
        public List<PresentedItemJsonFormat> presentedJsonFormat { get; set; }
        public ProjectJsonFormat()
        {
            type = String.Empty;
            ruleJsonFormats = new List<RuleJsonFormat>();
            presentedJsonFormat = new List<PresentedItemJsonFormat>();
        }


    }
    public class TypedParameter : ICloneable
    {
        public List<string> inputStrings { get; set; }


        public TypedParameter()
        {
            inputStrings = new List<string>();
        }

        public object Clone()
        {
            TypedParameter returnedObject = new TypedParameter();
            int index = 0;
            foreach (var element in inputStrings)
            {
                returnedObject.inputStrings.Add(element);
                index++;
            }
            return returnedObject;
        }
    }
}
