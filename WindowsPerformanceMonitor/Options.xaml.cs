﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow

        //public ObservableCollection<string> extFileComboBoxx { get; set; }
        public ObservableCollection<string> _extFileComboBox { get; set; }
        private string _selectedExtFileComboBox;

        public Options()
        {
            InitializeComponent();
            //extFileComboBoxx = new ObservableCollection<string> {".txt", ".doc", ".pdf"};
            extFileComboBox = new ObservableCollection<string> { ".txt", ".doc", ".pdf" };
            selectedExtFileComboBox = ".txt";
            this.DataContext = this;
        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
            FileExtensionComboBox.SelectedIndex = 0;
        }

        private void File_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CompareLogs compare = new CompareLogs(Application.Current.MainWindow);
            compare.Show();
            Application.Current.MainWindow.Hide();
        }

        public ObservableCollection<string> extFileComboBox
        {
            get { return _extFileComboBox; }
            set
            {
                _extFileComboBox = value;
                OnPropertyChanged(nameof(extFileComboBox));
            }
        }
        public string selectedExtFileComboBox
        {
            get { return _selectedExtFileComboBox; }
            set
            {
                _selectedExtFileComboBox = value;
                OnPropertyChanged(nameof(selectedExtFileComboBox));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Combobox " + FileExtensionComboBox.SelectedItem);
            string selected = (string)FileExtensionComboBox.SelectedItem;

            if (selected != null)
            {
                //update the file extension for reals
                Globals._logFileType = selected;
            }
            else
            {
                //liveGraph.ProcessPid = -1;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.IsChecked == true)
            {
                Globals._encryptionEnabled = true;
            } else
            {
                Globals._encryptionEnabled = false;
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null ) return;

            switch (radioButton.Content.ToString())
            {
                case "Repeat":
                    //setCPUValues(List);
                    //groupBoxDetails.Header = "CPU Details";
                    break;
                default:
                    break;
            }

        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            // CoolButton Clicked! Let's show our InputBox.
            InputBox.Visibility = System.Windows.Visibility.Visible;
            DurationInputBox.Visibility = System.Windows.Visibility.Visible;
            RepeatButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Do something with the Input

            // validate the time formats
            if (validateTime(InputTextBox.Text, DurationInputTextBox.Text) == 0) {
                String input = "Log Scheduled: " + InputTextBox.Text + " Duration: " + DurationInputTextBox.Text + " Repeats: " + RepeatButton.IsChecked;
                LogScheduleListBox.Items.Add(input); // Add Input to our ListBox.
                LogScheduleListBox.Visibility = System.Windows.Visibility.Visible;

                // YesButton Clicked! Let's hide our InputBox and handle the input text.
                InputBox.Visibility = System.Windows.Visibility.Collapsed;
                DurationInputBox.Visibility = System.Windows.Visibility.Collapsed;
                RepeatButton.Visibility = System.Windows.Visibility.Collapsed;

                // Clear InputBox.
                InputTextBox.Text = String.Empty;
                DurationInputTextBox.Text = String.Empty;
                RepeatButton.IsChecked = false;
            }
            // Clear InputBox.
            InputTextBox.Text = String.Empty;
            DurationInputTextBox.Text = String.Empty;
            RepeatButton.IsChecked = false;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = String.Empty;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            InputBox.Visibility = System.Windows.Visibility.Collapsed;
            DurationInputBox.Visibility = System.Windows.Visibility.Collapsed;
            RepeatButton.Visibility = System.Windows.Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = String.Empty;
            DurationInputTextBox.Text = String.Empty;
            RepeatButton.IsChecked = false;
        }

        private int validateTime(String time, String duration)
        {
            var timeregex = @"^ *(1[0-2]|[1-9]):[0-5][0-9] *(a|p|A|P)(m|M) *$";

            var match = Regex.Match(time, timeregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = MessageBox.Show("The input format is wrong. Example 00:00 AM.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            var durationregex = @"^(([0]?[1-9]|1[0-2])(:)([0-5][0-9]))$";

            match = Regex.Match(duration, durationregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = MessageBox.Show("The input format is wrong. Example 00:00.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            return 0;
        }

    }
}
