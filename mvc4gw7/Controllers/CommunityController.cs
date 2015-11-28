using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using System.IO;
using mvc4gw7.Models;

namespace mvc4gw7site.Controllers
{
    [Authorize(Roles="Administrator, Member")]
    public class CommunityController : Controller
    {
        readonly IAppMediaRepository repository;
 

        //MembershipProvider MembershipService = new SQLMembershipProvider();

        public CommunityController(IAppMediaRepository repository)
        {
            this.repository = repository;
            
        }


        //COLLECTION LIST
        [Authorize]
        public ActionResult CollectionsList()
        {
            ViewBag.User = this.User.Identity.Name.ToString();
            return View(repository.GetMediaCollectionsDescriptions(UserName: User.Identity.Name, MediaType: MediaType.Photo).ToList());
        }

        //SHOW COLLECTION
        [Authorize]        
        public ActionResult ShowCollection(int CollectionId)
        {
            MediaCollection MediaCollection = new MediaCollection();
            MediaCollection = repository.GetMediaCollectionDescription(CollectionId);
            ViewBag.User = this.User.Identity.Name.ToString();
            ViewBag.CollectionTitle = MediaCollection.Title.ToString();
            ViewBag.Description = MediaCollection.Description.ToString();
            ViewBag.CollectionFolder = MediaCollection.Location.ToString();
            return View(MediaCollection);

        }



        //ADD COLLECTION
        [Authorize]
        [HttpGet]
        public ActionResult AddCollection_Attributes()
        {
            ViewBag.User = this.User.Identity.Name.ToString();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddCollection_Attributes(MediaCollection MediaCollection)
        {
            ViewBag.User = this.User.Identity.Name.ToString();
            
            if (ModelState.IsValid)
            {
/*                try
                {
                    mgallery.Save();
                }
                catch (RuleException ex)
                {
                    ex.CopyToModelState(ModelState);
                }
*/

                MediaCollection.UserName = User.Identity.Name;
                repository.AddMediaCollectionDescription(MediaCollection);
            }

            return ModelState.IsValid ? View("AddCollection_File") : View();          

        }

        [Authorize]
        [HttpGet]
        public ActionResult AddCollection_File()
        {
            ViewBag.User = this.User.Identity.Name.ToString();
            return View();
        }


        [Authorize]        
        [HttpPost]
        public ActionResult AddCollection_File(HttpPostedFileBase fileUpload)
        {
            ViewBag.User = this.User.Identity.Name.ToString();            

            if (fileUpload!=null)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Content/MembersFiles/";
                string filename = Path.GetFileName(fileUpload.FileName);
                fileUpload.SaveAs(Path.Combine(path, filename));

                int lastCollectionId = repository.GetLastCollectionId();
                MediaElement Media = new MediaElement();
                Media.File = filename;
                Media.CollectionId = lastCollectionId;
                repository.AddMedia(Media);
            }

            return View();
        }

        [Authorize]
        public ActionResult AddCollection_Finish()
        {
            int addedCollectionId = repository.GetLastCollectionId();
            return RedirectToAction("ShowCollection", new { CollectionId = addedCollectionId });
        }

    }
}
