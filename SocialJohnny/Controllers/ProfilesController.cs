using Microsoft.AspNet.Identity;
using SocialJohnny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialJohnny.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Profiles
        public ActionResult Index()
        {
            return View("FailedProfile");
        }


        [Authorize(Roles = "Admin, User")]
        public ActionResult ViewEdit()
        {
            string _userId = User.Identity.GetUserId();
            var _profile = from p in db.Profiles
                           where p.UserId == _userId
                           select p;

            Profile profile = new Profile();
            foreach(var elem in _profile)
            {
                profile = elem;
                break;
            }
            return View("ViewEditProfile", profile);

        }

        [Authorize(Roles="Admin, User")]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            Profile currentProfile = db.Profiles.Find(id);
            if (currentProfile == null)
                return View("FailedProfile");
            if (User.Identity.GetUserId() != currentProfile.UserId)
                return View("FailedProfile");
            
            try
            {
                if (TryUpdateModel(currentProfile))
                {
                    currentProfile.IsPrivate = requestProfile.IsPrivate;
                    currentProfile.Email = requestProfile.Email;
                    currentProfile.City = requestProfile.City;
                    currentProfile.Job = requestProfile.Job;
                    currentProfile.Description = requestProfile.Description;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                    return View("FailedProfile");
            }
            catch (Exception e)
            {
                return View("FailedProfile");
            }
            
        }

        
    }
}