using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

using System.IO;
using mvc4gw7.Models;

namespace mvc4gw7.Models
{
    //SQL database
    
    public class AppMediaContext : DbContext
    {
        public AppMediaContext()
//            : base ("name=DefaultConnection")
            : base("name=WebHosting")
        {
        }

        public DbSet<MediaElement> MediaElements { get; set; }
        public DbSet<MediaCollection> MediaCollections { get; set; }

        /*       protected override void OnModelCreating(DbModelBuilder modelBuilder)
               {
                   modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
               } */

    }

    //public class AppMediaInitializer : DropCreateDatabaseAlways<AppMediaContext>
    //{
    //    protected override void Seed(AppMediaContext context)
    //    {
    //        base.Seed(context);
    //    }

    //}


    public class LINQAppMediaRepository : IAppMediaRepository
    {
        private AppMediaContext db = new AppMediaContext();
                
        //MediaClasses\

        public List<MediaCollection> GetMediaCollectionsDescriptions(string UserName, MediaType MediaType)
        {
            return db.MediaCollections.Where(x => x.UserName == UserName && x.MediaType == MediaType).ToList();
        }

        public MediaCollection GetMediaCollectionDescription(int CollectionId)
        {
            return db.MediaCollections.First(x => x.Id == CollectionId);
        }

        public List<MediaElement> GetMediaCollection(int CollectionId)
        {
            return db.MediaElements.Where(x => x.CollectionId == CollectionId).ToList();
        }

        public void AddMediaCollectionDescription(MediaCollection MediaCollection)
        {
            db.MediaCollections.Add(MediaCollection);
            db.SaveChanges();
        }

        public void AddMedia(MediaElement Media)
        {
            db.MediaElements.Add(Media);
            db.SaveChanges();
        }

        public int GetLastCollectionId()
        {
            return db.MediaCollections.OrderByDescending(x=>x.Id).First().Id;
        }

        public void EraseMediaCollection(int CollectionId)
        {
            List<MediaElement> Medias = db.MediaElements.Where(x => x.CollectionId == CollectionId).ToList();
            foreach (var Media in Medias)
            {
                string mediafile = AppDomain.CurrentDomain.BaseDirectory + "/Content/MembersFiles/" + Media.File;
                if (File.Exists(mediafile)) File.Delete(mediafile);
                db.MediaElements.Remove(Media);
            }

            MediaCollection MediaCollection = db.MediaCollections.First(x => x.Id == CollectionId);
            db.MediaCollections.Remove(MediaCollection);
            db.SaveChanges();
        
        }

        public List<MediaCollection> GetAllMediaCollectionsDescriptions()
        {
            return db.MediaCollections.ToList();
        }

        public List<VideoMediaElement> TransformToVideoMediaCollection (int CollectionId)
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