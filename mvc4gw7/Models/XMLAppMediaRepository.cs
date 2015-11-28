using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;
using System.IO;
using System.Web.Security;

namespace mvc4gw7.Models
{
    public class XMLAppMediaRepository:IAppMediaRepository
    {
        private string XMLDescriptionFile = AppDomain.CurrentDomain.BaseDirectory + @"/Content/XMLRepository.xml";

        public List<MediaCollection> GetMediaCollectionsDescriptions(string UserName, MediaType MediaType)
        {
            int Id=0;

            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);
            XmlElement root = XMLStore.DocumentElement;

            List<MediaCollection> mediaCollections = new List<MediaCollection>();
            foreach (XmlNode item in root.ChildNodes)
            {            
                if (item.Attributes["UserName"].Value.ToString() == UserName &&
                    item.Attributes["MediaType"].Value.ToString() == ((int)MediaType).ToString())
                {

                    MediaCollection mediaCollection = new MediaCollection();
                    mediaCollection.Id              = Id;
                    mediaCollection.UserName        = item.Attributes["UserName"].Value.ToString();
                    mediaCollection.MediaType       = MediaType;
                    mediaCollection.ShortTitle      = item.Attributes["ShortTitle"].Value.ToString();
                    mediaCollection.Title           = item.Attributes["Title"].Value.ToString();
                    mediaCollection.Location        = item.Attributes["Location"].Value.ToString();
                    mediaCollection.Description     = item.Attributes["Description"].Value.ToString();

                    mediaCollections.Add(mediaCollection);
                }
                ++Id;
            }

            return mediaCollections;

        }

        public MediaCollection GetMediaCollectionDescription(int CollectionId)
        {

            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);
            XmlNodeList root = XMLStore.DocumentElement.ChildNodes;
            XmlNode collectionNode = root[CollectionId];

            MediaCollection mediaCollection     = new MediaCollection();
                mediaCollection.Id              = CollectionId;
                mediaCollection.UserName        = collectionNode.Attributes["UserName"].Value.ToString();

            switch (collectionNode.Attributes["MediaType"].Value.ToString())
                {
                    case "Photo":
                        mediaCollection.MediaType = MediaType.Photo;
                        break;
                    case "Graphics":
                        mediaCollection.MediaType = MediaType.Graphic;
                        break;
                    case "Video":
                        mediaCollection.MediaType = MediaType.Video;
                        break;

                }

                mediaCollection.ShortTitle      = collectionNode.Attributes["ShortTitle"].Value.ToString();
                mediaCollection.Title           = collectionNode.Attributes["Title"].Value.ToString(); 
                mediaCollection.Location        = collectionNode.Attributes["Location"].Value.ToString();

                int Id = 1;
                List<MediaElement> mediaElements = new List<MediaElement>();
                foreach (XmlNode elementNode in collectionNode)
                {
                    MediaElement mediaElement = new MediaElement();
                    mediaElement.CollectionId = CollectionId;
                    mediaElement.Id = Id++;
                    mediaElement.File = elementNode.Attributes["fileName"].Value.ToString();

                    mediaElements.Add(mediaElement);
                }

                mediaCollection.MediaElements   = mediaElements;

