using System;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;

namespace S21eimagescollect
{
    class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                string conf = ConfigurationManager.AppSettings["DataPath"];
                string path = Path.Combine(Path.GetDirectoryName(conf), "catalog_product_entity.json");
                string host = Environment.GetEnvironmentVariable("MYSQL_HOST");
                string user = Environment.GetEnvironmentVariable("MYSQL_USER");
                string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
                string cs = $"server={host};userid={user};password={password};database=supergabry_symfony";

                string[] items = getMissingImages();

                string sql = string.Format("SELECT TRIM(p.CodArt) as CodArt, ANY_VALUE(e.CodEAN) AS CodEan, ANY_VALUE(p.DescArticolo) AS DescArticolo " +
                            "FROM tblarticolo p " +
                            "JOIN tblean e USING(CodArt) " +
                            "JOIN tbllistinovend l USING(CodArt) " +
                            "WHERE TRIM(p.CodArt) IN ({0}) " +
                            "AND STR_TO_DATE(l.FlgDataUltimaModifica, '%d/%m/%Y') >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)" +
                            "GROUP BY TRIM(p.CodArt) " +
                            "ORDER BY TRIM(p.CodArt)", string.Join(", ", items.Select(x => "?")));


                using var con = new MySqlConnection(cs);
                con.Open();
                using var cmd = new MySqlCommand(sql, con);

                int index = 0;
                foreach (string item in items)
                {
                    cmd.Parameters.AddWithValue($"param{index}", item);
                    index++;
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();

                adapter.Fill(table);

                string createText = JsonConvert.SerializeObject(table);
                File.WriteAllText(path, "{ \"catalog_product_entity\": " + createText + "}");

                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static string[] getMissingImages()
        {

            string host = Environment.GetEnvironmentVariable("MYSQL_HOST");
            string user = Environment.GetEnvironmentVariable("MYSQL_USER");
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            string cs = $"server={host};userid={user};password={password};database=supergabry_magento";
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();
            string sku;


            // First Check
            using var con1 = new MySqlConnection(cs);
            con1.Open();

            string sql1 = "SELECT a.sku FROM catalog_product_entity AS a " +
                          "LEFT JOIN catalog_product_entity_media_gallery_value AS b ON a.entity_id = b.entity_id " +
                          "LEFT JOIN catalog_product_entity_media_gallery AS c ON b.value_id = c.value_id WHERE c.value IS NULL";

            using var cmd1 = new MySqlCommand(sql1, con1);

            using MySqlDataReader rdr1 = cmd1.ExecuteReader();

            while (rdr1.Read())
            {
                sku = rdr1.GetString(0);
                if (!list1.Contains(sku))
                {
                    list1.Add(sku);
                }
            }
            con1.Close();


            // Second Check
            using var con2 = new MySqlConnection(cs);
            con2.Open();

            string sql2 = "SELECT a.sku FROM catalog_product_entity AS a " +
                          "LEFT JOIN catalog_product_entity_media_gallery_value_to_entity AS b ON a.entity_id = b.entity_id " +
                          "LEFT JOIN catalog_product_entity_media_gallery AS c ON b.value_id = c.value_id WHERE c.value IS NULL";

            using var cmd2 = new MySqlCommand(sql2, con2);

            using MySqlDataReader rdr2 = cmd2.ExecuteReader();

            while (rdr2.Read())
            {
                sku = rdr2.GetString(0);
                if (!list2.Contains(sku))
                {
                    list2.Add(sku);
                }
            }
            con2.Close();

            return list1.Intersect(list2).ToArray();
        }


    }
}
