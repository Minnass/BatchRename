using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using BasicCore;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace MainUI
{

    internal class Util
    {
        static char[] invalidExtensionCharacters =
          {
                 '<',
                '>',
                ':',
                '\"',
                '/',
                '\\',
                '|',
                '?',
                '*',
                 (char)0,
                (char)1,
                (char)2,
                (char)3,
                (char)4,
                (char)5,
                (char)6,
                (char)7,
                (char)8,
                (char)9,
                (char)10,
                (char)11,
                (char)12,
                (char)13,
                (char)14,
                (char)15,
                (char)16,
                (char)17,
                (char)18,
                (char)19,
                (char)20,
                (char)21,
                (char)22,
                (char)23,
                (char)24,
                (char)25,
                (char)26,
                (char)27,
                (char)28,
                (char)29,
                (char)30,
                (char)31
        };
        static string[] invalidName =
            {
                "CON",
                "PRN",
                "AUX",
                "NUL",
                "COM1",
                "COM2",
                "COM3",
                "COM4",
                "COM5",
                "COM6",
                "COM7",
                "COM8",
                "COM9",
                "LPT1",
                "LPT2",
                "LPT3",
                "LPT4",
                "LPT5",
                "LPT6",
                "LPT7",
                "LPT8",
                "LPT9"
            };

        /* function find child control inside parent conrol
         * 
         */
        public static FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }
        public static void loadDllFile(string path, IRuleHandlerFactory factory)
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = new DirectoryInfo(exeFolder).GetFiles(path);
            foreach (var dll in dlls)

            {

                var assembly = Assembly.LoadFile(dll.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        if (typeof(IRuleHandler).IsAssignableFrom(type))
                        {
                            var temp_rule = Activator.CreateInstance(type) as IRuleHandler;
                            if (!factory.isExistRule(temp_rule.getRuleType()))
                            {
                                factory.registerIRuleHandler(temp_rule);
                            }

                        }
                    }
                }
            }
        }

        public static string getErrorFileName(string fileName, bool isFileType)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "Empty name";
            }
            
  
            if(isFileType)
            {
                string[] subsString = fileName.Split(".", StringSplitOptions.None);
                string name = subsString[0];
                string extension=subsString[1]; 
                foreach(var character in invalidExtensionCharacters)
                {
                    if(name.Contains(character)||extension.Contains(character))
                    {
                        return $"Contain invalid {character}";
                   
                    }
                }
                if(invalidName.Contains(name))
                {
                    return $"{name} is invalid name";
                }
                if(name.Length>255)
                {
                    return "Name too long";
                }
            }
            else
            {
                foreach (var character in invalidExtensionCharacters)
                {
                    if (fileName.Contains(character))
                    {
                        return $"Contain invalid {character}";
                      
                    }

                }
                if (invalidName.Contains(fileName))
                {
                    return $"{fileName} is invalid name";
                }
                if(fileName.Length>255)
                {
                    return "name too long";
                }
            }
            return String.Empty;
        }
    }

   public class CHeckBoxConverter:IValueConverter
    {
        public MainWindow dataParent { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            IRuleHandler ruleHandler = (IRuleHandler)value;
            foreach (var rule in dataParent.activeRules)
            {
                if (ruleHandler.getRuleType().Equals(rule.getRuleType()))
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)false;
        }

   
    }
}
