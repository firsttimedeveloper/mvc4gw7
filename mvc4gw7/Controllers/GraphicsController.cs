using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc4gw7.Models;

using System.Xml;

namespace mvc4gw7.Controllers
{
    public class GraphicsController : Controller
    {
        readonly IAppMediaRepository repository;

        public GraphicsController(IAppMediaRepository repository)
        {
            this.repository = repository;
        }


        //UNDER CONSTRUCTION
        public ActionResult List()
        {
            return View(repository.GetMediaCollectionsDescriptions(UserName: "admin", MediaType: MediaType.Graphic).ToList());
        }

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
