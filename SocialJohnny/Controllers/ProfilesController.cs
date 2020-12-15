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

        [Authorize(Roles ="Admin,User")]
        public ActionResult ShowFriends()
        {
            Profile currentProfile = new Profile();
            string currId = User.Identity.GetUserId();
            List<Profile> friendsList = new List<Profile>();
            List<FriendsProfile> allProfiles = new List<FriendsProfile>();
            //get current profile
            var varProfiles = from p in db.Profiles
                              where p.UserId == currId
                              select p;
            foreach(var elem in varProfiles)
            {
                currentProfile = elem;
                break;
            }

            //get all profiles
            var varProfiles2 = from p in db.FriendsProfiles
                          select p;
            foreach(var elem in varProfiles2)
            {
                allProfiles.Add(elem);
            }

            foreach(FriendsProfile userProfile in allProfiles)
            {
                if (currentProfile.Friends.Contains(userProfile))
                    friendsList.Add(db.Profiles.Find(userProfile.Id));
            }
            ViewBag.friends = friendsList;
            return View();
        }


        [Authorize(Roles ="Admin,User")]
        public ActionResult AcceptRequest(int id)
        {
            string currId = User.Identity.GetUserId();

            //get the involved profiles
            Profile currentProfile = new Profile();
            Profile friendProfile = new Profile();

            //get currentprofile from the database
            var varCurrentProfile = from p in db.Profiles
                                    where p.UserId == currId
                                    select p;
            //save it in currentProfile
            foreach(var elem in varCurrentProfile)
            {
                currentProfile = elem;
                break;
            }
            //find the friend Profile
            friendProfile = db.Profiles.Find(id);
            //find the profiles for the friend request
            FriendsProfile friendsCurrentProfile = new FriendsProfile();
            FriendsProfile friendsNextProfile = new FriendsProfile();

            //get the profile from database and save it in friendsNextProfile.
            var varFriends = from f in db.FriendsProfiles
                             where f.UserId == friendProfile.UserId
                             select f;
            foreach(var elem in varFriends)
            {
                friendsNextProfile = elem;
                break;
            }

            var varFriendsCurr = from f in db.FriendsProfiles
                         where f.UserId == currentProfile.UserId
                         select f;
            foreach(var elem in varFriendsCurr)
            {
                friendsCurrentProfile = elem;
                break;
            }
            
            try
            {
                if (TryUpdateModel(currentProfile))
                {

                    currentProfile.Friends.Add(friendsNextProfile);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Test = "1"; return View("FailedProfile");
                }

                if (TryUpdateModel(friendProfile))
                {
                    friendProfile.Friends.Add(friendsCurrentProfile);
                    friendProfile.FriendRequests.Remove(currentProfile);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Test = "2"; return View("FailedProfile");
                }

            }
            catch (Exception e)
            {

                ViewBag.Test = e.ToString();
                return View("FailedProfile");
            }
            return RedirectToAction("ShowFriendRequest", "Profiles");
        }

        //delete a friend Reuquest
        [Authorize(Roles ="Admin,User")]
        public ActionResult DeleteRequest(int id)
        {
            //find the current Profile
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

        //self explanatory
        [Authorize(Roles ="Admin,User")]
        public ActionResult ShowFriendRequest()
        {
            //find the current profile
            string currId = User.Identity.GetUserId();
            Profile currentProfile = new Profile();
            Profile auxProfile = new Profile();
            //the list of profiles that have a friend request on the current profile
            List<Profile> profileList = new List<Profile>();
            List<Profile> auxProfileList = new List<Profile>();

            var varProfile = from p in db.Profiles
                             where p.UserId == currId
                             select p;
            //find the current profile
            foreach(var elem in varProfile)
            {
                currentProfile = elem;
                break;
            }

            var profiles = from p in db.Profiles
                           select p;
            //get a list of all profiles
            foreach (var elem in profiles)
            {
                auxProfile = elem;
                auxProfileList.Add(auxProfile);
            }
            //get the list of all profiles that have the currentProfile as a friendReuqest
            foreach(Profile elem in auxProfileList)
            {
                if (elem.FriendRequests.Contains(currentProfile))
                    profileList.Add(elem);
            }
            

            ViewBag.profiles = profileList;

            return View("ShowFriendRequest");
        }

        //add Friend Request
        [Authorize(Roles =("Admin, User"))]
        public ActionResult FriendRequest(int id, string nickname)
        {
            //get the requested friend profile
            Profile profileRequested = db.Profiles.Find(id);
            
            //find the current profile
            string currId = User.Identity.GetUserId();
            var profiles = from p in db.Profiles
                           where p.UserId == currId
                           select p;
            //and save it 
            Profile currentProfile = new Profile();
            foreach(var elem in profiles)
            {
                currentProfile = elem;
                break;
            }
            //save it in database
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

        //done, authorized for everyone
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

        //done 
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