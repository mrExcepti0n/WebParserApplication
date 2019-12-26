using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxySeverTools
{
    public class HeadlessChromeRequestProxySeverFactory : IProxyServerFactory
    {
        public async Task<List<ProxyServer>> GetProxyServers(string targetSite)
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

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (PuppeteerSharp.Page page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://hidemy.name/ru/proxy-list/?type=s&anon=4#list");

                var table = await page.QuerySelectorAsync("table");
                var mHtml = await table.EvaluateFunctionAsync<string>("el => el.innerHTML");



                var rows = await page.QuerySelectorAllAsync("table tbody tr td:first-child");

                foreach (var row in rows)
                {
                    var html = await row.EvaluateFunctionAsync<string>("el => el.innerHTML");
                }
            }

            throw new NotImplementedException();
        }
    }
}
