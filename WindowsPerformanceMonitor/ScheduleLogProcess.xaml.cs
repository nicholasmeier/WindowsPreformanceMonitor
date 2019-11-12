using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WindowsPerformanceMonitor.Models;

namespace WindowsPerformanceMonitor
{
    /// <summary>
    /// Interaction logic for ScheduleLogProcess.xaml
    /// </summary>
    public partial class ScheduleLogProcess : Window
    {
        Window mainWindowRef = null;
        ProcessEntry selectedProcess = new ProcessEntry();
        public ScheduleLogProcess(Window mainWindow, ProcessEntry process)
        {
            InitializeComponent();
            mainWindowRef = mainWindow;
            selectedProcess = process;
            loadSchedule();
        }

        private void loadSchedule()
        {
            if (selectedProcess.LogScheduleTime != null)
            {
                TimeTextBox.Text = selectedProcess.LogScheduleTime;
            }

            if (selectedProcess.LogScheduleDuration != null)
            {
                DurationTextBox.Text = selectedProcess.LogScheduleDuration;
            }
        }

        private void SaveScheduleLog_Click(object sender, RoutedEventArgs e)
        {
            if (validateTime(TimeTextBox.Text, DurationTextBox.Text) == 0)
            {
                // valid times
                selectedProcess.LogScheduleTime = TimeTextBox.Text;
                selectedProcess.LogScheduleDuration = DurationTextBox.Text;
                loadSchedule();
                MessageBox.Show("The scheduled log was updated successfully.", "Log Schedule Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ScheduleLogView_Closing(object sender, CancelEventArgs e)
        {
            mainWindowRef.Show();
        }

        private int validateTime(String time, String duration)
        {
            var timeregex = @"^ *(1[0-2]|[1-9]):[0-5][0-9] *(a|p|A|P)(m|M) *$";

            var match = Regex.Match(time, timeregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = MessageBox.Show("The input format is wrong. Example 00:00 AM.", "Time Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            var durationregex = @"^(([0]?[1-9]|1[0-2])(:)([0-5][0-9]))$";

            match = Regex.Match(duration, durationregex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                MessageBoxResult result = MessageBox.Show("The input format is wrong. Example 00:00.", "Duration Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 1;
            }

            return 0;
        }

    }
}
