using PuppeteerSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Parser.Ast;

namespace ProxySeverTools
{
    public class HeadlessChromeRequestProxySeverFactory : IProxyServerFactory
    {
        public async Task<List<ProxyServer>> GetProxyServers(string targetSite)
        {
            var proxyServers = new ConcurrentQueue<ProxyServer>();

            using (var browser = await GetBrowser())
            using (Page page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://hidemy.name/ru/proxy-list/?type=s&anon=4#list");
                //wait for redirect (avoiding automatic request)
                await Task.Delay(TimeSpan.FromSeconds(10));

                var rows = await page.QuerySelectorAllAsync("table tbody tr");
                //limit list to increase speed
                var tasks = rows.Take(5).Select(row => LoadProxy(row, proxyServers)).ToList();
                await Task.WhenAll(tasks);
            }
            return proxyServers.ToList();
        }

        private async Task LoadProxy(ElementHandle row, ConcurrentQueue<ProxyServer> proxyServers)
        {
            var addressQuery = await row.QuerySelectorAsync("td:first-child");
            var address = await addressQuery.EvaluateFunctionAsync<string>("el => el.innerHTML");

            var portQuery = await row.QuerySelectorAsync("td:nth-child(2)");
            var port = await portQuery.EvaluateFunctionAsync<int>("el => el.innerHTML");
            var proxyServer = new ProxyServer(address, port);
            await proxyServer.InitializeAsync();
            if (proxyServer.CanConnect)
            {
                proxyServers.Enqueue(proxyServer);
            }
        }
        
        private async Task<Browser> GetBrowser()
        {
            var options = new LaunchOptions
            {
                Headless = false,
                Args = new[] { "--no-sandbox",
                    "--disable-infobars",
                    "--disable-setuid-sandbox",
                    "--ignore-certificate-errors", }
            };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            return await Puppeteer.LaunchAsync(options);
        }
    }
}
