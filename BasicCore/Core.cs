using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace BasicCore
{
    public interface IRuleHandler:ICloneable
    {
        public bool isEditable();

        public string getRuleType();
        public void setParameter(TypedParameter parameter);
     
        public TypedParameter getParameter();
     

        public string Handle(string fileName, bool isFileType);

        public void HandleInputParameter();
   

        public string catchException();
 
    }
    public class Rule
    {
        public TypedParameter parameter = new TypedParameter();
    }
    public class TypedParameter:ICloneable
    {
        public List<string> inputStrings { get; set; }
        
        public string outputString { get; set; }

        public TypedParameter()
        {
            inputStrings = new List<string>();
            outputString = "";
        }

        public object Clone()
        {
            TypedParameter returnedObject = new TypedParameter();
            returnedObject.outputString=outputString;
            int index = 0;
            foreach(var element in inputStrings)
            {
                returnedObject.inputStrings.Add(element);
                index++;
            }
            return returnedObject;
        }
    }

}
