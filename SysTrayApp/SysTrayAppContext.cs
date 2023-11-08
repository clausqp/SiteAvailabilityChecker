using Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysTrayApp
{
    public class SysTrayAppContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        Configuration configWindow = new Configuration();

        System.Threading.Timer checkStatusTimer = null;

        StatusLamp lamp;
        IntPtr hGreen;
        IntPtr hRed;
        IntPtr hYellow;
        IntPtr hOff;


        public SysTrayAppContext()
        {
            lamp = new StatusLamp(new Size(16, 16));
            hGreen = lamp.SetColor(Color.Green).GetHicon();
            hRed = lamp.SetColor(Color.Red).GetHicon();
            hYellow = lamp.SetColor(Color.DarkOrange).GetHicon();
            hOff = lamp.SetColor(Color.Gray).GetHicon();

            ToolStripMenuItem configMenuItem = new ToolStripMenuItem("Configuration");//, new EventHandler(ShowConfig));
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");//, new EventHandler(Exit));
            configMenuItem.Click += ShowConfig;
            exitMenuItem.Click += Exit;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(hOff); // SysTrayApp.Properties.Resources.AppIcon2;
            notifyIcon.Text = "Starting";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(configMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            notifyIcon.Visible = true;

            checkStatusTimer = new System.Threading.Timer(CheckStatusTimerCallback, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }




        private void CheckStatusTimerCallback(object state)
        {
            var msg = new ProcessCommunication().ReadStatusMessage();
            if (msg == null)
            {
                notifyIcon.Text = "not running";
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hRed);
            }
            else if (msg.Timestamp.AddMinutes(20) < DateTime.Now)
            {
                notifyIcon.Text = "old response!";
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hYellow);
            }
            else
            {
                notifyIcon.Text = msg.StatusMessage;
                switch (msg.Status)
                {
                    case StatusValues.None:
                        notifyIcon.Icon = System.Drawing.Icon.FromHandle(hOff);
                        break;
                    case StatusValues.Good:
                        notifyIcon.Icon = System.Drawing.Icon.FromHandle(hGreen);
                        break;
                    case StatusValues.IpChange:
                    case StatusValues.NoSite:
                        notifyIcon.Icon = System.Drawing.Icon.FromHandle(hRed);
                        break;
                }
            }
        }


        void ShowConfig(object sender, EventArgs e)
        {
            // If we are already showing the window, merely focus it.
            if (configWindow.Visible)
            {
                configWindow.Activate();
            }
            else
            {
                configWindow.ShowDialog();
            }
        }


        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;
            Application.Exit();
        }


    }
}
