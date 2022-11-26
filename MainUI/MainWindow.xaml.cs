using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Security.Policy;
using BasicCore;
using System.Reflection;
using Fluent;
using System.ComponentModel;
using System.Collections;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MainUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private const string DLLS_PATH = "rules/*.dll";
        private BindingList<PresentedItem> selectedItems;
        private IRuleHandlerFactory IRuleHandlerFactory;
        private BindingList<IRuleHandler> selectedRules;
        private BindingList<string> itemTypes;

        public MainWindow()
        {
            InitializeComponent();
            selectedItems = new BindingList<PresentedItem>();
            selectedRules = new BindingList<IRuleHandler>();
            IRuleHandlerFactory = IRuleHandlerFactory.getInstance();
            itemTypes = new BindingList<string>() { "File", "Folder" };

        }

        private void RemoveChosenRule(object sender, RoutedEventArgs e)
        {

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Init collection of rules 
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = new DirectoryInfo(exeFolder).GetFiles(DLLS_PATH);
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
                            IRuleHandlerFactory.registerIRuleHandler(temp_rule);
                        }
                    }
                }
            }

            //binding rulesComboxBox
            rulesComboxBox.ItemsSource = IRuleHandlerFactory.getTotalCurrentRules();
            chosenItems.ItemsSource = selectedItems;
            //binding selectedRule
            chosenRules.ItemsSource = selectedRules;
            //binding typeComboBo
            typeComboBox.ItemsSource = itemTypes;


        }

        #region "Handle UI"
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double screenWidth = window.ActualWidth;
            itemsArea.Width = screenWidth
                 - ruleArea.ActualWidth
                 - ruleArea.Margin.Left
                 - ruleArea.Margin.Right
                 - itemsArea.Margin.Left
                 - itemsArea.Margin.Right;

        }

        private void chosenItems_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((GridViewColumn)((GridView)chosenItems.View).Columns[0]).Width = e.NewSize.Width / 4;
            ((GridViewColumn)((GridView)chosenItems.View).Columns[1]).Width = e.NewSize.Width / 4;
            ((GridViewColumn)((GridView)chosenItems.View).Columns[2]).Width = e.NewSize.Width / 4;
            ((GridViewColumn)((GridView)chosenItems.View).Columns[3]).Width = e.NewSize.Width / 4;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addRule_Click(object sender, RoutedEventArgs e)
        {

            if (rulesComboxBox.SelectedIndex != -1)
            {
                IRuleHandler selectedObject = (IRuleHandler)rulesComboxBox.SelectedItem;
                foreach (var element in selectedRules)
                {
                    if (element.getRuleType() == selectedObject.getRuleType())
                    {
                        return;
                    }
                }
                selectedRules.Add((IRuleHandler)selectedObject.Clone());
            }
        }

        private void typeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItems.Clear();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (typeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please choose type you want to rename", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int counter = 0;

            if (typeComboBox.SelectedItem.ToString() == "File")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    string[] files = openFileDialog.FileNames;


                    List<PresentedItem> newFilenames = new List<PresentedItem>();


                    foreach (var file in files)
                    {
                        newFilenames.Add(new PresentedItem
                        {
                            currentName = Path.GetFileName(file),
                            path = Path.GetFullPath(file),
                            newName = "",
                            error = ""
                        });
                    }

                    foreach (var file in newFilenames)
                    {
                        bool isExist = false;
                        foreach (var item in selectedItems)
                        {
                            if (file.currentName == item.currentName && file.path == item.path)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist)
                        {
                            selectedItems.Add(file);
                            counter++;
                        }
                    }
                    MessageBox.Show(counter + " files have been added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else if (typeComboBox.SelectedItem.ToString() == "Folder")
            {
                using var openFolderDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Multiselect = true
                };

                if (openFolderDialog.ShowDialog() == CommonFileDialogResult.Ok)

                {
                    var folders = openFolderDialog.FileNames;
                    List<PresentedItem> newFilenames = new List<PresentedItem>();
                    foreach (var item in folders)
                    {

                        newFilenames.Add(new PresentedItem
                        {
                            currentName = Path.GetFileName(item),
                            path = Path.GetFullPath(item),
                            newName = "",
                            error = ""
                        });
                    }
                    foreach (var folder in newFilenames)
                    {
                        bool isExist = false;
                        foreach (var item in selectedItems)
                        {
                            if (item.currentName == folder.currentName && item.path == folder.path)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist)
                        {
                            selectedItems.Add(folder);
                            counter++;
                        }
                    }
                    MessageBox.Show(counter + " folders have been added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {

            int positionItem = chosenItems.SelectedIndex;
            if(positionItem != -1)
            {
                selectedItems.RemoveAt(positionItem);   
            }
        }

        private void resetItem_Click(object sender, RoutedEventArgs e)
        {
            selectedItems.Clear();
        }

        private void resetRule_Click(object sender, RoutedEventArgs e)
        {
            selectedRules.Clear();
        }

        private void moveToTop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void moveToPrevious_Click(object sender, RoutedEventArgs e)
        {

        }

        private void moveToBehind_Click(object sender, RoutedEventArgs e)
        {

        }

        private void moveToBottom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteRule_Click(object sender, RoutedEventArgs e)
        {

        }



        //Setting can be editable for newRule added 





        #endregion

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{



        //    Debug.WriteLine(IRuleHandlerFactory.totalIruleHandler());


        //    //selectedItems = new List<PresentedItem>();
        //    //OpenFileDialog openFileDialog = new OpenFileDialog();
        //    //openFileDialog.Filter = "All files (*.*)|*.*";
        //    //openFileDialog.Multiselect = true;
        //    //if (openFileDialog.ShowDialog() == true)
        //    //{
        //    //    string[] files = openFileDialog.FileNames;
        //    //    foreach (var file in files)
        //    //    {
        //    //        string currentName = Path.GetFileName(file);
        //    //        string fullPath = Path.GetFullPath(file);
        //    //        selectedItems.Add(new PresentedItem
        //    //        {
        //    //            currentName = currentName,
        //    //            path = fullPath,
        //    //            newName="",
        //    //            error=""
        //    //        }) ;
        //    //    }
        //    //}







        //    foreach (var file in selectedItems)
        //    {
        //        //string temp = file.currentName;
        //        //foreach(var rule in chosenRules)
        //        //{
        //        //    temp = rule.Handle(temp, true);
        //        //}
        //        //file.newName = temp;

        //    }
        //    //chosenFiles.ItemsSource = selectedItems;
        //    DataContext = selectedItems;
        //}
    }
}
