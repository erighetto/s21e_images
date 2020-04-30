using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using S21eimagesexport.Model;

namespace S21eimagesexport
{
    class Program
    {
        public static void Main(string[] args)
        {
            string targetDirectory = ConfigurationManager.AppSettings["AssetsPath"];

            Import importObj = new Import();

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string path in fileEntries)
            {
                string fileName = Path.GetFileName(path);
                if (IsImage(Path.GetExtension(fileName)))
                {
                    string sku = fileName.Split('_')[0];

                    List<string> RoleList = new List<string> {
                          "base_image",
                          "small_image",
                          "thumbnail_image"
                    };

                    Image img = new Image
                    {
                        FileOrUrl = fileName,
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

                    importObj.Simple.Add(newObj);


                }
            }
        }

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

    }
}