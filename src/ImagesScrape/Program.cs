using Cookie = OpenQA.Selenium.Cookie;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System;
using System.Text;
using RandomUserAgent;

namespace ImagesScrape
{
    class Program
    {

        public static void Main(string[] args)
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string conf = configuration.GetSection("AppSetting")["DataPath"];
            string path = Path.Combine(Path.GetDirectoryName(conf), "catalog_product_entity.json");
            string json = File.ReadAllText(path);
            DateTime time = new DateTime(2020, 12, 31, 23, 02, 03);

            Proxy proxy = new Proxy
            {
                Kind = ProxyKind.Manual,
                IsAutoDetect = false,
                SslProxy = GetAvailableProxy()
            };

            string userAgent = RandomUa.RandomUserAgent;

            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            //options.Proxy = proxy;
            options.AddArgument("ignore-certificate-errors");
            options.AddArgument($"--user-agent={userAgent}");

            using (IWebDriver driver = new FirefoxDriver(options))
            {
                driver.Manage().Window.Maximize();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                try
                {

                    driver.Navigate().GoToUrl("https://www.cosicomodo.it");

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzid",
                        "qRNV0kjQC7Jjolpkl0IuG8QD6P1JCCVcXVTYZKbEYPCnqrXFG+xNSgsweLIFBK+FyT5F06S0PWyidcOvYYgq57vEXLA3bTzjpl9WlWrTQnDCrf4hJ2OEDBQre672cDyBX7gMEjfS62L5mahqdPQQx0AclekiNnFPDbUnwN2JfKggYB/EIsqJPs6QHHZXy7l5prMGeCjZR95tpGgbO4jqya+rFHVTMxnqwkdXdvlxL+pqT2iz3zKUgXBJcGhHjggU",
                        ".www.cosicomodo.it",
                        "/",
                        time
                    ));

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzsessionid",
                        "906d5ae8fc4e6355a5145ad557448d68",
                        ".www.cosicomodo.it",
                        "/",
                        time
                    ));

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "cookies-disclaimer-v1",
                        "true",
                        "www.cosicomodo.it",
                        "/",
                        time
                    ));


                    CatalogProductEntities tblarticoliObj = JsonConvert.DeserializeObject<CatalogProductEntities>(json);

                    foreach (CatalogProductEntity element in tblarticoliObj.CatalogProductEntity.OrderBy(o => o.CodArt))
                    {
                        var sku = element.CodArt;
                        var ean = element.CodEan;
                        var descr = element.DescArticolo;
                        if (string.IsNullOrEmpty(ean))
                        {
                            continue;
                        }

                        //int.TryParse(sku, out int number);
                        //if (number < 2300309)
                        //{
                        //    continue;
                        //}

                        Console.WriteLine($"Cerco l'articolo {sku}: {descr}");
                        string searchPageUrl = $"https://www.cosicomodo.it/spesa-online/ricerca?q={ean}";


                        driver.Navigate().GoToUrl(searchPageUrl);
                        IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".large-10 .listing")));
                        IList<IWebElement> links = firstResult.FindElements(By.TagName("a"));

                        if (links.Count() < 1)
                        {
                            continue;
                        }

                        IWebElement link = links.First(e => e.GetAttribute("href") != "#");
                        string productPageUrl = link.GetAttribute("href");
                        if (string.IsNullOrEmpty(productPageUrl))
                        {
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
            Image image = Image.FromStream(stream);

            if (image != null)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
                string conf = configuration.GetSection("AppSetting")["AssetsPath"];
                string pathToFile = Path.Combine(Path.GetDirectoryName(conf), "product_images/" + filename);

                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                {
                    image.Save(pathToFile, format);

                }
                else
                {
                    using (var b = new Bitmap(image.Width, image.Height))
                    {
                        b.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(image, 0, 0);
                        }

                        b.Save(pathToFile, format);
                    }
                }

            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

        /// <summary>
        /// Get random proxy
        /// </summary>
        /// <returns></returns>
        public static string GetAvailableProxy()
        {
            WebClient client = new WebClient();
            string proxy11key = Environment.GetEnvironmentVariable("PROXY_11_KEY");
            Stream stream = client.OpenRead($"https://proxy11.com/api/proxy.json?key={proxy11key}");
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string responseString = reader.ReadToEnd();

            Proxies proxies = JsonConvert.DeserializeObject<Proxies>(responseString);
            Random random = new Random();
            int k = random.Next(0, proxies.Data.Count);
            Datum value = proxies.Data[k];

            return value.Ip + ":" + value.Port;
        }

    }
}
