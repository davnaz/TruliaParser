using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using TruliaParser.DataProviders;
using AngleSharp.Dom.Html;
using AngleSharp.Dom;
using System.Threading;
using System.Net.NetworkInformation;
using TruliaParser.Components;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TruliaParser
{
    class Program
    {
        static void Main(string[] args)
        {

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = Convert.ToInt32(Resources.MaxDegreeOfParallelism);

            List<Region> regions =  DataProvider.Instance.GetRegionsFromDb();
            //foreach (Region reg in regions)
            //{
            //    Console.WriteLine(reg);
            //}


            //Parallel.ForEach(citiesList, options, (city) =>
            //{
                Parser p = new Parser();                
                p.StartParsing(regions[0]);

            //});

            


            Console.WriteLine("Работа парсера завершена. Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
