using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeverTools
{
    public class WebRequestProxyServerFactory : IProxyServerFactory
    {
        public Task<List<ProxyServer>> GetProxyServers(string targetSite)
        {
            var url = "http://free-proxy.cz/ru/proxylist/country/all/https/ping/level1";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            HtmlNode table = doc.GetElementbyId("logo");


            var tableRows = table.SelectNodes(".//tbody tr");
            var nodes = doc.DocumentNode.SelectNodes("//table[@class='item__line']");



            //var requester = new DefaultHttpRequester();
            //requester.Headers["User-Agent"] = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.14; rv:68.0) Gecko/20100101 Firefox/68.0";


            //var config = Configuration.Default.WithJs().WithMetaRefresh().With(requester).WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true, IsNavigationDisabled = false });
            ////;
            //var context = BrowsingContext.New(config);
            //var document = await context.OpenAsync("http://free-proxy.cz/ru/proxylist/country/all/https/ping/level1");

            throw new NotImplementedException();
        }
    }
}
