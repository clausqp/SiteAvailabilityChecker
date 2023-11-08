using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class Net
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Net));

        static HttpClient _client = null;


        public static string AuthKey = "ctAuthKey";
        public static string AuthValue = "C551E850-5BD5-4159-8505-85A11C75E4DB";

        private static System.Net.Http.HttpClientHandler requestHandler;


        private static HttpClientHandler RequestHandler
        {
            get
            {
                if (requestHandler == null)
                {
                    // Instantiate handler and add client certificate and validator:
                    requestHandler = new HttpClientHandler();

                    X509Certificate2 certificate = SecurityHelper.FindBySubjectName("tnc01223.stt.local"); //  CoreApiSettings.Instance.ClientCertificateSubject);
                    if (certificate == null)
                    {
                        Console.WriteLine($"Error: Failed to add certificate to request");
                        logger.Error($"Failed to add certificate to request");
                        throw new ApplicationException("Cannot find certificate for this machine");
                    }

                    Console.WriteLine($"Adding certificate {certificate.SubjectName.Name} to request");
                    logger.Debug($"**** Adding certificate {certificate.SubjectName.Name} to request handler");
                    requestHandler.ClientCertificates.Add(certificate);
                    requestHandler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;
                    requestHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                }

                return requestHandler;
            }
        }




        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //logger.Warn($"Certificate problem: {sslPolicyErrors}, isuer: {certificate.Issuer}, subject: {certificate.Subject}");
            return true;
        }




        private static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    // Use handler to create client:
                    _client = new HttpClient(RequestHandler, false)
                    {
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _client;
            }
        }



        public static HttpResponseMessage SendRequest(string baseUrl, string parameters, HttpMethod method)
        {
            string url = baseUrl;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                url += $"?{parameters}";
            }
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = method;
            httpRequestMessage.Headers.Add(AuthKey, AuthValue);
            httpRequestMessage.RequestUri = new Uri(url);

            var tsk1 = Client.SendAsync(httpRequestMessage);
            tsk1.Wait();
            var response = tsk1.Result;
            return response;
        }




        public static string ExtractContentFromResponse(HttpResponseMessage response)
        {
            var tsk2 = response.Content.ReadAsStringAsync();
            tsk2.Wait();
            string apiResponse = tsk2.Result;
            return apiResponse;
        }



    }
}
