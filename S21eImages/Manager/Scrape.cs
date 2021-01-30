using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Cookie = OpenQA.Selenium.Cookie;
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
using S21eImages.Model;

namespace S21eImages.Manager
{
    public class Scrape : IScrape
    {

        private IWebDriver _driver;

        public void Do()
        {

            string conf = Environment.GetEnvironmentVariable("DATA_PATH");
            string path = Path.Combine(Path.GetDirectoryName(conf) ?? throw new NullReferenceException("data path is null"), "catalog_product_entity.json");
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
                        continue;
                    }

                    int.TryParse(sku, out int n);
                    if (n < GetLastItem())
                    {
                        continue;
                    }

                    using (var db = new SQLiteDBContext())
                    {

                        var product = db.Products
                            .FirstOrDefault(b => b.Sku == sku);

                        DateTime foo = DateTime.Today;
                        int unixTime = Convert.ToInt32(((DateTimeOffset)foo).ToUnixTimeSeconds());

                        if (product is null)
                        {
                            db.Products.Add(new Products { Sku = sku, Attempts = 1, UpdatedAt = unixTime });
                            db.SaveChanges();
                        } else
                        {
                            if (product.Attempts < 100 && product.UpdatedAt < unixTime)
                            {
                                product.Attempts += 1;
                                product.UpdatedAt = unixTime;
                                db.SaveChanges();
                            }
                            else continue;
                        }

                    }

                    if (rowSum % 25 == 0)
                    {
                        if (_driver is IWebDriver)
                        {
                            _driver.Manage().Cookies.DeleteAllCookies();
                            _driver.Close();
                        }

                        DateTime time = DateTime.Now.AddYears(1);
                        _driver = InitBrowser();
                        _driver.Navigate().GoToUrl("https://www.cosicomodo.it");
                        _driver.Manage().Cookies.AddCookie(new Cookie(
                            "cosicomodo_provisionalcap",
                            "\"37030 - Montecchia di Crosara\"",
                            "www.cosicomodo.it",
                            "/",
                            time
                        ));

                        _driver.Manage().Cookies.AddCookie(new Cookie(
                            "familanord_anonymous_preferred_base_store",
                            "MAXIDI_FAMILA_019861",
                            "www.cosicomodo.it",
                            "/",
                            time
                        ));

                        _driver.Manage().Cookies.AddCookie(new Cookie(
                            "familanord_provisionalcap",
                            "\"37030 - Montecchia di Crosara\"",
                            "www.cosicomodo.it",
                            "/",
                            time
                        ));

                        _driver.Manage().Cookies.AddCookie(new Cookie(
                            "is_provisionalcap_from_aggregator",
                            "true",
                            "www.cosicomodo.it",
                            "/",
                            time
                        ));

                        _driver.Manage().Cookies.AddCookie(new Cookie(
                            "cookies-disclaimer-v1",
                            "true",
                            "www.cosicomodo.it",
                            "/",
                            time
                        ));
                    }

                    
                    Console.WriteLine($"Cerco l'articolo {sku}: {descr}");
                    string searchPageUrl = $"https://www.cosicomodo.it/familanord/san-bonifacio-villanova/ricerca?q={ean}";
                    WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(25));
                    _driver.Navigate().GoToUrl(searchPageUrl);

                    IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".dobody-container .listing")));
                    IList<IWebElement> links = firstResult.FindElements(By.TagName("a"));

                    if (!links.Any())
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

                    _driver.Navigate().GoToUrl(productPageUrl);
                    IWebElement secondResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".image-container")));
                    IList<IWebElement> images = secondResult.FindElements(By.CssSelector(".image-container .slick-slide a.mfp-zoom"));

                    for (int i = 0; i < images.Count() && i < 5; i++)
                    {
                        string image = images.ElementAt(i).GetAttribute("href");
                        if (string.IsNullOrEmpty(image)) continue;
                        Console.WriteLine(image);
                        SaveImage(image, sku + "_" + i + ".jpg", ImageFormat.Jpeg);
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
        private static IWebDriver InitBrowser ()
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
        /// controlla la presenza di immagini già scaricate
        /// </summary>
        /// <returns>int</returns>
        private int GetLastItem()
        {

            string targetDirectory = Environment.GetEnvironmentVariable("ASSETS_PATH");
            string fileEntry = Directory.GetFiles(Path.Combine(targetDirectory, "product_images"))
                        .OrderByDescending(d => d)
                        .ToArray()
                        .First();
            string fileName = Path.GetFileName(fileEntry);
            string sku = fileName.Split('_')[0];
            int.TryParse(sku, out int j);

            return j;
        }

        /// <summary>
        /// Salvataggio delle immagini
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        private static void SaveImage(string imageUrl, string filename, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            DrawingImage image = DrawingImage.FromStream(stream ?? throw new NullReferenceException("stream is null"));

            if (image != null)
            {

                string conf = Environment.GetEnvironmentVariable("ASSETS_PATH");
                string pathToFile = Path.Combine(Path.GetDirectoryName(conf) ?? throw new NullReferenceException("assets path is null"), "product_images/" + filename);

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
