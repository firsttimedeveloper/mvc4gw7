using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc4gw7.Models
{
    public interface IAppMediaRepository
    {

        List<MediaCollection> GetMediaCollectionsDescriptions(string UserName, MediaType MediaType);
        MediaCollection GetMediaCollectionDescription(int CollectionId);
        List<MediaElement> GetMediaCollection(int CollectionId);
        List<VideoMediaElement> TransformToVideoMediaCollection(int CollectionId);

        void AddMediaCollectionDescription(MediaCollection MediaCollection);
        void AddMedia(MediaElement Media);
        int GetLastCollectionId();
        void EraseMediaCollection(int CollectionId);

        List<MediaCollection> GetAllMediaCollectionsDescriptions();

    }
}