using BasicCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchRename
{

    
    public class ReplacingCharacterRule : Rule, IRuleHandler
    {
        private const int NEW_CHARACTER_POSTION = 0;
        private const int TOTAL_REPLACED_CHARACTERS_POSITION = 1;
        private List<string> replacedCharacters;
        private string newCharacter;
  
        public ReplacingCharacterRule()
        {
            replacedCharacters = new List<string>();    
        }
        public object Clone()
        {
            ReplacingCharacterRule returnedObject = new ReplacingCharacterRule();
            returnedObject.parameter = (TypedParameter)parameter.Clone();
            int index = 0;
            foreach(var element in replacedCharacters)
            {
                returnedObject.replacedCharacters.Add(replacedCharacters.ElementAt(index));
                index++;
            }
            return returnedObject;
        }

        public TypedParameter getParameter()
        {
            return parameter;   
        }

        public string getRuleType()
        {
            return "ReplacingCharacterRule";
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
               foreach(var element in subStrings[0])
                {
                    if(replacedCharacters.Contains(element.ToString()))
                    {
                        builder.Append(newCharacter);
                    }
                    else
                    {
                        builder.Append(element);
                    }
                }
                builder.Append(".");
                builder.Append(subStrings[1]);
            }
            else
            {
                foreach (var element in fileName)
                {
                    if (replacedCharacters.Contains(element.ToString()))
                    {
                        builder.Append(newCharacter);
                    }
                    else
                    {
                        builder.Append(element);
                    }
                }
            }
            string result = builder.ToString();
            return result;
        }
        public void HandleInputParameter()
        {
            newCharacter = parameter.inputStrings.ElementAt(NEW_CHARACTER_POSTION);
            int totalReplacedCharacters = int.Parse(parameter.inputStrings.ElementAt(TOTAL_REPLACED_CHARACTERS_POSITION));
            for(int i=2;i<2+totalReplacedCharacters ; i++)
            {
                replacedCharacters.Add(parameter.inputStrings.ElementAt(i));
            }
        }

        public void setParameter(TypedParameter parameter)
        {
            this.parameter = parameter;
            HandleInputParameter();
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
