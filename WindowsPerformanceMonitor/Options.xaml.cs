using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        private MainWindow mainWindow = null; // Reference to the MainWindow
        public ObservableCollection<string> _extFileComboBox { get; set; }
        private string _selectedExt;

        public Options()
        {
            InitializeComponent();
            extFileComboBox = new ObservableCollection<string> { ".txt", ".doc", ".pdf" };

            this.DataContext = this;

        }

        // Get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
            if (Globals.Settings.settings.LogFileFormat != null)
            {
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

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            FileExtensionComboBox.SelectedItem = ".doc";

            //Globals.Settings.Delete();
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
            CheckBox check = (CheckBox)sender;
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
    }
}
