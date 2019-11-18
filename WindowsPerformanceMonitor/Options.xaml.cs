using System;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : System.Windows.Controls.UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        public ObservableCollection<string> _extFileComboBox { get; set; }
        private string _selectedExt;
        NotificationThresholds nt = new NotificationThresholds();
        public Options()
        {
            InitializeComponent();
            extFileComboBox = new ObservableCollection<string> { ".txt", ".doc", ".pdf" };
            this.DataContext = this;
            loadThresholds();

        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
            if (Globals.Settings.settings.LogFileFormat != null)
            {

                if (Globals.Settings.settings.LogFilePath != null && Globals.Settings.settings.LogFilePath != string.Empty)
                {
                    Globals._log.logPath = Globals.Settings.settings.LogFilePath;
                }

                FileExtensionComboBox.SelectedItem = Globals.Settings.settings.LogFileFormat;
                if (Globals.Settings.settings.IsEncryption == true)
                {
                    enc.IsChecked = true;
                    Globals._encryptionEnabled = true;

                }
                else
                {
                    enc.IsChecked = false;
                    Globals._encryptionEnabled = false;
                }
            }
            else
            {
                selectedExt = ".txt";
            }
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
            Globals.Settings.Save();
        }
        private void ChooseLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Fbd = new FolderBrowserDialog();
            if (Fbd.ShowDialog() == DialogResult.OK)
            {
                Globals._log.logPath = Fbd.SelectedPath;
                Globals.Settings.settings.LogFilePath = Fbd.SelectedPath;
            }


            Globals._logRef.GetLogList();
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            FileExtensionComboBox.SelectedItem = ".doc";

            //Globals.Settings.Delete();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CompareLogs compare = new CompareLogs(System.Windows.Application.Current.MainWindow);
            compare.Show();
            System.Windows.Application.Current.MainWindow.Hide();
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
        public string selectedExt
        {
            get { return _selectedExt; }
            set
            {
                _selectedExt = value;
                OnPropertyChanged(nameof(selectedExt));
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
                Globals.Settings.settings.LogFileFormat = selected;
            }
            else
            {
                //liveGraph.ProcessPid = -1;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox check = (System.Windows.Controls.CheckBox)sender;
            if (check.IsChecked == true)
            {
                Globals._encryptionEnabled = true;
                Globals.Settings.settings.IsEncryption = true;
            }
            else
            {
                Globals._encryptionEnabled = false;
                Globals.Settings.settings.IsEncryption = false;

            }
        }

        public int loadThresholds()
        {
            if (nt.cpuThreshold < 0 || nt.cpuThreshold > 100)
            {
                nt.cpuThreshold = 0;
            }

            if (nt.gpuThreshold < 0 || nt.gpuThreshold > 100)
            {
                nt.gpuThreshold = 0;
            }

            if (nt.memoryThreshold < 0 || nt.memoryThreshold > 100)
            {
                nt.memoryThreshold = 0;
            }

            CPUThresholdTextBox.Text = nt.cpuThreshold.ToString();
            GPUThresholdTextBox.Text = nt.gpuThreshold.ToString();
            MemoryThresholdTextBox.Text = nt.memoryThreshold.ToString();
            return 0;
        }

        private void SaveThresholdsButton_Click(object snder, RoutedEventArgs e)
        {
            nt.cpuThreshold = Convert.ToDouble(CPUThresholdTextBox.Text);
            nt.gpuThreshold = Convert.ToDouble(GPUThresholdTextBox.Text);
            nt.memoryThreshold = Convert.ToDouble(MemoryThresholdTextBox.Text);
            loadThresholds();
            App.Current.Properties["cpuThreshold"] = nt.cpuThreshold;
            App.Current.Properties["gpuThreshold"] = nt.gpuThreshold;
            App.Current.Properties["memoryThreshold"] = nt.memoryThreshold;
        }
        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            // CoolButton Clicked! Let's show our InputBox.
            DurationInputBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // Do something with the Input

            // validate the time formats
            if (validateTime(InputTextBox.Text, DurationInputTextBox.Text) == 0) {
                String input = "Log Scheduled: " + InputTextBox.Text + " Duration: " + DurationInputTextBox.Text;
                LogScheduleListBox.Items.Add(input); // Add Input to our ListBox.
                LogScheduleListBox.Visibility = System.Windows.Visibility.Visible;

                // YesButton Clicked! Let's hide our InputBox and handle the input text.
                DurationInputBox.Visibility = System.Windows.Visibility.Collapsed;

                // Clear InputBox.
                InputTextBox.Text = String.Empty;
                DurationInputTextBox.Text = String.Empty;
            }
            // Clear InputBox.
            InputTextBox.Text = String.Empty;
            DurationInputTextBox.Text = String.Empty;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            DurationInputBox.Visibility = System.Windows.Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = String.Empty;
            DurationInputTextBox.Text = String.Empty;
        }

        private int validateTime(String time, String duration)
        {
            var timeregex = @"^ *(1[0-2]|[1-9]):[0-5][0-9] *(a|p|A|P)(m|M) *$";

            var match = Regex.Match(time, timeregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = System.Windows.MessageBox.Show("The input format is wrong. Example 00:00 AM.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            var durationregex = @"^(([0]?[1-9]|1[0-2])(:)([0-5][0-9]))$";

            match = Regex.Match(duration, durationregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = System.Windows.MessageBox.Show("The input format is wrong. Example 00:00.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            return 0;
        }

    }
}
