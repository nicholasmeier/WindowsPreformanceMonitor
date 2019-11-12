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
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for ProcessTree.xaml
    /// </summary>
    public partial class ProcessTree : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<ProcessEntry> _procListTreeView { get; set; }
        public ProcessTree()
        {
            InitializeComponent();
            HardwareObserver observer = new HardwareObserver(UpdateValues);
            Globals.provider.Subscribe(observer);
            _procListTreeView = new ObservableCollection<ProcessEntry>();
        }


        public void UpdateValues(ComputerObj comp)
        {
            if (comp.Tab == 1)
            {
                UpdateList(comp);
            }

            return;
        }

        public void UpdateList(ComputerObj comp)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (comp.ProcessTree != null)
                {
                    procListTreeView = new ObservableCollection<ProcessEntry>(comp.ProcessTree.OrderByDescending(p => p.Cpu));
                    Loading.Text = "";
                }

                UpdateProcessTreeView();
            });
        }

        public void UpdateProcessTreeView()
        {
            ProcessTreeView.Items.Clear();
            // make the tree with parent, child, and subchild
            foreach (ProcessEntry parent in _procListTreeView)
            {
                TreeViewItem ParentItem = new TreeViewItem();
                ParentItem.Header ="[" + parent.Name + ", pid: " + parent.Pid + "]";
                // check to see if they have a child to add
                if (parent.ChildProcesses.Count > 0)
                {
                    foreach (ProcessEntry child in parent.ChildProcesses)
                    {
                        TreeViewItem ChildItem = new TreeViewItem();
                        ChildItem.Header = "[" + child.Name + ", pid: " + child.Pid + "]";
                        // check to see if they have a sub child to add
                        if (child.ChildProcesses.Count > 0)
                        {
                            foreach (ProcessEntry subchild in child.ChildProcesses)
                            {
                                //get the subchild and add it to the child
                                TreeViewItem SubChildItem = new TreeViewItem();
                                SubChildItem.Header = "[" + subchild.Name + ", pid: " + subchild.Pid + "]";
                                ChildItem.Items.Add(SubChildItem);
                            }
                        }
                        ParentItem.Items.Add(ChildItem);
                    }
                }
                ProcessTreeView.Items.Add(ParentItem);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<ProcessEntry> procListTreeView
        {
            get { return _procListTreeView; }
            set
            {
                _procListTreeView = value;
                OnPropertyChanged(nameof(procListTreeView));
            }
        }
    }
}
