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

namespace WebParserTools
{
    public class AvitoParser : WebParser<PostInformation>
    {

        public AvitoParser(IProxyServerFactory proxyServerFactory, string catalogUrl, params IParseExport[] parsingEndHandlers)
        {
            _proxyServerFactory = proxyServerFactory;
            _catalogUrl = catalogUrl;

            foreach(var parsingEndHandler in parsingEndHandlers)
            {
                ParseEndEvent += parsingEndHandler.OnParseEnd;
            }
        }

        private IProxyServerFactory _proxyServerFactory;
        private string _catalogUrl;
        private string _baseUrl = "https://www.avito.ru";


        private event EventHandler<List<PostInformation>> ParseEndEvent;

        private void OnParseEnd(List<PostInformation> postInformations)
        {
            ParseEndEvent?.Invoke(this, postInformations);            
        }

        private List<string> GetItemsHrefs()
        {
            var web = new HtmlWeb();
            var doc = web.Load(_catalogUrl);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='item__line']");
            var hrefs = nodes.Select(nd => _baseUrl + nd.SelectSingleNode(".//a").Attributes["href"].Value).ToList();
            return hrefs;
        }



        public override async Task<List<PostInformation>> Parse()
        {
            var proxyServers = await _proxyServerFactory.GetProxyServers(_baseUrl);

            string proxyServerAddress = null;
            if (proxyServers.Any())
            {
                proxyServerAddress = $"{proxyServers[0].Host}:{proxyServers[0].Port}";
            }

            var itemsHrefs = GetItemsHrefs();

            var result = await ParseInternal(itemsHrefs, proxyServerAddress);

            OnParseEnd(result);
            return result;
        }



        private async Task<List<PostInformation>> ParseInternal(List<string> itemsHrefs, string proxyServerAddress = null)
        {
            var optionArgs = new List<string> { "--no-sandbox",
                               "--disable-infobars",
                               "--disable-setuid-sandbox",
                               "--ignore-certificate-errors"};

            if (proxyServerAddress != null)
            {
                optionArgs.Add("--proxy-server=" + proxyServerAddress);
            }

            var options = new LaunchOptions
            {
                Headless = false,
                Args = optionArgs.ToArray()
            };

            var result = new List<PostInformation>();       


            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (PuppeteerSharp.Page page = await browser.NewPageAsync())
            {
                foreach (var itemHref in itemsHrefs)
                {
                    var post = await GetInfo(page, itemHref);
                    if (post != null)
                    {
                        result.Add(post);
                    }
                }
            }

            return result;
        }


        private async Task<PostInformation> GetInfo(PuppeteerSharp.Page page, string url)
        {
            await page.GoToAsync(url);


            var getPhoneNumberButton = await page.QuerySelectorAsync("span.item-phone-button-sub-text");

            if (getPhoneNumberButton == null)
            {
                return null;
            }
            await getPhoneNumberButton.ClickAsync();

            var phoneNumber = await GetPhoneNumber(page);
            var authorName = await GetAuthorName(page);

            return new PostInformation
            {
                Author = new Author
                {
                    PhoneNumber = phoneNumber,
                    Name = authorName
                }
            };
        }

        private async Task<string> GetAuthorName(PuppeteerSharp.Page page)
        {

            var sellerInfoName = await page.WaitForSelectorAsync(".seller-info-name.js-seller-info-name a").EvaluateFunctionAsync<string>("element => element.innerHTML");
            sellerInfoName = sellerInfoName.Trim();

            var sellerInfoValueElement = await page.QuerySelectorAsync("div.seller-info-label + div.seller-info-value");
            if (sellerInfoValueElement != null)
            {
                var sellerInfoValue = (await sellerInfoValueElement.EvaluateFunctionAsync<string>("element => element.innerHTML")).Trim();
                sellerInfoName += ": " + sellerInfoValue;
            }

            return sellerInfoName;
        }


        private async Task<string> GetPhoneNumber(PuppeteerSharp.Page page)
        {
            string phoneImage = await page.WaitForSelectorAsync("div.popup-content img").EvaluateFunctionAsync<string>("element => element.src");

            string imageStr = phoneImage.Substring(phoneImage.IndexOf(",") + 1).Trim();
            byte[] imageByte = System.Convert.FromBase64String(imageStr);

            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            {               
                using (var memoryStream = new MemoryStream(imageByte))
                {                   
                    var file = Pix.LoadFromMemory(memoryStream.ToArray());               
                    var page1 = engine.Process(file);
                    return page1.GetText().Trim();
                }            
           }
        }        

    }
}
