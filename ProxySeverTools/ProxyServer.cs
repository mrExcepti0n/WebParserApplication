using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeverTools
{
    public class ProxyServer
    {
        public ProxyServer(string host, int port, string login = null, string password = null)
        {
            Host = host;
            Port = port;

            NeedAuthentication = login == null ? false : true;

            Login = login;
            Password = password;
        }


        public string Host { get; }
        public int Port { get; }

        public bool NeedAuthentication { get; }

        public string Password { get; }
        public string Login { get; }


        public bool CanConnect
        {
            get {
                if (_canConnect == null)
                {
                    throw new InvalidOperationException();
                }
                return _canConnect.Value;
            }
            private set {
                _canConnect = value;
            }
        }

        private bool? _canConnect;

        public async Task InitializeAsync(string url = "https://www.google.com")
        {
            CanConnect = await CheckConnection(url);            
        }       


        public async Task<bool> CheckConnection(string url = "https://www.google.com")
        {
            Uri uri = new Uri(url);
            IWebProxy proxy = new WebProxy(Host, Port);
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myWebRequest.Proxy = proxy;

            try
            {
                using (var response = await myWebRequest.GetResponseAsync())
                {
                    var statusCode = ((HttpWebResponse)response).StatusCode;

                    if (statusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }                
            }
            catch (Exception)
            {
               
            }

            return false; ;
        }


        public static async Task<ProxyServer> CreateProxyServer(string host, int port)
        {
            var proxyServer = new ProxyServer(host, port);
            await proxyServer.InitializeAsync();

            return proxyServer;
        }
    }
}
