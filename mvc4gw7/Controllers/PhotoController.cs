using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using mvc4gw7.Models;

namespace mvc4gw7.Controllers
{
    public class PhotoController : Controller
    {
        readonly IAppMediaRepository repository;

        public PhotoController(IAppMediaRepository repository)
        {
            this.repository = repository;
        }


        // LIST
        public ActionResult List()
        {
            return View(repository.GetMediaCollectionsDescriptions(UserName: "admin", MediaType: MediaType.Photo).ToList());
        }

        //SHOW ITEM
        public ActionResult ShowItem(int CollectionId)
        {
            MediaCollection MediaCollection = new MediaCollection();
            MediaCollection = repository.GetMediaCollectionDescription(CollectionId);
            ViewBag.collectionTitle = MediaCollection.ShortTitle.ToString();
            ViewBag.collectionFolder = MediaCollection.Location.ToString();

            return View(MediaCollection.MediaElements.ToList());            
        }


    }
}
