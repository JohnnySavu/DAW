using Microsoft.AspNet.Identity;
using SocialJohnny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialJohnny.Controllers
{
    public class GroupsController : Controller
    {
        // GET: Groups
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            

            if (TempData.ContainsKey("AddedToGroup"))
                ViewBag.addedToGroup = TempData["AddedToGroup"].ToString();

            if (TempData.ContainsKey("DeleteGroup"))
                ViewBag.deleteMessage = TempData["DeleteGroup"].ToString();

            if (TempData.ContainsKey("AddGroup"))
                ViewBag.addMessage = TempData["AddGroup"].ToString();

            if (TempData.ContainsKey("EditGroup"))
                ViewBag.editMessage = TempData["EditGroup"].ToString();

            ViewBag.IsAdmin = false;
            ViewBag.IsLoged = false;
            Profile currentProfile = new Profile();
            if (Request.IsAuthenticated)
            {
                ViewBag.isLoged = true;
                ViewBag.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                {
                    ViewBag.ShowButton = true;
                    ViewBag.IsAdmin = true;
                }
                else
                {
                    string currId = User.Identity.GetUserId();
                    currentProfile = db.Profiles.Where(p => p.UserId == currId).ToList().First();
                }
            }
            else
                ViewBag.UserId = "";

            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;
            var groups = from Group in db.Groups
                         select Group;
            ViewBag.groups = groups;
            ViewBag.profile = currentProfile;
            

            return View();
        }

        //[Authorize(Roles="Admin,User")]
        public ActionResult ViewPosts(int id)
        {

            bool ok = false;
            Group currentGroup = db.Groups.Find(id);
            if (currentGroup.IsPrivate == false)
            {
                ok = true;
            }
            else {
                if (Request.IsAuthenticated) {
                    string currId = User.Identity.GetUserId();
                    Profile currentProfile = db.Profiles.Where(p => p.UserId == currId).ToList().First();

                    if (currentGroup.Profiles.Contains(currentProfile))
                        ok = true;
                    if (User.IsInRole("Admin"))
                        ok = true;
                }
            }
            
            if (ok == true)
            {
                var posts = from p in db.Posts
                            where p.GroupId == id
                            select p;
                ViewBag.groupId = id;
                ViewBag.posts = posts;
                return View();
            }
            else
                return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult AddToGroup(int id)
        {

            Group group = db.Groups.Find(id);
            Profile profileToAdd = new Profile();
            string currentId = User.Identity.GetUserId();

            var profiles = from _profile in db.Profiles
                          where _profile.UserId == currentId
                          select _profile;
            foreach(var profile in profiles)
            {
                profileToAdd = profile;
            }

            try
            {
                if (TryUpdateModel(group))
                {

                    if (group.Profiles.Contains(profileToAdd))
                    {
                        group.Profiles.Remove(profileToAdd);
                        TempData["AddedToGroup"] = "Ai parasit grupul cu succes!";
                    }
                    else
                    {
                        group.Profiles.Add(profileToAdd);
                           TempData["AddedToGroup"] = "Te-ai alaturat cu succes!";
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "Groups");
                }
                else
                    return View("FailedGroup");
            }
            catch (Exception e)
            {
                ViewBag.Exception = e;
                return View("FailedGroup");
            }

        }

        public ActionResult IndexReload(Group reloadGroup)
        {
            if (TempData.ContainsKey("DeleteGroup"))
                ViewBag.deleteMessage = TempData["DeleteGroup"].ToString();

            if (TempData.ContainsKey("AddGroup"))
                ViewBag.addMessage = TempData["DeleteGroup"].ToString();

            if (TempData.ContainsKey("EditGroup"))
                ViewBag.editMessage = TempData["EditGroup"].ToString();
            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;
            var groups = from Group in db.Groups
                         select Group;
            ViewBag.groups = groups;
            return View("Index", reloadGroup);
        }

        [HttpPost]
        [Authorize(Roles ="Admin,User")]
        public ActionResult New(Group group)
        {
            string currentId = User.Identity.GetUserId();
            group.UserId = currentId;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(group);
                    db.SaveChanges();
                    TempData["AddGroup"] = "Grupul a fost creat cu succes";
                    return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("IndexReload", group);
            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }
        [Authorize(Roles ="Admin,User")]
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
                return RedirectToAction("Index");
            ViewBag.group = group;
            return View();
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                Group group= db.Groups.Find(id);
                if (TryUpdateModel(group))
                {
                    group.Name = requestGroup.Name;
                    group.IsPrivate = requestGroup.IsPrivate;
                    db.SaveChanges();
                }
                TempData["EditGroup"] = "Grupul " + group.Name + " modificata cu succes!";

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == 1)
                    return RedirectToAction("Index");

                Group group= db.Groups.Find(id);
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["DeleteGroup"] = "Grupul " + group.Name + " a fost stears cu succes";

                return RedirectToAction("DeletePosts/" + id.ToString(), "Posts");
            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }


    }
}