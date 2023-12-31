﻿using Core;
using log4net;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteAvailabilityChecker
{
    public class CheckerCore : ServiceBase
    {
        private static ILog logger = log4net.LogManager.GetLogger("CheckerCore");

        private static System.Threading.Timer timer = null;

        private IDisposable webApiHandle = null;


        public void Start(string[] args)
        {
            this.OnStart(args);
        }


        protected override void OnStart(string[] args)
        {
            logger.Info("Starting Site availability checker");

            var intervalStr = ConfigurationManager.AppSettings.Get("IntervalInMinutes");
            if (!int.TryParse(intervalStr, out int intervalInMins))
            {
                logger.Error("Check interval in minutes is not parsable. Will use 60 minutes");
                intervalInMins = 60;
            }
            logger.Debug($"Check interval is {intervalInMins} minutes");

            timer = new Timer(timerCallback, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(intervalInMins));

            var portStr = ConfigurationManager.AppSettings.Get("ApiPort");
            if (!int.TryParse(portStr, out int port))
            {
                logger.Error("Port number for API is not parsable. Will use 8181");
                port = 8181;
            }
            LaunchWebApi(port);
        }


        protected void LaunchWebApi(int port)
        {
            // Startup full web-api:
            var webApiHttpsUrl = "http://*:" + port;

            webApiHandle = WebApp.Start<API.WebApiConfig>(webApiHttpsUrl);
        }


            
        private static void timerCallback(object state)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string siteProblemDetail = "No problem";
                    StatusValues siteProblem = StatusValues.Good;

                    var url = new Uri(ConfigurationManager.AppSettings.Get("SiteUrl"));
                    client.BaseAddress = url;
                    var siteResponse = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).GetAwaiter().GetResult();
                    logger.Debug($"Response from {url.ToString()} is {siteResponse.StatusCode} ");
                    if (!siteResponse.IsSuccessStatusCode)
                    {
                        logger.Error($"Site response indicated failure: {siteResponse.StatusCode} {siteResponse.ReasonPhrase}");
                        siteProblem =  StatusValues.NoSite;
                        siteProblemDetail = $"Site response indicated failure: {siteResponse.StatusCode} {siteResponse.ReasonPhrase}\n";
                    }

                    var expectedIp = ConfigurationManager.AppSettings.Get("ExpectedIp");
                    var ip = System.Net.Dns.GetHostAddresses(url.Host)[0];
                    if (ip.ToString() != expectedIp)
                    {
                        logger.Error("IP has changed!");
                        siteProblem =  StatusValues.IpChange;
                        siteProblemDetail += $"IP has changed: was {ip} expected {expectedIp}\n";
                    }
                    else
                    {
                        logger.Debug($"IP is {ip} as expected");
                    }

                    CurrentState.StatusMessage.SerialNumber++;
                    CurrentState.StatusMessage.Timestamp = DateTime.Now;
                    CurrentState.StatusMessage.Status = siteProblem;
                    CurrentState.StatusMessage.StatusDescription = siteProblemDetail;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to check availability of site");
            }
        }





    }
}
