using Cookie = OpenQA.Selenium.Cookie;
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


            FirefoxOptions options = new FirefoxOptions();
            options.AddArguments("--headless");

            using (IWebDriver driver = new FirefoxDriver(options))
            {
                driver.Manage().Window.Maximize();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                try
                {
                    driver.Navigate().GoToUrl("https://www.cosicomodo.it");

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzid",
                        "LuK8CpFtsds0UnWPg0Yq9EaAAHfZyg8Rp9pNSaztL/Wf0as4oMMmVeLQJYd9dhck0yVRT95O7IgTCJZdF7eH5PGiegm1qPcARfXoFe5fnOQUzbqWm8btdCPlIvd8bBr/Vu8f+1VjkMK/dIJi+4odVe1c5xj8DwfPMU8TQeAHn03NhJCDL4Bs0iCr0DyMhfYtXe+Ptfvt/dc7cZVFKFs3qdcOre3+PZx4LftsqjQTpdeJBVHEdN9z6uj58SAbM/n7F/7vdv3OOaKoNdc75IY6xA==",
                        ".www.cosicomodo.it",
                        "/",
                        time
                    ));

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "rbzsessionid",
                        "d78e471a2f818c65a15b7de836a1f801",
                        ".www.cosicomodo.it",
                        "/",
                        time
                    ));

                    driver.Manage().Cookies.AddCookie(new Cookie(
                        "GCLB",
                        "CIHDvcbJmeX4twE",
                        "www.cosicomodo.it",
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
                    image.Save(pathToFile.Replace(".jpg",".png"), ImageFormat.Png);
                }
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

    }
}
