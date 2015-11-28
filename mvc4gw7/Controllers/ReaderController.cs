using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml;
using System.ServiceModel.Syndication;
using System.Globalization;

namespace mvc4gw7.Controllers
{
   
    public class ReaderController : Controller
    {
        
        public ActionResult Index()
        {
            List<TapeItem> Tape = new List<TapeItem>();
            RSSReader reader = new RSSReader();

            Tape.Add(new TapeItem { Message = "Погода", Link = " " });

            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load("http://export.yandex.ru/weather-ng/forecasts/27730.xml");
            XmlElement xmlroot = xmldocument.DocumentElement;
            XmlNodeList daynodes = xmlroot.GetElementsByTagName("day");

            string today = DateTime.Today.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"));
            string tommorow = DateTime.Today.AddDays(1).ToString("D", CultureInfo.CreateSpecificCulture("ru-RU"));

            foreach (XmlElement daynode in daynodes)
            {

                XmlNodeList day_partnodes = daynode.GetElementsByTagName("day_part");
                string date = DateTime.ParseExact(daynode.Attributes["date"].Value, "yyyy-MM-dd", null).ToString("D", CultureInfo.CreateSpecificCulture("ru-RU")); ;


                if (date == today || date == tommorow)
                {
                    Tape.Add(new TapeItem { Message = date, Link = " " }); 
                    foreach (XmlNode day_partnode in day_partnodes)
                    {

                        string message = string.Empty;
                        switch (day_partnode.Attributes["type"].Value)
                        {

                            case "morning":
                                message = "Утро";
                                break;

                            case "day":
                                message = "День";
                                break;

                            case "evening":
                                message = "Вечер";
                                break;

                            case "night":
                                message = "Ночь";
                                break;
                        }//switch
                        if (message != string.Empty)
                        {
                            message += ": +";
                            if (day_partnode["temperature"] != null)
                            {
                                message += day_partnode["temperature"].InnerText + "°C";
                            }
                            else
                            {
                                message += day_partnode["temperature_from"].InnerText + "°C ... +" + day_partnode["temperature_to"].InnerText + "°C";
                            }
                            message += " (" + day_partnode["weather_type"].InnerText + ")";
                            Tape.Add(new TapeItem { Message = message, Link = " " }); 
                        }

                    }//foreach day_partnode
                }//if date
            }//foreach daynode

            Tape.Add(new TapeItem { Message = " ", Link = " " });

            foreach (Feed feed in reader.sources)
            {
                Tape.Add(new TapeItem{Message=feed.Title, Link=" "});

                var lastnews = reader.GetLastNews(feed.Url);
                foreach (SyndicationItem item in lastnews)
                {
                    TapeItem TapeItem = new TapeItem();
                    TapeItem.Message= item.Title.Text;                    
                    TapeItem.Link = item.Links.FirstOrDefault().Uri.ToString();
                    Tape.Add(TapeItem);
                }

                Tape.Add(new TapeItem{Message=" ", Link=" "});
            }

            return View(Tape);
        }

    }
}

