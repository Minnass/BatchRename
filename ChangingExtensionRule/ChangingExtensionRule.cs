using BasicCore;
using System;
using System.Linq;
using System.Text;

namespace BatchRename
{
    public class ChangingExtensionRule : Rule, IRuleHandler
    {
        private const int NEW_EXTENSION_POSTION = 0;
        private string newExtension;
        private const string ERROR = "Cannot change Extension Rule for folder";
        public object Clone()
        {
            ChangingExtensionRule returnedObject = new ChangingExtensionRule();
            returnedObject.parameter = (TypedParameter)parameter.Clone();
            returnedObject.newExtension = newExtension;
            return returnedObject;
        }

        public TypedParameter getParameter()
        {
            return parameter;
        }
        public void setParameter(TypedParameter parameter)
        {
            this.parameter = parameter;
            HandleInputParameter();
        }

        public string getRuleType()
        {
            return "ChangingExtensionRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if (string.IsNullOrEmpty(fileName)||!isFileType)
            {
                return "";
            }
            string[] subStrings = fileName.Split(".", StringSplitOptions.None);
            var builder = new StringBuilder();
            builder.Append(subStrings[0]);
            builder.Append(".");
            builder.Append(newExtension);
            string result = builder.ToString();
            return result;
        }

        public void HandleInputParameter()
        {
            newExtension = parameter.inputStrings.ElementAt(NEW_EXTENSION_POSTION);
        }

        public bool isEditable()
        {
            return true;
        }
        public string catchException()
        {
            return ERROR;
        }
        public override string ToString()
        {
            return getRuleType();
        }

    }
}
