using BasicCore;
using System;
using System.Text;

namespace BatchRename
{
    public class RemovingAllSpaceRule : Rule, IRuleHandler
    {
        public string catchException()
        {
            return String.Empty;
        }

        public object Clone()
        {
            RemovingAllSpaceRule returnedObject = new RemovingAllSpaceRule();
            returnedObject.parameter = (TypedParameter)parameter.Clone();

            return returnedObject;
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
