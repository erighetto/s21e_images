using System;
using Microsoft.Extensions.DependencyInjection;

namespace S21eImages
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test if input arguments were supplied.
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter an option.");
            } else
            {
                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<ICollect, Collect>()
                    .AddSingleton<IScrape, Scrape>()
                    .AddSingleton<IExport, Export>()
                    .BuildServiceProvider();

                if (args[0] == "collect")
                {
                    var collect = serviceProvider.GetService<ICollect>();
                    collect.Do();
                } else if (args[0] == "scrape") {
                    var scrape = serviceProvider.GetService<IScrape>();
                    scrape.Do();
                } else if (args[0] == "export")
                {
                    var export = serviceProvider.GetService<IExport>();
                    export.Do();
                }  else
                {
                    Console.WriteLine("You say goodbye and I say hello.");
                }
            }
        }
    }
}
