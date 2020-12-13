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

        public ActionResult AcceptRequest(int id)
        {
            string currId = User.Identity.GetUserId();

            Profile currentProfile = new Profile();
            Profile friendProfile = new Profile();

            var varCurrentProfile = from p in db.Profiles
                                    where p.UserId == currId
                                    select p;
            foreach(var elem in varCurrentProfile)
            {
                currentProfile = elem;
                break;
            }

            friendProfile = db.Profiles.Find(id);

            try
            {
                if (TryUpdateModel(friendProfile))
                {
                    friendProfile.FriendRequests.Remove(currentProfile);
                    friendProfile.Friends.Add(currentProfile);
                    db.SaveChanges();
                }
                else
                    return View("FailedProfiles");

                if (TryUpdateModel(currentProfile))
                {
                    currentProfile.Friends.Add(friendProfile);
                    db.SaveChanges();
                }
                else
                    return View("FailedProfiles");
            }
            catch (Exception e)
            {
                return View("FailedProfiles");
            }
            return RedirectToAction("ShowFriendRequest", "Profiles");
        }

        public ActionResult DeleteRequest(int id)
        {
            string currId = User.Identity.GetUserId();

            Profile currentProfile = new Profile();
            Profile friendProfile = new Profile();

            var varCurrentProfile = from p in db.Profiles
                                    where p.UserId == currId
                                    select p;
            foreach (var elem in varCurrentProfile)
            {
                currentProfile = elem;
                break;
            }

            friendProfile = db.Profiles.Find(id);

            try
            {
                if (TryUpdateModel(friendProfile))
                {
                    friendProfile.FriendRequests.Remove(currentProfile);
                    db.SaveChanges();
                }
                else
                    return View("FailedProfiles");
            }
            catch (Exception e)
            {
                return View("FailedProfiles");
            }
            return RedirectToAction("ShowFriendRequest", "Profiles");
        }

        public ActionResult ShowFriendRequest()
        {
            string currId = User.Identity.GetUserId();
            Profile currentProfile = new Profile();
            Profile auxProfile = new Profile();
            List<Profile> profileList = new List<Profile>();
            List<Profile> auxProfileList = new List<Profile>();

            var varProfile = from p in db.Profiles
                             where p.UserId == currId
                             select p;

            foreach(var elem in varProfile)
            {
                currentProfile = elem;
                break;
            }

            var profiles = from p in db.Profiles
                           select p;
    
            foreach (var elem in profiles)
            {
                auxProfile = elem;
                auxProfileList.Add(auxProfile);
            }

            foreach(Profile elem in auxProfileList)
            {
                if (elem.FriendRequests.Contains(currentProfile))
                    profileList.Add(elem);
            }
            

            ViewBag.profiles = profileList;

            return View("ShowFriendRequest");
        }


        public ActionResult FriendRequest(int id, string nickname)
        {
            Profile profileRequested = db.Profiles.Find(id);
            string currId = User.Identity.GetUserId();
            var profiles = from p in db.Profiles
                           where p.UserId == currId
                           select p;

            Profile currentProfile = new Profile();
            foreach(var elem in profiles)
            {
                currentProfile = elem;
                break;
            }

            try
            {
                if(TryUpdateModel(currentProfile))
                {
                    currentProfile.FriendRequests.Add(profileRequested);
                    db.SaveChanges();
                    ViewBag.FaieldRequest = nickname;
                    
                    return RedirectToAction("ShowFriendRequest", "Profiles");
                }
                else
                {
                    ViewBag.FaieldRequest = "1";
                    return View("FailedProfile");
                }
            }
            catch(Exception e)
            {
                ViewBag.FaieldRequest = "2";
                return View("FailedProfile");
            }

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
            string currId = User.Identity.GetUserId();
            var profiles = from p in db.Profiles
                           where p.Nickname.Contains(nickname) &&
                           p.IsPrivate == false &&
                           p.UserId != currId
                           select p;

            ViewBag.profiles = profiles;
            ViewBag.nickname = nickname;
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

            //it means i won't change my nickname 
            if (currentProfile.Nickname == requestProfile.Nickname)
                count = 0;

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