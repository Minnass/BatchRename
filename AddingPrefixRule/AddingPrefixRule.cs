using BasicCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BatchRename
{
    public class AddingPrefixRule :Rule, IRuleHandler
    {
        private const int PREFIX_PARAMETER_POSTION = 0;
        private string prefixParameter;
        public object Clone()
        {
            AddingPrefixRule returnedObject = new AddingPrefixRule();
            returnedObject.prefixParameter = prefixParameter;
            returnedObject.parameter = (TypedParameter)parameter.Clone();
            return returnedObject;
        }

        public TypedParameter getParameter()
        {
            return parameter;
        }

        public void  setParameter(TypedParameter parameter)
        {
            this.parameter = parameter;
            HandleInputParameter();
        }

        public string getRuleType()
        {
            return "AddingPrefixRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            var builder = new StringBuilder();
            builder.Append(prefixParameter);
            builder.Append(fileName);
            string result = builder.ToString();
            return result;
        }

        public void HandleInputParameter()
        {
            prefixParameter = parameter.inputStrings.ElementAt(PREFIX_PARAMETER_POSTION);
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
