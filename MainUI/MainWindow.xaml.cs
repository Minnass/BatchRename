using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using BasicCore;
using Fluent;
using System.ComponentModel;
using System.Collections;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.Json;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Extensions.Primitives;

namespace MainUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private string pathProjectSaving = AppDomain.CurrentDomain.BaseDirectory + @"../../../project.proj";
        private const string DLLS_PATH = "rules/*.dll";
        private BindingList<PresentedItem> selectedItems;
        private IRuleHandlerFactory IRuleHandlerFactory;
        private BindingList<IRuleHandler> selectedRules;
        public  List<IRuleHandler> activeRules;
        private BindingList<string> itemTypes;

        #region"Start project"
        public MainWindow()
        {
            InitializeComponent();

            CHeckBoxConverter cHeckBoxConverter = new CHeckBoxConverter { dataParent = this };
           var objec= (CHeckBoxConverter)this.FindResource("checkboxConverter");
            objec.dataParent = this;
            selectedItems = new BindingList<PresentedItem>();
            selectedRules = new BindingList<IRuleHandler>();
            activeRules = new List<IRuleHandler>();
            IRuleHandlerFactory = IRuleHandlerFactory.getInstance();
            itemTypes = new BindingList<string>() { "File", "Folder" };
            this.DataContext = this;

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Auto saving project one time after 12 second 
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (s, ev) => saveProject(pathProjectSaving);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

            //Init collection of rules 
            Util.loadDllFile(DLLS_PATH, IRuleHandlerFactory);
            //binding rulesComboxBox
            rulesComboxBox.ItemsSource = IRuleHandlerFactory.getTotalCurrentRules();
            chosenItems.ItemsSource = selectedItems;
            //binding selectedRule
            chosenRules.ItemsSource = selectedRules;
            //binding typeComboBo
            typeComboBox.ItemsSource = itemTypes;


        }
        private void renderLastContent_Click(object sender, EventArgs e)
        {
            if (File.Exists(pathProjectSaving))
            {
                var result = MessageBox.Show("Do you want to continue with lately project? ", "Autosave Detected", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK);
                if (MessageBoxResult.Yes == result)
                {
                    loadProject(pathProjectSaving);
                }
            }
        }

        #endregion

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

        #endregion


        #region "Handle click event Tab Item"

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
        private void resetRule_Click(object sender, RoutedEventArgs e)
        {
            Util.loadDllFile(DLLS_PATH, IRuleHandlerFactory);
            rulesComboxBox.ItemsSource = IRuleHandlerFactory.getTotalCurrentRules();
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
                            newName = string.Empty,
                            error = string.Empty
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

        private void resetItem_Click(object sender, RoutedEventArgs e)
        {
            selectedItems.Clear();
        }
        #endregion

        #region "Handle something on Item listview"
        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {

            int positionItem = chosenItems.SelectedIndex;
            if (positionItem != -1)
            {
                selectedItems.RemoveAt(positionItem);
            }
        }
        private void DropItem(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string item in items)
                {
                    if (typeComboBox.SelectedItem.ToString() == "File")
                    {
                        if (File.Exists(Path.GetFullPath(item)))
                        {
                            selectedItems.Add(new PresentedItem()
                            {
                                currentName = Path.GetFileName(item),
                                path = Path.GetFullPath(item)
                            });
                        }
                    }
                    else if (typeComboBox.SelectedItem.ToString() == "Folder")
                    {
                        if (Directory.Exists(Path.GetFullPath(item)))
                        {
                            selectedItems.Add(new PresentedItem()
                            {
                                currentName = Path.GetFileName(item),
                                path = Path.GetFullPath(item)
                            });
                        }
                    }
                }
            }
        }

        #endregion

        #region "Interact with rule in selected rules"
        private void moveToTop_Click(object sender, RoutedEventArgs e)
        {
            int index = chosenRules.SelectedIndex;
            if (index != -1 && index > 0)
            {
                IRuleHandler temp = selectedRules[index];
                for (int i = index; i > 0; i--)
                {
                    selectedRules[i] = selectedRules[i - 1];
                }
                selectedRules[0] = temp;

            }

        }

        private void moveToPrevious_Click(object sender, RoutedEventArgs e)
        {
            int index = chosenRules.SelectedIndex;
            if (index != -1 && index > 0)
            {
                IRuleHandler temp = selectedRules[index];
                selectedRules[index] = selectedRules[index - 1];
                selectedRules[index - 1] = temp;
            }
        }

        private void moveToBehind_Click(object sender, RoutedEventArgs e)
        {
            int index = chosenRules.SelectedIndex;
            if (index != -1 && index < selectedRules.Count - 1)
            {
                IRuleHandler temp = selectedRules[index];
                selectedRules[index] = selectedRules[index + 1];
                selectedRules[index + 1] = temp;
            }
        }

        private void moveToBottom_Click(object sender, RoutedEventArgs e)
        {
            int index = chosenRules.SelectedIndex;
            if (index != -1 && index < selectedRules.Count - 1)
            {
                IRuleHandler temp = selectedRules[index];
                for (int i = index; i < selectedRules.Count - 1; i++)
                {
                    selectedRules[i] = selectedRules[i + 1];
                }
                selectedRules[selectedRules.Count - 1] = temp;

            }

        }
        private void deleteRule_Click(object sender, RoutedEventArgs e)
        {
            if (chosenRules.SelectedItem != null)
            {
                List<IRuleHandler> temp = new List<IRuleHandler>();
                foreach (var item in chosenRules.SelectedItems)
                {
                    temp.Add((IRuleHandler)item);
                }
                foreach (var item in temp)
                {
                    selectedRules.Remove((IRuleHandler)item);
                    try
                    {
                        activeRules.Remove((IRuleHandler)item);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }

        private void expander_loading(object sender, RoutedEventArgs e)
        {
            var boder = VisualTreeHelper.GetParent((FrameworkElement)sender);
            var contentPresenter = VisualTreeHelper.GetParent(boder);
            var dockpandel = (DockPanel)VisualTreeHelper.GetParent(contentPresenter);
            IRuleHandler obj = (IRuleHandler)dockpandel.DataContext;
            if (obj.isEditable())
            {
                StackPanel stackPanel = (StackPanel)sender;
                obj.getEditor(stackPanel);
            }
        }
        private void activateOrDisableRule_Click(object sender, RoutedEventArgs e)
        {

            var expander = (Grid)VisualTreeHelper.GetParent((FrameworkElement)sender);
            IRuleHandler ruleHandler = (IRuleHandler)expander.DataContext;

            if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
            {
                activeRules.Add(ruleHandler);
            }
            else
            {
                activeRules.Remove(ruleHandler);
            }

            Debug.WriteLine(activeRules.Count + " " + selectedRules.Count);
        }
        #endregion


        #region "Handle Click Ribbon Tab item "

        private void preview_Click(object sender, RoutedEventArgs e)
        {
            if (activeRules.Count == 0)
            {
                MessageBox.Show("None of rules set has been selected. Please add rule before rename.", "Process aborted", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }



            if (selectedItems.Count == 0)
            {
                MessageBox.Show("None of items  has been selected. Please add item for renaming.", "Process aborted", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }


            foreach (var item in selectedItems)
            {
                item.error = string.Empty;
            }


            //To check duplication
            List<string> newName = new List<string>();
            Dictionary<string, int> duplicatedName = new Dictionary<string, int>();

            if (typeComboBox.SelectedItem.ToString() == "File")
            {
                foreach (var item in selectedItems)
                {
                    if (!File.Exists(item.path))
                    {
                        item.error = "Source file not exist";
                        continue;
                    }
                    string tmpNewName = item.currentName;
                    foreach (var rule in activeRules)
                    {
                        if (!rule.raiseParameterInputError().Equals(string.Empty))
                        {
                            MessageBox.Show(rule.raiseParameterInputError(), rule.getRuleType(), MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        tmpNewName = rule.Handle(tmpNewName, true);

                    }
                    item.newName = tmpNewName;

                    newName.Add(item.newName);

                    //Check file name is valid or not

                    string error = Util.getErrorFileName(item.newName, true);
                    if (error != string.Empty)
                    {
                        item.error += error;
                    }
                }
                foreach (var rule in activeRules)
                {
                    rule.Done();
                }

            }
            else if (typeComboBox.SelectedItem.ToString() == "Folder")
            {
                foreach (var item in selectedItems)
                {
                    if (!Directory.Exists(item.path))
                    {
                        item.error = "Source folder not exist";
                        continue;
                    }
                    string tmpNewName = item.currentName;
                    foreach (var rule in activeRules)
                    {
                        if (!rule.raiseParameterInputError().Equals(string.Empty))
                        {
                            MessageBox.Show(rule.raiseParameterInputError(), rule.getRuleType(), MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        tmpNewName = rule.Handle(tmpNewName, false);
                    }
                    item.newName = tmpNewName;
                    newName.Add(item.newName);
                    string error = Util.getErrorFileName(item.newName, false);
                    if (error != string.Empty)
                    {
                        item.error = error;
                    }
                }
                foreach (var rule in activeRules)
                {
                    rule.Done();
                }
            }

            foreach (var name in newName)
            {
                if (!duplicatedName.ContainsKey(name))
                {
                    duplicatedName.Add(name, 1);
                }
                else
                {
                    duplicatedName[$"{name}"] += 1;
                }
            }

            foreach (var pair in duplicatedName)
            {
                if (pair.Value > 1)
                {
                    foreach (var item in selectedItems)
                    {
                        if (item.newName.Equals(pair.Key))
                        {
                            item.error += " Duplicated name";
                        }
                    }
                }
            }

        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (activeRules.Count == 0)
            {
                MessageBox.Show("None of rules set has been selected. Please add rule before rename.", "Process aborted", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("None of items  has been selected. Please add item for renaming.", "Process aborted", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            foreach(var item in selectedItems)
            {
                item.error = string.Empty;
            }

            string targetPath = "";
            bool moveMode = false;

            if (moveToNew.IsChecked == true)
            {

                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    targetPath = dialog.SelectedPath;
                    moveMode = true;
                }
            }
            List<string> newName = new List<string>();
            Dictionary<string, int> duplicatedName = new Dictionary<string, int>();
            int failItemCounter = 0;

            if (typeComboBox.SelectedItem.ToString() == "File")
            {
                foreach (var item in selectedItems)
                {
                    if (!File.Exists(item.path))
                    {
                        item.error = "Source file not exist";
                        continue;
                    }
                    string tmpNewName = item.currentName;
                    foreach (var rule in activeRules)
                    {
                        if (!rule.raiseParameterInputError().Equals(string.Empty))
                        {
                            MessageBox.Show(rule.raiseParameterInputError(), rule.getRuleType(), MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        tmpNewName = rule.Handle(tmpNewName, true);
                    }

                    item.newName = tmpNewName;
                    newName.Add(item.newName);

                    //Check file name is valid or not

                    string error = Util.getErrorFileName(item.newName, true);
                    if (error != string.Empty)
                    {
                        item.error += error;
                    }
                }
                foreach (var rule in activeRules)
                {
                    rule.Done();
                }

            }
            else if (typeComboBox.SelectedItem.ToString() == "Folder")
            {
                foreach (var item in selectedItems)
                {
                    if (!Directory.Exists(item.path))
                    {
                        item.error = "Source folder not exist";
                        continue;
                    }
                    string tmpNewName = item.currentName;
                    foreach (var rule in activeRules)
                    {
                        if (!rule.raiseParameterInputError().Equals(string.Empty))
                        {
                            MessageBox.Show(rule.raiseParameterInputError(), rule.getRuleType(), MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        tmpNewName = rule.Handle(tmpNewName, false);
                    }
                    item.newName = tmpNewName;
                    newName.Add(item.newName);
                    string error = Util.getErrorFileName(item.newName, false);
                    if (error != string.Empty)
                    {
                        item.error = error;
                    }
                }
                foreach (var rule in activeRules)
                {
                    rule.Done();
                }
            }

            foreach (var name in newName)
            {
                if (!duplicatedName.ContainsKey(name))
                {
                    duplicatedName.Add(name, 1);
                }
                else
                {
                    duplicatedName[$"{name}"] += 1;
                }
            }



            foreach (var pair in duplicatedName)
            {
                if (pair.Value > 1)
                {
                    foreach (var item in selectedItems)
                    {
                        if (item.newName.Equals(pair.Key))
                        {
                            item.error += " Duplicated name";
                        }
                    }
                }
            }
            if (moveToNew.IsChecked == false)
            {
                foreach (var item in selectedItems)
                {
                    if (item.error.Equals(string.Empty))
                    {
                        try
                        {
                            string pathFolder = Path.GetDirectoryName(item.path);
                            if (typeComboBox.SelectedItem.ToString().Equals("File"))
                            {
                                File.Move(item.path, pathFolder + "/" + item.newName);

                            }
                            else if (typeComboBox.SelectedItem.ToString().Equals("Folder"))

                            {
                                Directory.Move(item.path, pathFolder + "/" + item.newName);
                            }
                            item.currentName = item.newName;
                            item.path = pathFolder + "/" + item.newName;
                        }
                        catch (Exception exception)
                        {

                        }

                    }
                    else
                    {
                        failItemCounter++;
                    }
                }
            }
            else
            {
                if (moveMode)
                {
                    foreach (var item in selectedItems)
                    {
                        if (item.error.Equals(string.Empty))
                        {
                            if (typeComboBox.SelectedItem.ToString().Equals("File"))
                            {
                                File.Copy(item.path, targetPath + "/" + item.newName);
                            }
                            else if (typeComboBox.SelectedItem.ToString().Equals("Folder"))
                            {
                                // dasdadadsadasdsadasdsadsadadsadsadasdsadsadsadsadsadsadsadsdad
                            }
                            item.currentName = item.newName;
                        }
                        else
                        {
                            failItemCounter++;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            MessageBox.Show(selectedItems.Count - failItemCounter + $" item rename successfully. Error {failItemCounter} for renaming", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void saveRule_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRules.Count == 0)
            {
                MessageBox.Show("No rule to save.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "JSON (*.json)|*.json";

            saveFileDialog.ValidateNames = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                List<RuleJsonFormat> ruleJsonFormats = new List<RuleJsonFormat>();
                StreamWriter output;

                foreach (var rule in selectedRules)
                {
                    ruleJsonFormats.Add(new RuleJsonFormat
                    {
                        InputStrings = rule.getParameter().inputStrings,

                        RuleType = rule.getRuleType()
                    });
                }
                output = new StreamWriter(saveFileDialog.FileName);
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                string strJson = JsonSerializer.Serialize(ruleJsonFormats.ToArray(), opt);
                output.Write(strJson);
                output.Close();
            }


        }

        private void OpenRule_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON(*.json) | *.json";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedRules.Clear();
                List<RuleJsonFormat> ruleJsonFormats = new List<RuleJsonFormat>();
                StreamReader reader = new StreamReader(openFileDialog.FileName);
                string json = reader.ReadToEnd();
                try
                {
                    ruleJsonFormats = JsonSerializer.Deserialize<List<RuleJsonFormat>>(json);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    MessageBox.Show("Open rule not succesful", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                foreach (var item in ruleJsonFormats)
                {
                    IRuleHandler rule = IRuleHandlerFactory.parse(item.RuleType);
                    if (rule != null)
                    {
                        if (item.InputStrings.Count != 0)
                        {
                            rule.setParameter(new TypedParameter { inputStrings = item.InputStrings });
                        }
                        selectedRules.Add(rule);
                    }
                    MessageBox.Show("Load rule successfully.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }


        }

        private void saveProject_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PROJ (*.proj)|*.proj";
            if (saveDialog.ShowDialog() == true)
            {
                path = saveDialog.FileName;
                saveProject(path);
                MessageBox.Show("Save Project succesfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void saveProject(string path)
        {
            List<PresentedItemJsonFormat> presentedFormat = new List<PresentedItemJsonFormat>();
            List<RuleJsonFormat> ruleFormat = new List<RuleJsonFormat>();


            foreach (var rule in selectedRules)
            {
                ruleFormat.Add(new RuleJsonFormat
                {
                    RuleType = rule.getRuleType(),
                    InputStrings = rule.getParameter().inputStrings
                });
            }
            foreach (var item in selectedItems)
            {
                presentedFormat.Add(new PresentedItemJsonFormat
                {
                    currentName = item.currentName,
                    newName = item.newName,
                    path = item.path,
                    error = item.error,

                });
            }


            ProjectJsonFormat projectJsonFormat = new ProjectJsonFormat
            {
                type = (typeComboBox.SelectedItem != null) ? typeComboBox.SelectedItem.ToString() : string.Empty,
                ruleJsonFormats = ruleFormat,
                presentedJsonFormat = presentedFormat
            };

            StreamWriter output;
            try
            {
                output = new StreamWriter(path);
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                string strJson = JsonSerializer.Serialize(projectJsonFormat, opt);
                output.Write(strJson);
                output.Close();

            }
            catch (Exception e)
            {
                //do nothing       
            }
        }

        void loadProject(string path)
        {
            ProjectJsonFormat projectJsonFormat = new ProjectJsonFormat();
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            try
            {
                projectJsonFormat = JsonSerializer.Deserialize<ProjectJsonFormat>(json);
            }
            catch (Exception exception)
            {

                MessageBox.Show("Project load failed.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (projectJsonFormat != null)
            {
                if (!projectJsonFormat.type.Equals(string.Empty))
                {
                    typeComboBox.SelectedItem = projectJsonFormat.type;
                }
                if (projectJsonFormat.ruleJsonFormats.Count > 0)
                {
                    foreach (var rule in projectJsonFormat.ruleJsonFormats)
                    {
                        IRuleHandler newRule = IRuleHandlerFactory.parse(rule.RuleType);
                        if (rule != null)
                        {
                            if (rule.InputStrings.Count != 0)
                            {
                                newRule.setParameter(new TypedParameter { inputStrings = rule.InputStrings });
                            }
                            selectedRules.Add(newRule);
                        }
                    }

                }
                if (projectJsonFormat.presentedJsonFormat.Count > 0)
                {
                    foreach (var item in projectJsonFormat.presentedJsonFormat)
                    {
                        selectedItems.Add(new PresentedItem
                        {
                            currentName = item.currentName,
                            newName = item.newName,
                            path = item.path,
                            error = item.error
                        });
                    }
                }
            }
            MessageBox.Show("Project load  succesfully. ", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void loadProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PROJ (*.proj)|*.proj";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedRules.Clear();
                selectedItems.Clear();
                loadProject(openFileDialog.FileName);
            }

        }

        #endregion
    }
}
