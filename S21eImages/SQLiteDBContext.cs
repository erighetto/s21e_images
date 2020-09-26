using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using S21eImages.Model;

namespace S21eImages
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        readonly static string conf = Environment.GetEnvironmentVariable("DATA_PATH");
        readonly static string path = Path.Combine(Path.GetDirectoryName(conf), "products.db");
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={path}");
    }
}