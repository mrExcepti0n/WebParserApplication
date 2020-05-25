using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeverTools
{
    public class HardSetAddressProxyServerFactory : IProxyServerFactory
    {
        public async Task<List<ProxyServer>> GetProxyServers(string targetSite)
        {
            var proxyServers = new List<ProxyServer>
            {
                new ProxyServer("51.158.186.141", 8080)
                //new ProxyServer("194.226.34.132", 5555),
                //new ProxyServer("93.171.164.251", 8080),
                //new ProxyServer("51.68.207.81", 80),
            };
            
            foreach (var proxyServer in proxyServers)
            {
                await proxyServer.InitializeAsync(targetSite);
            }

            return proxyServers.Where(ps => ps.CanConnect).ToList();
        }
    }
}
