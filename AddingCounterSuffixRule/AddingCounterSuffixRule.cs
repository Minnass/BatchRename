using BasicCore;
using System;
using System.Linq;
using System.Text;

namespace BatchRename
{
    public class AddingCounterSuffixRule : Rule, IRuleHandler
    {
        private const int DIGITS_POSTION = 0;
        private const int START_INDEX_POSITION = 1;
        private const int STEP_POSTION = 2;

        private int startIndex;
        private int step;
        private int numberOfDigits;
        private int nextIndex;

        public object Clone()
        {
            AddingCounterSuffixRule returnedObject = new AddingCounterSuffixRule();
            returnedObject.parameter =(TypedParameter)parameter.Clone();
            returnedObject.startIndex=startIndex;
            returnedObject.step=step;
            returnedObject.numberOfDigits=numberOfDigits;   
            returnedObject.nextIndex=nextIndex;
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
            return "AddingCounterSuffixRule";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return"";
            }
            var builder = new StringBuilder();
            string tempOfIntToString=nextIndex.ToString();
            int numberOfZeros = numberOfDigits - tempOfIntToString.Length;
            if(isFileType)
            {
                string[] subStrings = fileName.Split(".", StringSplitOptions.None);
                builder.Append(subStrings[0]);
      
                builder.Append('0', numberOfZeros);
                builder.Append(nextIndex.ToString());
                builder.Append(".");
                builder.Append(subStrings[1]);
            }
            else
            {
                builder.Append(fileName);
         
                builder.Append('0', numberOfZeros);
                builder.Append(nextIndex.ToString());
            }
            nextIndex+=step; 
            string result=builder.ToString();
            return result;
        }

        public void HandleInputParameter()
        {
            numberOfDigits = int.Parse(parameter.inputStrings.ElementAt(DIGITS_POSTION));
            startIndex = int.Parse(parameter.inputStrings.ElementAt(START_INDEX_POSITION));
            step = int.Parse(parameter.inputStrings.ElementAt(STEP_POSTION));
            nextIndex = startIndex;
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
