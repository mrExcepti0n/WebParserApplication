using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeverTools
{
    public interface IProxyServerFactory
    {
        Task<List<ProxyServer>> GetProxyServers(string targetSite);
    }
}
