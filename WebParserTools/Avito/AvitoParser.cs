using HtmlAgilityPack;
using ProxySeverTools;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;
using WebParserTools.Abstractions;
using WebParserTools.Model;

namespace WebParserTools.Avito
{
    public class AvitoParser : WebParser<PostInformation>
    {
        public AvitoParser(IProxyServerFactory proxyServerFactory, string catalogUrl)
        {
            _proxyServerFactory = proxyServerFactory;
            _catalogUrl = catalogUrl;
            _avitoPostParser = new AvitoPostParser();
        }

        private readonly IProxyServerFactory _proxyServerFactory;
        private readonly string _catalogUrl;
        private string _baseUrl = "https://www.avito.ru";
        private readonly AvitoPostParser _avitoPostParser;


        public event EventHandler<ParseCompletedEventArgs> ParseCompleted;

        private void OnParseCompleted(List<PostInformation> postInformations)
        {
            ParseCompleted?.Invoke(this, new ParseCompletedEventArgs(postInformations));
        }

        public override async Task<List<PostInformation>> Parse()
        {
            List<string> itemsHrefs = GetItemsHrefs();
            List<ProxyServer> proxyServers = await _proxyServerFactory.GetProxyServers(_baseUrl);

            var proxyServerAddresses = new Queue<string>(proxyServers.Select(ps => $"{ps.Host}:{ps.Port}"));

            var result = await ParseInternal(new Queue<string>(itemsHrefs), proxyServerAddresses);

            OnParseCompleted(result);
            return result;
        }

        private List<string> GetItemsHrefs()
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(_catalogUrl);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='item__line']");
            return nodes.Select(nd => _baseUrl + nd.SelectSingleNode(".//a").Attributes["href"].Value).ToList();
        }

        private async Task<List<PostInformation>> ParseInternal(Queue<string> itemsHrefs, Queue<string> proxyServerAddressCollection = null)
        {
            List<PostInformation> postInformations = new List<PostInformation>();
            try
            {
                string proxyAddress = proxyServerAddressCollection.Any() ? proxyServerAddressCollection.Dequeue() : null;

                using (var browser = await GetBrowser(proxyAddress))
                using (PuppeteerSharp.Page page = await browser.NewPageAsync())
                {
                    while (itemsHrefs.Any())
                    {
                        var post = await _avitoPostParser.GetInfo(page, itemsHrefs.Dequeue());
                        if (post != null)
                        {
                            postInformations.Add(post);
                        }
                    }
                }

            }
            //todo concretize exception
            catch (Exception exception)
            {
                if (proxyServerAddressCollection.Any())
                {
                    postInformations.AddRange(await ParseInternal(itemsHrefs, proxyServerAddressCollection));
                }
            }
            return postInformations;
        }


        private async Task<Browser> GetBrowser(string proxyAddress)
        {
            var options = GetPuppeteerOptions(proxyAddress);
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            return await Puppeteer.LaunchAsync(options);
        }

        private LaunchOptions GetPuppeteerOptions(string proxyAddress)
        {
            var optionArgs = new List<string> { "--no-sandbox",
                "--disable-infobars",
                "--disable-setuid-sandbox",
                "--ignore-certificate-errors"};

            if (!string.IsNullOrWhiteSpace(proxyAddress))
            {
                optionArgs.Add($"--proxy-server={proxyAddress}");
            }

            return new LaunchOptions
            {
                Headless = false,
                Args = optionArgs.ToArray()
            };
        }
    }
}
