using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;
using Tesseract;
using WebParserTools.Model;
using Page = PuppeteerSharp.Page;

namespace WebParserTools.Avito
{
    public class AvitoPostParser
    {
        public async Task<PostInformation> GetInfo(Page page, string url)
        {
            var response =await page.GoToAsync(url);

            if (response.Status == HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException("Forbidden");
            }

            if (response.Url.Contains("/blocked"))
            {
                ///todo try recognize captcha
                throw new HttpRequestException("Blocked");
            }

            var getPhoneNumberButton = await page.QuerySelectorAsync("span.item-phone-button-sub-text");

            if (getPhoneNumberButton == null)
            {
                return null;
            }
            await getPhoneNumberButton.ClickAsync();

            string phoneNumber = await GetPhoneNumber(page);
            string authorName = await GetAuthorName(page);

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
