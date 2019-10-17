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
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : Window, INotifyPropertyChanged
    {

        private ObservableCollection<ProcessEntry> _applications { get; set; }
        Window mainWindowRef = null;

        public GeneralView(Window mainWindow)
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            Applications = new ObservableCollection<ProcessEntry>();
            mainWindowRef = mainWindow;

        }

        private void UpdateValues(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                Applications = new ObservableCollection<ProcessEntry>(comp.ProcessList.Where(p => p.IsApplication == true));
                listView.ItemsSource = Applications;
            });
        }

        private void DetailedView_Click(object sender, System.EventArgs e)
        {
            this.Close();
            mainWindowRef.Show();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<ProcessEntry> Applications
        {
            get { return _applications; }
            set
            {
                _applications = value;
                OnPropertyChanged(nameof(Applications));
            }
        }
        #endregion
    }
}
