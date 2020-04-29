using System;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using S21eimagescollect.Model;
using S21eimagescollect.Service;
using System.Collections.Generic;
using System.Collections.Specialized;

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

                        string response = googleImagesClient.FindImages(options);

                        Search imagesObj = JsonConvert.DeserializeObject<Search>(response);

                        if (imagesObj.Items is object)
                        {
                            foreach (Item item in imagesObj.Items)
                            {
                                Console.WriteLine(item.Link.ToString());
                            }
                        }


                    }
                }

            }
            
        }
    }
}
