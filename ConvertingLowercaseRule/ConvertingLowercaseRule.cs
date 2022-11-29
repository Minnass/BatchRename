using BasicCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{
    public class ConvertingLowercaseRule : Rule, IRuleHandler
    {

        public object Clone()
        {
            ConvertingLowercaseRule returnedObject=new ConvertingLowercaseRule();
            returnedObject.setParameter((TypedParameter)parameter.Clone());
            return returnedObject;
        }

        public void Done()
        {
            // do nothing
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
            return "ConvertingLowercaseRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            var builder = new StringBuilder();

            if(isFileType)
            {
                string[] subStrings = fileName.Split(".", StringSplitOptions.None);
                builder.Append(subStrings[0].ToLower());
                builder.Append(".");
                builder.Append(subStrings[1]);
            }
            else
            {
                builder.Append(fileName.ToLower());
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
           //do nothing heres
        }
        public override string ToString()
        {
            return "Converting Lower Case Rule";
        }
    }
}
