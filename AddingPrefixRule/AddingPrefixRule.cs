using BasicCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BatchRename
{

    public class PrefixRuleEditor
    {
        private const int PREFIX_PARAMETER_POSTION = 0;

        public delegate void prefixEditor(TypedParameter param);
        public event prefixEditor editorEvent;

        TypedParameter typedParameter { get; set; }
        TextBlock textBlock = new TextBlock();
        TextBox textBox = new TextBox();


        public PrefixRuleEditor(TypedParameter parameter,StackPanel frame)
        {
            this.typedParameter = parameter;
            textBlock.Text = "Prefix:";
            textBox.Text=(typedParameter.inputStrings.Count==0)?string.Empty:typedParameter.inputStrings.ElementAt(PREFIX_PARAMETER_POSTION);
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
    public class AddingPrefixRule :Rule, IRuleHandler
    {
        private const int PREFIX_PARAMETER_POSTION = 0;
        private string prefixParameter;
        public AddingPrefixRule()
        {
            parameter.inputStrings.Add(string.Empty);
            prefixParameter = string.Empty;
        }
        public object Clone()
        {
            AddingPrefixRule returnedObject = new AddingPrefixRule();
            returnedObject.prefixParameter = prefixParameter;
            returnedObject.setParameter((TypedParameter)parameter.Clone());
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
            prefixParameter =(parameter.inputStrings.Count!=0)? parameter.inputStrings.ElementAt(PREFIX_PARAMETER_POSTION):string.Empty;
        }

        public bool isEditable()
        {
            return true;
        }


        public override string ToString()
        {
            return "Adding Prefix Rule";
        }

    
         
        public void getEditor(StackPanel frame)
        {
            PrefixRuleEditor editor = new PrefixRuleEditor(this.parameter ,frame);
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