            return mediaCollection;

        }

        public List<MediaElement> GetMediaCollection(int CollectionId)
        {
            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);
            XmlNodeList root = XMLStore.DocumentElement.ChildNodes;

            int Id=1;
            List<MediaElement> mediaElements = new List<MediaElement>();

            foreach (XmlNode node in root[CollectionId])
            {
                MediaElement mediaElement =  new MediaElement();
                mediaElement.CollectionId = CollectionId;
                mediaElement.Id           = Id++;
                mediaElement.File         = node.Attributes["fileName"].Value.ToString();

                mediaElements.Add(mediaElement);
            }

            return mediaElements;

        }

        public List<VideoMediaElement> TransformToVideoMediaCollection(int CollectionId)
        {
            List<MediaElement> mediaCollection = GetMediaCollection(CollectionId);
            string mediaFolder = GetMediaCollectionDescription(CollectionId).Location;

            List<VideoMediaElement> videoMediaCollection = new List<VideoMediaElement>();
            foreach (MediaElement mediaElement in mediaCollection)
            {
                VideoMediaElement videoMediaElement = new VideoMediaElement();
                videoMediaElement.VideoFile         = "/Content/" + mediaFolder + mediaElement.File;
                videoMediaElement.ImageFile         = "/Content/" + mediaFolder + Path.GetFileNameWithoutExtension(mediaElement.File) + ".jpg";
                videoMediaCollection.Add(videoMediaElement);
            }

            return videoMediaCollection;     
        }

        public void AddMediaCollectionDescription(MediaCollection MediaCollection)
        {
            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);

            XmlElement node_mediaCollection = XMLStore.CreateElement("item" + (GetLastCollectionId() + 2));
            
            XmlAttribute attr_userName = XMLStore.CreateAttribute("UserName");
            attr_userName.Value = MediaCollection.UserName;
            node_mediaCollection.Attributes.Append(attr_userName);

            XmlAttribute attr_mediaType = XMLStore.CreateAttribute("MediaType");
            attr_mediaType.Value = MediaCollection.MediaType.ToString();
            node_mediaCollection.Attributes.Append(attr_mediaType);

            XmlAttribute attr_Location = XMLStore.CreateAttribute("Location");
            attr_Location.Value = MediaCollection.Location;
            node_mediaCollection.Attributes.Append(attr_Location);

            XmlAttribute attr_ShortTitle = XMLStore.CreateAttribute("ShortTitle");
            attr_ShortTitle.Value = MediaCollection.ShortTitle;
            node_mediaCollection.Attributes.Append(attr_ShortTitle);

            XmlAttribute attr_Title = XMLStore.CreateAttribute("Title");
            attr_Title.Value = MediaCollection.ShortTitle;
            node_mediaCollection.Attributes.Append(attr_Title);

            XmlAttribute attr_Description = XMLStore.CreateAttribute("Description");
            attr_Description.Value = MediaCollection.Description;
            node_mediaCollection.Attributes.Append(attr_Description);


            XMLStore.DocumentElement.InsertAfter(node_mediaCollection, XMLStore.DocumentElement.LastChild);

            XMLStore.Save(XMLDescriptionFile);

        }

        public void AddMedia(MediaElement Media)
        {
            XmlDocument XMLfile = new XmlDocument();
            XMLfile.Load(XMLDescriptionFile);

            XmlElement node_mediaElement = XMLfile.CreateElement("item");
            XmlAttribute attr_fileName = XMLfile.CreateAttribute("fileName");
            attr_fileName.Value = Media.File;
            node_mediaElement.Attributes.Append(attr_fileName);

            XMLfile.DocumentElement.LastChild.InsertAfter(node_mediaElement, XMLfile.DocumentElement.LastChild.LastChild);

            XMLfile.Save(XMLDescriptionFile);

        }

        public int GetLastCollectionId()
        {
            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);
            XmlNodeList root = XMLStore.DocumentElement.ChildNodes;
            return root.Count-1;
        }

        public void EraseMediaCollection(int CollectionId)
        {
            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);
            XmlNode node = XMLStore.DocumentElement.ChildNodes[CollectionId];
            XMLStore.DocumentElement.RemoveChild(node);
            XMLStore.Save(XMLDescriptionFile);
        }

        public List<MediaCollection> GetAllMediaCollectionsDescriptions()
        {
            int Id = 0;

            XmlDocument XMLStore = new XmlDocument();
            XMLStore.Load(XMLDescriptionFile);

            List<MediaCollection> mediaCollections = new List<MediaCollection>();

            foreach (XmlNode item in XMLStore.DocumentElement.ChildNodes)
            {

                MediaCollection mediaCollection = new MediaCollection();
                mediaCollection.Id              = Id;
                mediaCollection.UserName        = item.Attributes["UserName"].Value.ToString();

                switch (item.Attributes["mediaType"].Value.ToString())
                    {
                        case "Photo":
                            mediaCollection.MediaType = MediaType.Photo;
                            break;
                        case "Graphics":
                            mediaCollection.MediaType = MediaType.Graphic;
                            break;
                        case "Video":
                            mediaCollection.MediaType = MediaType.Video;
                            break;
                    }

                mediaCollection.ShortTitle      = item.Attributes["ShortTitle"].Value.ToString();
                mediaCollection.Title           = item.Attributes["Title"].Value.ToString();
                mediaCollection.Location        = item.Attributes["Location"].Value.ToString();

                mediaCollections.Add(mediaCollection);

                ++Id;
            }

            return mediaCollections;
        }
    }
}