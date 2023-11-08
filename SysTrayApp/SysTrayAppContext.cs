using Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysTrayApp
{
    public class SysTrayAppContext : ApplicationContext
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SysTrayAppContext));

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
            logger.Debug("Starting");
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
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(hOff); 
            notifyIcon.Text = "Starting";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(configMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            notifyIcon.Visible = true;

            checkStatusTimer = new System.Threading.Timer(CheckStatusTimerCallback, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(60));
            logger.Info("SysTrayApp Started");
        }


        private Core.Message RequestStatus()
        {
            try
            {
                logger.Debug("Requesting status from service");
                var url = $"http://localhost:8180/api/v1/getstate";
                string param = "";
                var response = Core.Helpers.Net.SendRequest(url, param, System.Net.Http.HttpMethod.Get);
                var errorCode = (int)response.StatusCode;
                if (!response.IsSuccessStatusCode)
                {
                    logger.Warn($"GetState failed to retreive status, error code {errorCode}");
                    return null;
                }

                var message = JsonConvert.DeserializeObject<Core.Message>(Core.Helpers.Net.ExtractContentFromResponse(response));
                logger.Debug($"GetState returned {message.Status}, {message.SerialNumber}, {message.StatusDescription}");
                return message;
            }
            catch (Exception ex)
            {
                logger.Error($"GetState failed", ex);
                return null;
            }
        }

        private void CheckStatusTimerCallback(object state)
        {
            var msg = RequestStatus();
            if (msg == null)
            {
                notifyIcon.Text = "Not running";
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hRed);
                logger.Error("service not running");
            }
            else if (msg.SerialNumber == 0)
            {
                notifyIcon.Text = "Just started..";
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hYellow);
                logger.Warn("service just started");
            }
            else if (msg.Timestamp.AddMinutes(20) < DateTime.Now)
            {
                notifyIcon.Text = "Old response!";
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hYellow);
                logger.Warn($"service response is {DateTime.Now.Subtract(msg.Timestamp).TotalMinutes} minutes old");
            }
            else
            {
                notifyIcon.Text = msg.StatusDescription;
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
                logger.Info($"service response {msg.Status} {msg.StatusDescription} and {DateTime.Now.Subtract(msg.Timestamp).TotalMinutes} minutes old");
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
