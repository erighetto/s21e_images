﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using S21eImages.Model;

namespace S21eImages.Manager
{
    public class Export : IExport
    {
        public void Do()
        {

            string targetDirectory = Environment.GetEnvironmentVariable("ASSETS_PATH");

            Import importObj = new Import
            {
                Simple = new List<Simple> { }
            };

            string[] fileEntries = Directory.GetFiles(Path.Combine(targetDirectory, "product_images"));
            foreach (string path in fileEntries.OrderBy(o => o))
            {
                string fileName = Path.GetFileName(path);

                if (IsImage(Path.GetExtension(fileName)))
                {
                    string sku = fileName.Split('_')[0];

                    Console.WriteLine("Sku: {0} - File: {1}", sku, fileName);

                    Image img = new Image
                    {
                        FileOrUrl = "product_images/" + fileName,
                        Global = new Global
                        {
                            Role = new List<string> {
                                "image",
                                "small_image",
                                "thumbnail"
                            }
                        }
                    };

                    Simple itemObj = importObj.Simple.FirstOrDefault(x => x.Sku == sku);
                    if (itemObj == null)
                    {
                        importObj.Simple.Add(new Simple
                        {
                            Sku = sku,
                            Global = new Global
                            {
                                Status = "1"
                            },
                            Images = new Images
                            {
                                Image = new List<Image> {
                                    img
                                }
                            }
                        });
                    }
                    else
                    {
                        itemObj.Images.Image.Add(img);
                    }

                }
            }

            string fileToWrite = Path.Combine(targetDirectory, "export-articoli.xml");

            WriteToFile(fileToWrite, importObj);
        }

        /// <summary>
        /// Controllo se è un'immagine
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsImage(string ext)
        {
            List<string> imagesTypes = new List<string> {
                ".png",
                ".jpg",
                ".jpeg",
                ".bmp",
                ".gif"
            };

            return imagesTypes.Contains(ext.ToLower());
        }


        /// <summary>
        /// Scrivo nel file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="importObj"></param>
        private static void WriteToFile(string filename, Import importObj)
        {
            if (!File.Exists(filename))
            {
                File.CreateText(filename);
            }
            XmlSerializer x = new XmlSerializer(typeof(Import));
            TextWriter writer = new StreamWriter(filename, false);
            x.Serialize(writer, importObj);
        }
    }
}
