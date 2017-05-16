using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using TruliaParser.DataProviders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using AngleSharp;
using AngleSharp.Scripting.JavaScript.Services;
using Jint.Native;
using AngleSharp.Extensions;
using Jint.Native.Object;

namespace TruliaParser.Components
{
    class Parser
    {
        private static WebProxy currentProxy;
        private static HtmlParser parser;

        public Parser()
        {
            //currentProxy = ProxySolver.Instance.getNewProxy();
            var config = new Configuration()
                .WithJavaScript();
            parser = new HtmlParser(config); //создание экземпляра парсера, он можнт быть использован несколько раз для одного потока(экземпляра класса Parser)
            
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
        public void StartParsing(Region region)
        {
            string regionLink = region.Link;
            //Console.WriteLine("Beginning the parsing new region:\nState: {0}, County: {1}, Link: {2}", region.State, region.RegionName, region.Link);
            //List<string> offerLinks = new List<string>(GetOffersLinks(regionLink));
           // offerLinks.ForEach(i => Console.WriteLine(i));
            ParseOffer("https://www.trulia.com/rental/4010620838-1915-Dundee-Dr-Prattville-AL-36066");
        }

        private void ParseOffer(string offerLink)
        {
            string offerHtml = Constants.WebAttrsNames.NotFound;
            IDocument offerDom;
            int numOfRetrying = Convert.ToInt32(Resources.NumberOfLoadRetrying);
            for(int i = 0;i < numOfRetrying; i++)
            {
                offerHtml = WebHelpers.GetHtml(offerLink);
                if(offerHtml == Constants.WebAttrsNames.NotFound)
                {
                    //UpdateInternalProxy();
                }
                else
                {
                    break;
                }
            }
            
            if(offerHtml != Constants.WebAttrsNames.NotFound)
            {
                offerDom = parser.Parse(offerHtml.Replace("trulia.propertyData.set","var ourdata = ")); //костыль, призванный решить проблему с не работающими методами сайта в голом HTML(без внешних JS)
                ObjectInstance basicData = offerDom.ExecuteScript("ourdata") as  ObjectInstance; //получаем JS-переменную, и теперь по ключам вытаскиваем данные
                Offer o = new Offer(basicData);
                o.directLink = offerLink;
                o.FillFromHtmlDocument(offerDom);
            }
        }

        private List<string> GetOffersLinks(string regionLink)
        {
            int switchProxyRemindCounter = 0; //счетчик количества использования одного прокси
            string nextPageLink = regionLink;
            IElement nextPageLinkDom = null;            
            List<string> offerLinks = new List<string>();
            while (true)
            {
                switchProxyRemindCounter++;
                string searchResultPageHtml = WebHelpers.GetHtml(nextPageLink); //скачали страницу с выдачей
                var searchResultPageDom = parser.Parse(searchResultPageHtml); //перегнали в DOM
                var offerLinksDom = searchResultPageDom.QuerySelectorAll(Constants.OfferListSelectors.OfferLinks); //получили все ссылки на предложения
                if (offerLinksDom != null) //если ссылки на странице есть, собираем в список
                {
                    offerLinks.AddRange(offerLinksDom.ToList().Select(i => Resources.BaseLink+i.GetAttribute(Constants.WebAttrsNames.href)));
                    Console.WriteLine("Добавлено предложений в список/всего: {0}/{1}",offerLinksDom.Length, offerLinks.Count);
                }
                else
                {
                    Console.WriteLine("На этой странице нет ссылок: {0}", nextPageLink);
                    break;
                }
                
                nextPageLinkDom = searchResultPageDom.QuerySelector(Constants.OfferListSelectors.NextPage); //обращаемся к элементу, где сидит ссылка на следующую страницу выдачи
                if(nextPageLinkDom.GetAttribute(Constants.WebAttrsNames.href) != null)
                {
                    nextPageLink = nextPageLinkDom.GetAttribute(Constants.WebAttrsNames.href).Replace("//","https://");
                    
                    Console.WriteLine("Next page link: {0}", nextPageLink);
                }       
                else
                {
                    Console.WriteLine("Ссылки на следующую страницу нет. Закансиваем собирать ссылки этого региона, {0}.",regionLink);
                    break;
                }
                
                if (switchProxyRemindCounter > 10)
                {
                    Console.WriteLine("Refreshing proxy...");
                    //UpdateInternalProxy();
                    switchProxyRemindCounter = 0;
                }
            }
            return offerLinks;

        }


    }
}


 
 
        