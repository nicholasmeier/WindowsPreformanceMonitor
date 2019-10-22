﻿using System;
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
using WindowsPerformanceMonitor.Backend;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : Window, INotifyPropertyChanged
    {

        private ObservableCollection<ProcessEntry> _applications { get; set; }
        private ProcessEntry _selectedApplication;
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

                ProcessEntry selected = SelectedApplication;

                if (SelectedApplication != null)
                {
                    SelectedApplication = Applications.FirstOrDefault(p => p.Pid == selected.Pid);
                }

                listView.SelectedItem = SelectedApplication;
            });
        }

        private void KillProcess_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex > -1)
            {
                Processes procs = new Processes();
                ProcessEntry procEntry = (ProcessEntry) listView.Items[listView.SelectedIndex];
                procs.Kill(procEntry.Pid);
            }
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

        public ProcessEntry SelectedApplication
        {
            get { return _selectedApplication; }
            set
            {
                _selectedApplication = value;
                OnPropertyChanged(nameof(SelectedApplication));
            }
        }
        #endregion

        private void GeneralView_Closing(object sender, CancelEventArgs e)
        {
            mainWindowRef.Show();
        }
    }
}
