
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using TruliaParser.DataProviders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;

namespace TruliaParser.Components
{
    class Parser
    {
        private static WebProxy currentProxy;
        private static HtmlParser parser;

        public Parser()
        {
            //currentProxy = ProxySolver.Instance.getNewProxy();
            parser = new HtmlParser(); //создание экземпляра парсера, он можнт быть использован несколько раз для одного потока(экземпляра класса Parser)
        }

        private WebProxy UpdateInternalProxy()
        {
            return currentProxy = ProxySolver.Instance.getNewProxy();
        }

        private void ParseRegionsToDb()
        {
            DataProvider.Instance.ExecureSP(DataProvider.Instance.CreateSQLCommandForInsertSP(Resources.ClearRegionsSPName)); //чистим таблицу с ссылками на регионы
            SqlCommand insertLink = DataProvider.Instance.CreateSQLCommandForInsertSP(Resources.InsertRegionSP); //подготовка процедуры для занесения ссылок в БД
            string rentMapLink = "https://www.trulia.com/rent-sitemap/";
            string statePageHtml;
            while (true)
            {
                statePageHtml = WebHelpers.GetHtmlThrowProxy(rentMapLink, UpdateInternalProxy());
                if (statePageHtml != Constants.WebAttrsNames.NotFound)
                {
                    break;
                }
            }
            var document = parser.Parse(statePageHtml);
            List<IElement> stateLinks = document.QuerySelectorAll(".activeLink.h7").ToList(); //добыли список штатов

            stateLinks.ForEach(i =>
            {
                string stateName = i.TextContent.Trim().Replace(" real estate", "");
                Console.WriteLine(Resources.BaseLink + i.GetAttribute(Constants.WebAttrsNames.href));
                var statePage = parser.Parse(WebHelpers.GetHtmlThrowProxy(Resources.BaseLink + i.GetAttribute(Constants.WebAttrsNames.href), UpdateInternalProxy()));
                List<IElement> countryLinks = statePage.QuerySelectorAll(".mts li a").ToList();
                countryLinks.ForEach(link =>
                {
                    string countyName = link.TextContent.Trim().Replace(" County", "");
                    string countyPageHtml;
                    while (true)
                    {
                        countyPageHtml = WebHelpers.GetHtmlThrowProxy(Resources.BaseLink + link.GetAttribute(Constants.WebAttrsNames.href), UpdateInternalProxy());
                        if (countyPageHtml != Constants.WebAttrsNames.NotFound)
                        {
                            break;
                        }
                    }
                    string regionLink = Resources.BaseLink + parser.Parse(countyPageHtml).QuerySelector(".mbl a").GetAttribute(Constants.WebAttrsNames.href);
                    Console.WriteLine("{0}, {1}, {2}", stateName, countyName, regionLink);
                    insertLink.Parameters.Clear();
                    insertLink.Parameters.AddWithValue(Constants.RegionLinkDbParams.State, stateName);
                    insertLink.Parameters.AddWithValue(Constants.RegionLinkDbParams.RegionName, countyName);
                    insertLink.Parameters.AddWithValue(Constants.RegionLinkDbParams.Link, regionLink);
                    DataProvider.Instance.ExecureSP(insertLink);
                });
            });
        }

        /// <summary>
        /// Парсит регион сайта Trulia  
        /// </summary>
        /// <param name="regionLink">Полный адрес сайта региона для выборки</param>
        public void StartParsing(string regionLink)
        {
            Console.WriteLine();            
        }



        

        

    }
}
