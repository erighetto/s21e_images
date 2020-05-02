using System;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;

namespace S21eimagesrefine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /**
             * Esempio 1
             */
            ScrapingBrowser browser = new ScrapingBrowser();

            WebPage homePage = browser.NavigateToPage(new Uri("http://www.bing.com/"));

            PageWebForm form = homePage.FindFormById("sb_form");
            form["q"] = "scrapysharp";
            form.Method = HttpVerb.Get;
            WebPage resultsPage = form.Submit();

            HtmlNode[] resultsLinks = resultsPage.Html.CssSelect("div.sb_tlst h3 a").ToArray();

            WebPage blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();


            /**
             * Esempio 2
             */
            var url = "https://www.cosicomodo.it/spesa-online/ricerca?q=8000500227848&empty-cookie=true";
            var webGet = new HtmlWeb();
            if (webGet.Load(url) is HtmlDocument document)
            {
                var nodes = document.DocumentNode.CssSelect("ul.listing li").ToList();
                foreach (var node in nodes)
                {
                    Console.WriteLine("Articolo: " + node.CssSelect("h3 a").Single().InnerText);
                }
            }

        }
    }
}
