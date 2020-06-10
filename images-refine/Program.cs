using Cookie = OpenQA.Selenium.Cookie;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using S21emodellayer.Model;
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
            string path = Path.Combine(Path.GetDirectoryName(conf), "tblarticolo.json");
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
                        "LuK8CpFtsds0UnWPg0Yq9Ajh62mCOZM12MgGHtsLSm2+jrrFMpd9u2G4Nuva4Gycg00ESYOyUdUK0uY4+ZvJ39ge/iQgsN+IcdcCA8ZQIP5pM3FSd9OH/cprOzPj9DmPBYROuNlkC5DZ3u/WAz3439SBTMYMKtcAtgt9EjSFUk5IgFWpT/Q7tlvZq+eoa3JSVbCAzYsOKXartcYVDn6s65IZCcYcfjKYi4A11/X/EJnsciyMv4eYGysJzGhpxH9R",
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


                    List<Tblarticoli> tblarticoliList = JsonConvert.DeserializeObject<List<Tblarticoli>>(json);

                    foreach (Tblarticoli tblarticoliObj in tblarticoliList)
                    {
                        if (tblarticoliObj.Type == "table")
                        {
                            foreach (Datum element in tblarticoliObj.Data)
                            {
                                var sku = element.CodArt;
                                var ean = element.CodEan;
                                var descr = element.DescArticolo;
                                string searchPageUrl = $"https://www.cosicomodo.it/spesa-online/ricerca?q={ean}";
                                Console.WriteLine($"Articolo #{sku}: {descr}");


                                driver.Navigate().GoToUrl(searchPageUrl);
                                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".large-10 .listing")));
                                IList<IWebElement> links = firstResult.FindElements(By.TagName("a"));
                                IWebElement link = links.First(e => e.GetAttribute("href") != "#");
                                string productPageUrl = link.GetAttribute("href");
                                Console.WriteLine(productPageUrl);

                                driver.Navigate().GoToUrl(productPageUrl);
                                IWebElement secondResult = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".image-container")));
                                IList<IWebElement> images = secondResult.FindElements(By.CssSelector(".image-container .slick-slide a.mfp-zoom"));

                                for (int i = 0; i < images.Count(); i++)
                                {
                                    string image = images.ElementAt(i).GetAttribute("href");
                                    Console.WriteLine(image);
                                    //SaveImage(image, sku + "_" + i + ".jpg", ImageFormat.Jpeg);
                                }

                            }
                        }
                    }

                }
                catch (InvalidCookieDomainException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (WebDriverTimeoutException e)
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
            Bitmap bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                string conf = ConfigurationManager.AppSettings["AssetsPath"];
                string pathToFile = Path.Combine(Path.GetDirectoryName(conf), "product_images/" + filename);
                bitmap.Save(pathToFile, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

    }
}
