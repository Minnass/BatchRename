using BasicCore;
using System;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace BatchRename
{
    public class AddingSuffixRule : Rule, IRuleHandler
    {

        private const int SUFFIX_PARAMETER_POSITION = 0;
        private string suffixParameter;

        public object Clone()
        {
            AddingSuffixRule returnedObject = new AddingSuffixRule();
            returnedObject.suffixParameter = suffixParameter;
            returnedObject.parameter = (TypedParameter)parameter.Clone();
            return returnedObject;

        }
        public AddingSuffixRule()
        {
            suffixParameter = "";
        }

        public TypedParameter getParameter()
        {
            return this.parameter;

        }

        public string getRuleType()
        {
            return "AddingSuffixRule";
        }

        public void setParameter(TypedParameter parameter)
        {
            this.parameter = parameter;
            HandleInputParameter();
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
                string[] substrings = fileName.Split(".", StringSplitOptions.None);
                builder.Append(substrings[0]);
                builder.Append(suffixParameter);
                builder.Append(".");
                builder.Append(substrings[1]);
            }

            else
            {
                builder.Append(fileName);
                builder.Append(suffixParameter);
            }
            string result = builder.ToString();
            return result;

        }

        public void HandleInputParameter()
        {
            suffixParameter = parameter.inputStrings.ElementAt(SUFFIX_PARAMETER_POSITION);
        }

        public bool isEditable()
        {
            return true;
        }

        public string catchException()
        {
            return String.Empty;
        }
        public override string ToString()
        {
            return getRuleType();
        }
    }
}
