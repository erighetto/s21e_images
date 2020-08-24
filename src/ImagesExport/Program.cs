﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ImagesExport.Model;

namespace ImagesExport
{
    class Program
    {
        static void Main(string[] args)
        {
            string targetDirectory = ConfigurationManager.AppSettings["AssetsPath"];

            Import importObj = new Import();

            string[] fileEntries = Directory.GetFiles(Path.Combine(targetDirectory, "product_images"));
            foreach (string path in fileEntries)
            {
                string fileName = Path.GetFileName(path);

                if (IsImage(Path.GetExtension(fileName)))
                {
                    string sku = fileName.Split('_')[0];

                    Console.WriteLine("Sku: {0} - File: {1}", sku, fileName);

                    List<string> RoleList = new List<string> {
                          "image",
                          "small_image",
                          "thumbnail"
                    };

                    Image img = new Image
                    {
                        FileOrUrl = "product_images/" + fileName,
                        Global = new Global()
                    };
                    img.Global.Role = RoleList;

                    Simple newObj;
                    if (importObj.Simple is object)
                    {
                        newObj = importObj.Simple.FirstOrDefault(x => x.Sku == sku);
                        if (newObj == null)
                        {
                            newObj = new Simple
                            {
                                Sku = sku
                            };
                        }
                    }
                    else
                    {
                        newObj = new Simple
                        {
                            Sku = sku
                        };
                    }

                    if (newObj.Images is object)
                    {
                        newObj.Images.Image.Add(img);
                    }
                    else
                    {
                        Images imgs = new Images();
                        newObj.Images = imgs;
                        newObj.Images.Image = new List<Image> {
                           img
                        };
                    }
                    if (importObj.Simple is object)
                    {
                        importObj.Simple.Add(newObj);
                    }
                    else
                    {
                        importObj.Simple = new List<Simple>
                        {
                            newObj
                        };
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
            XmlSerializer x = new XmlSerializer(typeof(Import));
            TextWriter writer = new StreamWriter(filename, false);
            x.Serialize(writer, importObj);
        }
    }
}