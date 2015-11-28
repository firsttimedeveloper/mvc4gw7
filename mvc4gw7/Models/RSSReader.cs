using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;
using System.Text;
using System.ServiceModel.Syndication;

namespace mvc4gw7
{
    public class Feed
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class TapeItem
    {
        public string Message { get; set; }
        public string Link { get; set; }
    }


    public class RSSReader
    {
        public List<Feed> sources = new List<Feed>() {
                //new Feed { Title= "NY Times" , Url= "http://rss.nytimes.com/services/xml/rss/nyt/HomePage.xml"},
                new Feed { Title= "Mail.ru", Url= "https://news.mail.ru/rss/incident/91/" },
                new Feed { Title= "Yandex", Url= "http://news.yandex.ru/index.rss" },
                new Feed { Title= "Rambler" , Url= "http://news.rambler.ru/rss/head/"},
                new Feed { Title= "ИТАР-ТАСС", Url= "http://tass.ru/rss/v2.xml" },                
                new Feed { Title= "РБК", Url= "http://static.feed.rbc.ru/rbc/internal/rss.rbc.ru/rbc.ru/mainnews.rss" },
                new Feed { Title= "CNN", Url= "http://rss.cnn.com/rss/edition.rss" },
                new Feed { Title= "Президент РФ. Новости", Url= "http://www.kremlin.ru/events/president/news/feed" },
                new Feed { Title= "Президент РФ. Документы", Url= "http://www.kremlin.ru/acts/news/feed" },
                new Feed { Title= "РПЦ", Url= "http://www.patriarchia.ru/rss/rss_news.rss" },
                new Feed { Title= "РПЦ ОВЦС", Url= "https://mospat.ru/ru/feed/" },
                new Feed { Title= "NASA. Breaking News", Url= "http://www.nasa.gov/rss/dyn/breaking_news.rss" },
                //new Feed { Title= "NASA. Education News", Url= "http://www.nasa.gov/rss/dyn/educationnews.rss" },

            };
       

        private List<SyndicationItem> GetItemsFromUrl(string url)
        {
            List<SyndicationItem> ListA = new List<SyndicationItem>();
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            foreach (SyndicationItem item in feed.Items)
            {
                item.Copyright = (feed.Copyright == null) ? new TextSyndicationContent(feed.Generator) : new TextSyndicationContent(feed.Copyright.Text);
                item.Id = (item.Id == null) ? item.Links[0].Uri.ToString() : item.Id;
                ListA.Add(item);
            }

            return ListA;
        }

        public List<SyndicationItem> GetLastNews(string url)
        {
            List<SyndicationItem> lastNews = new List<SyndicationItem>();
            lastNews.AddRange(this.GetItemsFromUrl(url));
            lastNews.Sort(delegate(SyndicationItem x, SyndicationItem y) { return y.PublishDate.CompareTo(x.PublishDate); });

            return lastNews;
        }
    }

}