using BasicCore;
using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{
    public class ReplacingNewNameEditor
    {
        private const int NEW_NAME_POSTION = 0;

        public delegate void newNameEditor(TypedParameter param);
        public event newNameEditor editorEvent;

        TypedParameter typedParameter { get; set; }
        TextBlock textBlock = new TextBlock();
        TextBox textBox = new TextBox();


        public ReplacingNewNameEditor(TypedParameter parameter, StackPanel frame)
        {
            this.typedParameter = parameter;
            textBlock.Text = "New Name:";
            textBox.Text = (typedParameter.inputStrings.Count == 0) ? string.Empty : typedParameter.inputStrings.ElementAt(NEW_NAME_POSTION);
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
    public class ReplacingNewNameRule : Rule, IRuleHandler
    {
        private const int NEW_NAME_POSTION = 0;
        private string newName;

        public ReplacingNewNameRule()
        {
            parameter.inputStrings.Add(string.Empty);
            newName = string.Empty;
        }
        public object Clone()
        {
            ReplacingNewNameRule returnedObject = new ReplacingNewNameRule();
            returnedObject.newName = newName;
            returnedObject.setParameter((TypedParameter)parameter.Clone());
            return returnedObject;
        }

        public void Done()
        {
           // do nothing
        }

        public void getEditor(StackPanel frame)
        {
            ReplacingNewNameEditor editor = new ReplacingNewNameEditor(this.parameter, frame);
            editor.editorEvent += setParameter;
        }

        public TypedParameter getParameter()
        {
            return parameter;
        }

        public string getRuleType()
        {
            return "ReplacingNewName";
        }

        public string Handle(string fileName, bool isFileType)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "";
            }
            var builder = new StringBuilder();
            string[] subStrings = fileName.Split(".", StringSplitOptions.None);

            builder.Append(newName);
            if(isFileType)
            {
                builder.Append(".");
            builder.Append(subStrings[1]);
            }
            string result=builder.ToString();
            return result;
        }

        public void HandleInputParameter()
        {
          newName = (parameter.inputStrings.Count != 0) ? parameter.inputStrings.ElementAt(NEW_NAME_POSTION) : string.Empty;
        }

        public bool isEditable()
        {
            return true;
        }
        public override string ToString()
        {
            return "Replacing New Name";
        }


        public string raiseParameterInputError()
        {
            if(newName.Equals(string.Empty))
            {
                return "Parameter Input is invalid.";
            }
            return string.Empty;
        }

        public void setParameter(TypedParameter parameter)
        {
            this.parameter = parameter;
            HandleInputParameter();
        }
    }
}
