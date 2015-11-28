using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using mvc4gw7.Models;

namespace mvc4gw7.Controllers
{

    public class VideoController : Controller
    {
        readonly IAppMediaRepository repository;

        public VideoController(IAppMediaRepository repository)
        {
            this.repository = repository;
        }

        //UNDER CONSTRUCTION
        public ActionResult List()
        {
            return View(repository.GetMediaCollectionsDescriptions(UserName: "admin", MediaType: MediaType.Video));

        }

        //SHOW ITEM
        public ActionResult ShowItem(int CollectionId)
        {
            MediaCollection MediaCollection = new MediaCollection();
            MediaCollection = repository.GetMediaCollectionDescription(CollectionId);
            ViewBag.collectionTitle = MediaCollection.ShortTitle.ToString();
            
            return View(repository.TransformToVideoMediaCollection(CollectionId));
        }

        public ActionResult ShowVideo(string file, string collectionTitle)
        {
            ViewBag.Title = "Видеоматериалы";
            ViewBag.collectionTitle = collectionTitle;
            ViewBag.File = file;

            return View();
        }

    }
}
