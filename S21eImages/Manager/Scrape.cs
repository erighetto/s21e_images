using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using DrawingImage = System.Drawing.Image;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System;
using RandomUserAgent;

namespace S21eImages
{
    public class Scrape : IScrape
    {

        public IWebDriver driver;

        public void Do()
        {

            string conf = Environment.GetEnvironmentVariable("DATA_PATH");
            string path = Path.Combine(Path.GetDirectoryName(conf), "catalog_product_entity.json");
            string json = File.ReadAllText(path);

            try
            {

                CatalogProductEntities tblarticoliObj = JsonConvert.DeserializeObject<CatalogProductEntities>(json);

                int rowSum = 0;
                foreach (CatalogProductEntity element in tblarticoliObj.CatalogProductEntity.OrderBy(o => o.CodArt))
                {
                    var sku = element.CodArt;
                    var ean = element.CodEan;
                    var descr = element.DescArticolo;
                    if (string.IsNullOrEmpty(ean))
                    {
                        rowSum++;
                        continue;
                    }

                    //int.TryParse(sku, out int number);
                    //if (number < 68522895)
                    //{
                    //    continue;
                    //}

                    Console.WriteLine($"Cerco l'articolo {sku}: {descr}");
                    string searchPageUrl = $"https://www.cosicomodo.it/spesa-online/ricerca?q={ean}";

                    if (rowSum == 0 || rowSum % 25 == 0)
                    {
                        if (driver is IWebDriver)
                        {
                            driver.Close();
                        }

                        driver = InitBrowser();
                    }
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    driver.Navigate().GoToUrl(searchPageUrl);
                    IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".large-10 .listing")));
                    IList<IWebElement> links = firstResult.FindElements(By.TagName("a"));

                    if (links.Count() < 1)
                    {
                        rowSum++;
                        continue;
                    }

                    IWebElement link = links.First(e => e.GetAttribute("href") != "#");
                    string productPageUrl = link.GetAttribute("href");
                    if (string.IsNullOrEmpty(productPageUrl))
                    {
                        rowSum++;
                        continue;
                    }
                    Console.WriteLine(productPageUrl);

                    driver.Navigate().GoToUrl(productPageUrl);
                    IWebElement secondResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".image-container")));
                    IList<IWebElement> images = secondResult.FindElements(By.CssSelector(".image-container .slick-slide a.mfp-zoom"));

                    for (int i = 0; i < images.Count() && i < 5; i++)
                    {
                        string image = images.ElementAt(i).GetAttribute("href");
                        if (!string.IsNullOrEmpty(image))
                        {
                            Console.WriteLine(image);
                            SaveImage(image, sku + "_" + i + ".jpg", ImageFormat.Jpeg);
                        }

                    }

                    rowSum++;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Thread.Sleep(3000);
            }

        }

        /// <summary>
        /// Inizializza il browser
        /// </summary>
        /// <returns></returns>
        public static IWebDriver InitBrowser ()
        {
            string userAgent = RandomUa.RandomUserAgent;

            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            options.AddArgument($"--user-agent={userAgent}");
            options.AddArgument("--width=1366");
            options.AddArgument("--height=768");
            options.SetPreference("dom.webdriver.enabled", false);
            options.SetPreference("useAutomationExtension", false);

            IWebDriver driver = new FirefoxDriver(options);

            return driver;
        }

        /// <summary>
        /// Salvataggio delle immagini
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public static void SaveImage(string imageUrl, string filename, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            DrawingImage image = DrawingImage.FromStream(stream);

            if (image != null)
            {

                string conf = Environment.GetEnvironmentVariable("ASSETS_PATH");
                string pathToFile = Path.Combine(Path.GetDirectoryName(conf), "product_images/" + filename);

                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                {
                    image.Save(pathToFile, format);

                }
                else
                {
                    using var b = new Bitmap(image.Width, image.Height);
                    b.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(image, 0, 0);
                    }

                    b.Save(pathToFile, format);
                }

            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}
