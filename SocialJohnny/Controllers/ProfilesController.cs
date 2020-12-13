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
            if (TempData.ContainsKey("WrongNickName"))
                ViewBag.NicknameMessage = TempData["WrongNickName"].ToString();

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

        public ActionResult Find(string nickname)
        {
            var profiles = from p in db.Profiles
                           where p.Nickname.Contains(nickname) &&
                           p.IsPrivate == false
                           select p;
            ViewBag.profiles = profiles;
            return View();
        }

        [Authorize(Roles="Admin, User")]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            Profile currentProfile = db.Profiles.Find(id);
            if (currentProfile == null)
                return View("FailedProfile");
            if (User.Identity.GetUserId() != currentProfile.UserId)
                return View("FailedProfile");
           
            //check for multiple nicknames 
            var profiles = from p in db.Profiles
                          where p.Nickname == requestProfile.Nickname
                          select p;

            //the Nickname is already taken
            int count = 0;
            foreach(var elem in profiles)
            {
                count++;
            }

            if (count > 0)
            {
                TempData["WrongNickname"] = "The nickname is already used";
                return RedirectToAction("ViewEdit", "Profiles");

            }
            try
            {
                if (TryUpdateModel(currentProfile))
                {
                    currentProfile.IsPrivate = requestProfile.IsPrivate;
                    currentProfile.Nickname = requestProfile.Nickname;
                    currentProfile.Email = currentProfile.Email;
                    currentProfile.FirstName = requestProfile.FirstName;
                    currentProfile.LastName = requestProfile.LastName;
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