using BasicCore;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BatchRename
{

    public class CounterSuffixRuleEditor
    {
        private const int DIGITS_POSTION = 0;
        private const int START_INDEX_POSITION = 1;
        private const int STEP_POSTION = 2;
        private TypedParameter parameter { get; set; }
        public delegate void suffixEditor(TypedParameter param);
        public event suffixEditor editorEvent;

        private Grid grid = new Grid();
        private TextBlock digit = new TextBlock();
        private TextBlock start = new TextBlock();
        private TextBlock step = new TextBlock();
        private TextBox digitInput=new TextBox();
        private TextBox startInput=new TextBox();
        private TextBox stepInput=new TextBox();    
        
        public CounterSuffixRuleEditor(TypedParameter parameter, StackPanel frame)
        {
            this.parameter=parameter;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(2,GridUnitType.Star)});
           
            digit.Text = "Digits:";

            start.Text = "Start:";
 
            step.Text = "Step:";

            digitInput.Text= (parameter.inputStrings[DIGITS_POSTION].Equals(string.Empty)) ? string.Empty : parameter.inputStrings.ElementAt(DIGITS_POSTION);
            startInput.Text = (parameter.inputStrings[START_INDEX_POSITION].Equals(string.Empty)) ? string.Empty : parameter.inputStrings.ElementAt(START_INDEX_POSITION);
            stepInput.Text = (parameter.inputStrings[STEP_POSTION].Equals(string.Empty)) ? string.Empty : parameter.inputStrings.ElementAt(STEP_POSTION);            var style = new Style(typeof(Border));
            style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3)));
            digitInput.Resources.Add(typeof(Border), style);
            startInput.Resources.Add(typeof(Border), style);
            stepInput.Resources.Add(typeof(Border), style);
            stepInput.Margin=new Thickness(2);
            startInput.Margin=new Thickness(2);
            digitInput.Margin = new Thickness(2);
            digitInput.TextChanged += digitInput_change;
            startInput.TextChanged+=startInput_change;
            stepInput.TextChanged += step_change;

            grid.Children.Add(digit);
            grid.Children.Add(start);
            grid.Children.Add(step);
            grid.Children.Add(digitInput);
            grid.Children.Add(startInput);
            grid.Children.Add(stepInput);
            Grid.SetRow(digit, 0);
            Grid.SetColumn(digit, 0);
            Grid.SetRow(start, 1);
            Grid.SetColumn(start,0);
            Grid.SetRow(step, 2);
            Grid.SetColumn(step, 0);
            Grid.SetRow(digitInput, 0);
            Grid.SetColumn(digitInput, 1);
            Grid.SetRow(startInput, 1);
            Grid.SetColumn(startInput, 1);
            Grid.SetRow(stepInput, 2);
            Grid.SetColumn(stepInput, 2);
            

            frame.Children.Add(grid);
        }
        private void digitInput_change(object sender, EventArgs e)
        {
              parameter.inputStrings[DIGITS_POSTION] = digitInput.Text;
              editorEvent?.Invoke(parameter);

        }
        private void startInput_change(object sender, EventArgs e)
        {

   
                parameter.inputStrings[START_INDEX_POSITION] = startInput.Text;

                editorEvent?.Invoke(parameter);


        }
        private void step_change(object sender, EventArgs e)
        {
         
     
                parameter.inputStrings[STEP_POSTION] = stepInput.Text;
                editorEvent?.Invoke(parameter);

         
        }
    }
    public class AddingCounterSuffixRule : Rule, IRuleHandler
    {
        private const int DIGITS_POSTION = 0;
        private const int START_INDEX_POSITION = 1;
        private const int STEP_POSTION = 2;

        private int startIndex;
        private int step;
        private int numberOfDigits;
        private int nextIndex;

        public  AddingCounterSuffixRule()
        {
            parameter.inputStrings.Add(string.Empty);
            parameter.inputStrings.Add(string.Empty);
            parameter.inputStrings.Add(string.Empty);
            startIndex = -1;
            step = -1;
            numberOfDigits = -1;
            nextIndex = -1;
        }
        public object Clone()
        {
            AddingCounterSuffixRule returnedObject = new AddingCounterSuffixRule();
            returnedObject.setParameter((TypedParameter)parameter.Clone());
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
            if(nextIndex.ToString().Length>numberOfDigits)
            {
                return fileName;
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
            Regex validFormat = new Regex(@"^\d+$");
            if (!validFormat.IsMatch(parameter.inputStrings[STEP_POSTION])
                || !validFormat.IsMatch(parameter.inputStrings[START_INDEX_POSITION])
                || !validFormat.IsMatch(parameter.inputStrings[DIGITS_POSTION]))
            {
                numberOfDigits = -1;
                startIndex = -1;
                step = -1;
                return;
            }
            numberOfDigits = int.Parse(parameter.inputStrings.ElementAt(DIGITS_POSTION));
            startIndex = int.Parse(parameter.inputStrings.ElementAt(START_INDEX_POSITION)) ;
            step = int.Parse(parameter.inputStrings.ElementAt(STEP_POSTION));
            nextIndex = startIndex;
        }

        public bool isEditable()
        {
            return true;
        }

        public override string ToString()
        {
            return "Adding Counter Suffix Rule";
        }

        public void getEditor(StackPanel frame)
        {
           CounterSuffixRuleEditor editor = new CounterSuffixRuleEditor(this.parameter, frame);
            editor.editorEvent += setParameter;
        }

        public string raiseParameterInputError()
        {
            if (startIndex == -1||step==-1||numberOfDigits==-1)
            {
                return "Invalid parameter input.";
            }
            if(startIndex.ToString().Length>numberOfDigits)
            {
                return "Invalid parameter input.";
            }
            return string.Empty;
        }

        public void Done()
        {
            nextIndex = startIndex;
        }
    }
}
