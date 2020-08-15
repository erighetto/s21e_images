using System;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using S21eimagescollect.Model;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace S21eimagescollect
{
    class Program
    {
        public static void Main(string[] args)
        {
            string conf = ConfigurationManager.AppSettings["DataPath"];
            string path = Path.Combine(Path.GetDirectoryName(conf), "tblarticolo.json");
            string json = File.ReadAllText(path);
            GoogleImages googleImagesClient = new GoogleImages();

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

                        Console.WriteLine($"Articolo #{sku}: {descr}");

                        NameValueCollection options = new NameValueCollection
                        {
                            { "q", ean },
                            { "searchType", "image" },
                            { "cx", Environment.GetEnvironmentVariable("GOOGLE_CSE_ID") },
                            { "key", Environment.GetEnvironmentVariable("GOOGLE_API_KEY") },
                            { "start", "0" },
                            { "imgSize", "large" }
                        };


                        try
                        {
                            string response = googleImagesClient.FindImages(options);

                            Search imagesObj = JsonConvert.DeserializeObject<Search>(response);

                            if (imagesObj.Items is object)
                            {
                                int i = 0;
                                foreach (Item item in imagesObj.Items)
                                {
                                    if (i >= 3)
                                        break;

                                    Console.WriteLine(item.Link.ToString());

                                    SaveImage(item.Link.ToString(), sku + "_" + i + ".jpg", ImageFormat.Jpeg);

                                    i++;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0} Exception caught.", e);
                        } finally
                        {
                            Thread.Sleep(3000);
                        }

                    }
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
