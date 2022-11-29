using BasicCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Media3D;

namespace BatchRename
{

    public class ReplacingCharacterRuleEditor
    {
        private const int NEW_CHARACTER_POSTION = 0;
        private const int TOTAL_REPLACED_CHARACTERS_POSITION = 1;

        private TypedParameter parameter { get; set; }
        public delegate void PlacingCharacterEditor(TypedParameter param);
        public event PlacingCharacterEditor editorEvent;


        private Grid grid = new Grid();
        private TextBlock newCharacter = new TextBlock();
        private TextBlock removedCharacter = new TextBlock();
        private TextBlock notion=new TextBlock();

        private TextBox newChacterInput = new TextBox();
        private TextBox removedCharacterInput =new TextBox();
        


        public ReplacingCharacterRuleEditor(TypedParameter parameter, StackPanel frame)
        {
            this.parameter = parameter;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
       

            newCharacter.Text = "New character:";
            newCharacter.FontSize = 10;
    
            removedCharacter.Text = "Characters removed:";
            removedCharacter.FontSize = 10;
       

            notion.Text = "(Multiple characters are separated by ',')";
            notion.FontSize = 10;
  
            newChacterInput.Text = (parameter.inputStrings[NEW_CHARACTER_POSTION].Equals(string.Empty)) ? string.Empty : parameter.inputStrings.ElementAt(NEW_CHARACTER_POSTION);
            removedCharacterInput.Text = (parameter.inputStrings[TOTAL_REPLACED_CHARACTERS_POSITION].Equals(string.Empty)) ? string.Empty : parameter.inputStrings.ElementAt(TOTAL_REPLACED_CHARACTERS_POSITION);

            var style = new Style(typeof(Border));
            style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3)));
            newChacterInput.Resources.Add(typeof(Border), style);
            removedCharacterInput.Resources.Add(typeof(Border), style);
            newCharacter.Margin = new Thickness(2);
            removedCharacterInput.Margin = new Thickness(2);
            grid.Children.Add(newCharacter);
            grid.Children.Add(removedCharacter);
            grid.Children.Add(newChacterInput);
            grid.Children.Add(removedCharacterInput);
            grid.Children.Add(notion);
            notion.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(newCharacter, 0);
            Grid.SetColumn(newCharacter, 0);
            Grid.SetRow(removedCharacter, 1);
            Grid.SetColumn(removedCharacter, 0);
            Grid.SetRow(newChacterInput, 0);
            Grid.SetColumn(newChacterInput, 1);
            Grid.SetRow(removedCharacterInput, 1);
            Grid.SetColumn(removedCharacterInput, 1);
            Grid.SetRow(notion, 2);
            Grid.SetColumn(notion, 0);
            Grid.SetColumnSpan(notion, 3);
            

            newChacterInput.TextChanged += newCharacterInput_change;
            removedCharacterInput.TextChanged += removedCharacter_change;

            frame.Children.Add(grid);
        }
        private void newCharacterInput_change(object sender, EventArgs e)
        {
            parameter.inputStrings[NEW_CHARACTER_POSTION] = newChacterInput.Text;

            editorEvent?.Invoke(parameter);
        }
        private void removedCharacter_change(object sender, EventArgs e)
        {

            Regex validFormat = new Regex(@"([\w\d\s],)+[\w\d\s]$|[\w\d\s]$");
            if(!validFormat.IsMatch(removedCharacterInput.Text))
            {
                parameter.inputStrings[TOTAL_REPLACED_CHARACTERS_POSITION] = string.Empty;
            }
            else
            {
                string temp = parameter.inputStrings[NEW_CHARACTER_POSTION];
                parameter.inputStrings.Clear();
                parameter.inputStrings.Add(temp);
                parameter.inputStrings.Add(string.Empty);
                string[] subStrings=removedCharacterInput.Text.Split(",",StringSplitOptions.None);
                parameter.inputStrings[TOTAL_REPLACED_CHARACTERS_POSITION] = subStrings.Length.ToString();
                foreach(var item in subStrings)
                {
                    parameter.inputStrings.Add(item);
                }
            }
            editorEvent?.Invoke(parameter);
        }

    }
    
    public class ReplacingCharacterRule : Rule, IRuleHandler
    {
        private const int NEW_CHARACTER_POSTION = 0;
        private const int TOTAL_REPLACED_CHARACTERS_POSITION = 1;
        private List<string> replacedCharacters;
        private string newCharacter;
  
        public ReplacingCharacterRule()
        {
            parameter.inputStrings.Add(string.Empty);
            parameter.inputStrings.Add(string.Empty);
            replacedCharacters = new List<string>();
        }
        public object Clone()
        {
            ReplacingCharacterRule returnedObject = new ReplacingCharacterRule();
            returnedObject.setParameter((TypedParameter)parameter.Clone());
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
            replacedCharacters.Clear();

            newCharacter = (parameter.inputStrings.ElementAt(NEW_CHARACTER_POSTION)==string.Empty)? string.Empty : parameter.inputStrings[NEW_CHARACTER_POSTION];
            if(parameter.inputStrings[TOTAL_REPLACED_CHARACTERS_POSITION] != string.Empty)
            {
                int totalReplacedCharacters = int.Parse(parameter.inputStrings.ElementAt(TOTAL_REPLACED_CHARACTERS_POSITION));
                for (int i=2; i<2+totalReplacedCharacters;i++)
                {
                    replacedCharacters.Add(parameter.inputStrings.ElementAt(i));
                }
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

        public override string ToString()
        {
            return "Replacing Character Rule";
        }

        public void getEditor(StackPanel frame)
        {
            ReplacingCharacterRuleEditor editor = new ReplacingCharacterRuleEditor(this.parameter, frame);
            editor.editorEvent += setParameter;
        }

        public string raiseParameterInputError()
        {
            if (parameter.inputStrings[NEW_CHARACTER_POSTION] == string.Empty || parameter.inputStrings[TOTAL_REPLACED_CHARACTERS_POSITION]==string.Empty)
            {
                return "Invalid parameter input.";
            }
            return string.Empty;
        }

        public void Done()
        {
            
        }
    }
}
