using BasicCore;
using System;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{

    

    public class RemovingAllSpaceRule : Rule, IRuleHandler
    {

        public object Clone()
        {
            RemovingAllSpaceRule returnedObject = new RemovingAllSpaceRule();
            returnedObject.setParameter((TypedParameter)parameter.Clone());
            return returnedObject;
        }

        public void Done()
        {
          //Do nothing
        }

        public void getEditor(StackPanel frame)
        {
            //do nothing
        }

        public TypedParameter getParameter()
        {
            return parameter;   
        }

        public string getRuleType()
        {
            return "RemovingAllSpaceRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            var builder = new StringBuilder();
            foreach(var character in fileName)
            {
                if(character!=' ')
                {
                    builder.Append(character);
                }
            }
            string result=builder.ToString();      
            return result;
        }

        public void HandleInputParameter()
        {
         //do nothing
        }

        public bool isEditable()
        {
           return false;
        }

        public string raiseParameterInputError()
        {
           return String.Empty;
        }

        public void setParameter(TypedParameter parameter)
        {
           //do nothing
        }
        public override string ToString()
        {
            return "Removing All Space Rule";
        }
    }
}
