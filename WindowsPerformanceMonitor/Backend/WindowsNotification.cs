using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsPerformanceMonitor.Backend
{
    class WindowsNotification
    {
        private readonly NotifyIcon _notifyIcon;
        public WindowsNotification()
        {
            _notifyIcon = new NotifyIcon();
            // Extracts your app's icon and uses it as notify icon
            _notifyIcon.Icon = new System.Drawing.Icon("../../Graphics/WindowsPerformanceMonitor.ico");
            // Hides the icon when the notification is closed
            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
        }

        public void ShowNotification(string title, string message)
        {
            _notifyIcon.Visible = true;
            // Shows a notification with specified message and title
            _notifyIcon.ShowBalloonTip(50000, title, message, ToolTipIcon.Warning);
        }
    }
}
