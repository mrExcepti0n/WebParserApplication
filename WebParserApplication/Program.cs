
using AngleSharp;
using ProxySeverTools;
using System.Threading.Tasks;
using AngleSharp.Scripting;
using AngleSharp.Io;
using System.Net.Http;
using System.Linq;
using PuppeteerSharp;
using System;
using WebParserTools;
using Tesseract;
using System.Drawing;

namespace WebParserApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var proxyServerFactory = new HardSetAddressProxyServerFactory();
            var parseExport = new ExcelParseExport(@"avitoResults.xlsx");
            var avitoParser = new AvitoParser(proxyServerFactory, "https://www.avito.ru/rostov-na-donu/lichnye_veschi", parseExport);
            await avitoParser.Parse();
        }

    }
}
