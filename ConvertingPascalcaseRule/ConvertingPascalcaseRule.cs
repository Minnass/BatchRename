using BasicCore;
using System;
using System.Text;

namespace BatchRename
{
    public class ConvertingPascalcaseRule : Rule, IRuleHandler
    {
        public string catchException()
        {
            return String.Empty;
        }

        public object Clone()
        {
            ConvertingPascalcaseRule returnedObject = new ConvertingPascalcaseRule();
            returnedObject.parameter = (TypedParameter)parameter;
            return returnedObject;
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
            throw new NotImplementedException();
        }

        public bool isEditable()
        {
            return false;
        }

        public void setParameter(TypedParameter parameter)
        {
            //do nothing
        }
        public override string ToString()
        {
            return getRuleType();
        }
    }
}
