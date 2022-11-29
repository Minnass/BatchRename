using BasicCore;
using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BatchRename
{

    public class ChangingExtensionExtensionEditor
    {
        private const int NEW_EXTENSION_POSTION = 0;

        public delegate void changingExtensionEditor(TypedParameter param);
        public event changingExtensionEditor editorEvent;

        TypedParameter typedParameter { get; set; }
        TextBlock textBlock = new TextBlock();
        TextBox textBox = new TextBox();
        public ChangingExtensionExtensionEditor(TypedParameter parameter, StackPanel frame)
        {
            this.typedParameter = parameter;
            textBlock.Text = "New Extension:";
            textBox.Text = (typedParameter.inputStrings.Count == 0) ? "" : typedParameter.inputStrings.ElementAt(NEW_EXTENSION_POSTION);
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
    public class ChangingExtensionRule : Rule, IRuleHandler
    {
        private const int NEW_EXTENSION_POSTION = 0;
        private string newExtension;
  
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
            newExtension = (parameter.inputStrings.Count != 0) ? parameter.inputStrings.ElementAt(NEW_EXTENSION_POSTION) : string.Empty;
        }

        public bool isEditable()
        {
            return true;
        }

        public override string ToString()
        {
            return "Changing Extension Rule";
        }

        public void getEditor(StackPanel frame)
        {
            ChangingExtensionExtensionEditor editor = new ChangingExtensionExtensionEditor(this.parameter, frame);
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
