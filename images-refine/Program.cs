﻿using Cookie = OpenQA.Selenium.Cookie;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using S21eimagesrefine.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System;
using System.Text;
using RandomUserAgent;

namespace S21eimagesrefine
{
    class Program
    {

        public static void Main(string[] args)
        {

            string conf = ConfigurationManager.AppSettings["DataPath"];
            string path = Path.Combine(Path.GetDirectoryName(conf), "catalog_product_entity.json");
            string json = File.ReadAllText(path);
            DateTime time = new DateTime(2020, 12, 31, 23, 02, 03);

            Proxy proxy = new Proxy
            {
                Kind = ProxyKind.Manual,
                IsAutoDetect = false,
                SslProxy = getAvailableProxy()
            };

            string userAgent = RandomUa.RandomUserAgent;

            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            //options.Proxy = proxy;
            //options.AddArgument("ignore-certificate-errors");
            //options.AddArgument($"--user-agent={userAgent}");

            using (IWebDriver driver = new FirefoxDriver(options))
            {
                driver.Manage().Window.Maximize();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                try
                {
                    //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    //js.ExecuteScript("window.key = \"blahblah\";");
                    driver.Navigate().GoToUrl("https://www.cosicomodo.it");

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzid",
                        "WiX9Yj63ipoEanWsCkRi5g9723qLKUZmO5uAGMfjLajVPkW3C84Kk+e5/VpDDTKuzyad49HO/4QkblOxNGLt61r+Y183VCNhAPDPtGhn2tUu4a61c87kWGplrRrbak4wTJDh0N/fFsQkAmhnzkxA8hDCAEO6hxeMo/hUrPdSSITUjho3lf1JZdwd5S/2rP0jl5prIxmxXaZ3fFnIf9l8xM0U9FjS3qh/k76NhnhpWa1BOHr/Il31aDO8firsKYRJ",
                        ".www.cosicomodo.it",
                        "/",
                        time
                    ));

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzsessionid",
                        "d3323152acc5c5d4818b1f6a9164758b",
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

                    foreach (CatalogProductEntity element in tblarticoliObj.CatalogProductEntity)
                    {
                        var sku = element.CodArt;
                        var ean = element.CodEan;
                        var descr = element.DescArticolo;
                        if (string.IsNullOrEmpty(ean))
                        {
                            continue;
                        }

                        //int number;
                        //int.TryParse(sku, out number);
                        //if (number < 4066551)
                        //{
                        //    continue;
                        //}

                        Console.WriteLine($"Cerco l'articolo #{sku}: {descr}");
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
                string conf = ConfigurationManager.AppSettings["AssetsPath"];
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
        public static string getAvailableProxy()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://proxy11.com/api/proxy.json?key=MTM4Mg.XucDgw.yKXb8LhuqJ4mWTaugo93hI9bYQ4");
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
