using System;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;

namespace S21eimagesrefine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ScrapingBrowser browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;

            WebPage resultsPage = browser.NavigateToPage(new Uri("https://www.cosicomodo.it/spesa-online/ricerca?q=8000500227848&empty-cookie=true"));

            HtmlNode[] resultsLinks = resultsPage.Html.CssSelect("div.filters-result ul li article div.info h3 a").ToArray();

            WebPage productPage = resultsPage.FindLinks(By.Text("nutella B-ready 6 x 22 g")).Single().Click();

            Console.WriteLine("Hello World!");
        }
    }
}
