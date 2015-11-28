using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

namespace mvc4gw7.Models
{
    public class GrowndworkAppMediaRepository : IAppMediaRepository
    {

        //media
        private List<MediaElement> medias = new List<MediaElement>() {
            new MediaElement {Id=1, CollectionId=1, File="2_1.jpg"},
            new MediaElement {Id=2, CollectionId=1, File="2_2.jpg"},
            new MediaElement {Id=3, CollectionId=1, File="2_3.jpg"},
            new MediaElement {Id=4, CollectionId=1, File="2_4.jpg"},
            new MediaElement {Id=5, CollectionId=1, File="2_5.jpg"},
            new MediaElement {Id=6, CollectionId=1, File="2_6.jpg"},
            new MediaElement {Id=7, CollectionId=1, File="2_7.jpg"},
            new MediaElement {Id=8, CollectionId=1, File="2_8.jpg"},
            new MediaElement {Id=9, CollectionId=1, File="2_9.jpg"},
            new MediaElement {Id=10, CollectionId=1, File="2_10.jpg"},
            new MediaElement {Id=11, CollectionId=2, File="3_1.jpg"},
            new MediaElement {Id=12, CollectionId=2, File="3_2.jpg"},
            new MediaElement {Id=13, CollectionId=2, File="3_3.jpg"},
            new MediaElement {Id=14, CollectionId=3, File="1_1.jpg"},
            new MediaElement {Id=15, CollectionId=3, File="1_2.jpg"},
            new MediaElement {Id=16, CollectionId=4, File="2_1.jpg"},
            new MediaElement {Id=17, CollectionId=4, File="2_2.jpg"},
            new MediaElement {Id=18, CollectionId=4, File="2_3.jpg"},
            new MediaElement {Id=19, CollectionId=4, File="2_4.jpg"},
            new MediaElement {Id=20, CollectionId=5, File="1_1.png"},
            new MediaElement {Id=21, CollectionId=5, File="1_2.png"},
            new MediaElement {Id=22, CollectionId=5, File="1_3.png"},
            new MediaElement {Id=20, CollectionId=6, File="1_3.png"},
            new MediaElement {Id=21, CollectionId=6, File="1_3.png"},
            new MediaElement {Id=22, CollectionId=6, File="1_3.png"},
            new MediaElement {Id=23, CollectionId=7, File="1_1.flv"},
            new MediaElement {Id=23, CollectionId=7, File="1_2.flv"},
            new MediaElement {Id=24, CollectionId=8, File="2_1.flv"}

        };

        private List<MediaCollection> mediacollections = new List<MediaCollection>()
        {
            new MediaCollection {Id=1, UserName="admin", ShortTitle="Новости. 16.06.2008. Зарайск", Title="Новости. 16.06.2008. Зарайск", Description="some description", Location="mobile/photo/Item2/", MediaType=MediaType.Photo},
            new MediaCollection {Id=2, UserName="admin", ShortTitle="Новости. 29.05.2008. Состязание", Title="Новости. 29.05.2008. Состязание", Description="some description", Location="mobile/photo/Item3/", MediaType=MediaType.Photo},
            new MediaCollection {Id=3, UserName="admin", ShortTitle="Эскизы анимации. 2012", Title="Эскизы анимации. 2012", Description="some description", Location="mobile/graphics/Item1/", MediaType=MediaType.Graphic},
            new MediaCollection {Id=4, UserName="admin", ShortTitle="Фоны презентации курсовой работы. 2012", Title="Фоны презентации курсовой работы. 2012", Description="some description", Location="mobile/graphics/Item2/", MediaType=MediaType.Graphic},
            new MediaCollection {Id=5, UserName="other1", ShortTitle="Traffic lights", Title="Colors of traffic lights", Description="Admin test collection", Location="MembersFiles/", MediaType=MediaType.Photo},
            new MediaCollection {Id=6, UserName="other2", ShortTitle="Red lights", Title="Only red", Description="Other user test collection", Location="MembersFiles/", MediaType=MediaType.Photo},
            new MediaCollection {Id=7, UserName="admin", ShortTitle="Тестовая коллекция 1", Title="Тестовая коллекция 1", Description="some description", Location="mobile/video/Item1/", MediaType=MediaType.Video},
            new MediaCollection {Id=8, UserName="admin", ShortTitle="Тестовая коллекция 2", Title="Тестовая коллекция 2", Description="some description", Location="mobile/video/Item2/", MediaType=MediaType.Video}
        };

        public List<MediaCollection> GetMediaCollectionsDescriptions(string UserName, MediaType MediaType)
        {
            return mediacollections.Where(x => x.UserName == UserName && x.MediaType == MediaType).ToList();
        }

        public MediaCollection GetMediaCollectionDescription(int CollectionId)
        {
            return mediacollections.Find(x => x.Id == CollectionId);
        }

        public List<MediaElement> GetMediaCollection(int CollectionId)
        {
            return medias.Where(x => x.CollectionId == CollectionId).ToList();
        }

        public void AddMediaCollectionDescription(MediaCollection MediaCollection)
        {
            mediacollections.Add(MediaCollection);
        }

        public void AddMedia(MediaElement Media)
        {
            medias.Add(Media);
        }

        public int GetLastCollectionId()
        {
            return mediacollections.OrderByDescending(x => x.Id).First().Id;
        }

        public void EraseMediaCollection(int CollectionId)
        {
            List<MediaElement> Medias = medias.FindAll(x => x.CollectionId == CollectionId).ToList();
            foreach (var Media in Medias) medias.Remove(Media);
            MediaCollection MediaCollection = mediacollections.Find(x => x.Id == CollectionId);
            mediacollections.Remove(MediaCollection);
        
        }

        public List<MediaCollection> GetAllMediaCollectionsDescriptions()
        {
            return mediacollections;
        }

        public List<VideoMediaElement> TransformToVideoMediaCollection(int CollectionId)
        {
            List<MediaElement> mediaCollection = GetMediaCollection(CollectionId);
            string mediaFolder = GetMediaCollectionDescription(CollectionId).Location;

            List<VideoMediaElement> videoMediaCollection = new List<VideoMediaElement>();
            foreach (MediaElement mediaElement in mediaCollection)
            {
                VideoMediaElement videoMediaElement = new VideoMediaElement();
                videoMediaElement.VideoFile = "/Content/" + mediaFolder + mediaElement.File;                
                videoMediaElement.ImageFile = "/Content/" + mediaFolder + Path.GetFileNameWithoutExtension(mediaElement.File) + ".jpg";
                videoMediaCollection.Add(videoMediaElement);
            }

            return videoMediaCollection;
        }

    }
}