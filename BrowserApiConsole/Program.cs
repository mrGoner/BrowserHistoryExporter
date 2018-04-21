using System;
using BrowserHistoryExportApi;

namespace BrowserApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var api = new BrowserExportApi();

                Console.WriteLine("Available browsers:");

                foreach (var browser in api.GetSupportBrowsers())
                {
                    Console.WriteLine(browser);
                }

                Console.WriteLine("Available extentions:");

                foreach (var extention in api.GetSupportExportExtentions())
                {
                    Console.WriteLine(extention);
                }

                Console.WriteLine("Enter pathToDb");
                var path = Console.ReadLine();

                Console.WriteLine("Enter browser name");
                var choosedBrowser = Console.ReadLine();

                Console.WriteLine("Enter date from");
                var from = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Enter date until");
                var until = DateTime.Parse(Console.ReadLine());

                var historyCollection = api.Export(path, choosedBrowser, from, until);

                foreach (var history in historyCollection)
                {
                    Console.WriteLine($"Date: {history.Date} Url: {history.Url} Title {history.Title}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }
}
