using BasicCore;
using System;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{


    public class SuffixRuleEditor
    {
        private const int SUFFIX_PARAMETER_POSTION = 0;

        public delegate void prefixEditor(TypedParameter param);
        public event prefixEditor editorEvent;

        TypedParameter typedParameter { get; set; }
        TextBlock textBlock = new TextBlock();
        TextBox textBox = new TextBox();


        public SuffixRuleEditor(TypedParameter parameter, StackPanel frame)
        {
            this.typedParameter = parameter;
            textBlock.Text = "Suffix:";
            textBox.Text = (typedParameter.inputStrings.Count == 0) ? string.Empty : typedParameter.inputStrings.ElementAt(SUFFIX_PARAMETER_POSTION);
            textBox.TextChanged += textbox_change;
            frame.Children.Add(textBlock);
            frame.Children.Add(textBox);

        }



        private void textbox_change(object sender, EventArgs e)
        {
            typedParameter.inputStrings.Clear();
            typedParameter.inputStrings.Add(textBox.Text);
            editorEvent?.Invoke(typedParameter);
        }

    }
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


        public override string ToString()
        {
            return "Adding Suffix Rule";
        }

        public void getEditor(StackPanel frame)
        {
            SuffixRuleEditor editor = new SuffixRuleEditor(this.parameter, frame);
            editor.editorEvent += setParameter;
        }

        public string raiseParameterInputError()
        {
            return string.Empty;
        }

        public void Done()
        {
         //do nothing
        }
    }
}
