using System;
using System.Linq;
using HtmlAgilityPack;
using System.Collections.Generic;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace S21eimagesrefine
{
    class Program
    {
        public static async Task Main(string[] args)
        {


            var options = new LaunchOptions { Headless = true, ExecutablePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1920,
                    Height = 1280
                });
                await page.GoToAsync("https://www.cosicomodo.it/spesa-online/ricerca?q=8000500227848");

                
                try  {
                    var allResultsSelector = ".large-10 .listing";
                    var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                    await page.WaitForSelectorAsync(allResultsSelector, new WaitForSelectorOptions { Timeout = 10000, Hidden = false });
                    var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                    foreach (string url in urls)
                    {
                        Console.WriteLine($"Url: {url}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                
                

                
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            return;
        }
    }
}
