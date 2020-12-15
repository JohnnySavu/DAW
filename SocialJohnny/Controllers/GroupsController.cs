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


            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;
            var groups = from Group in db.Groups
                         select Group;
            ViewBag.groups = groups;

            

            return View();
        }

        public ActionResult ViewPosts(int id)
        {
            var posts = from p in db.Posts
                        where p.GroupId == id
                        select p;
            ViewBag.groupId = id;
            ViewBag.posts = posts;
            return View();
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
                    TempData["AddedToGroup"] = "Successfully added to group!";
                    group.Profiles.Add(profileToAdd);
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