using BasicCore;
using System;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{
    public class ConvertingPascalcaseRule : Rule, IRuleHandler
    {

        public object Clone()
        {
            ConvertingPascalcaseRule returnedObject = new ConvertingPascalcaseRule();
            returnedObject.setParameter((TypedParameter)parameter.Clone());
            return returnedObject;
        }

        public void Done()
        {
            //do nothing
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
            return "ConvertingPascalcaseRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            var builder = new StringBuilder();

            if (isFileType)
            {
                string[] subStrings = fileName.Split(".", StringSplitOptions.None);
                builder.Append(subStrings[0].ToUpper());
                builder.Append(".");
                builder.Append(subStrings[1]);
            }
            else
            {
                builder.Append(fileName.ToUpper());
            }
            string result = builder.ToString();
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
            return "Converting Pascal Case Rule";
        }
    }
}
