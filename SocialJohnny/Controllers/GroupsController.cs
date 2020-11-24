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
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var groups = from Group in db.Groups
                         select Group;
            ViewBag.groups = groups;
            return View();
        }

        [HttpPost]
        public ActionResult New(Group group)
        {

            try
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
                return RedirectToAction("Index");
            ViewBag.group = group;
            return View();
        }

        [HttpPut]
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
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == 1)
                    return RedirectToAction("Index");

                Group group= db.Groups.Find(id);
                db.Groups.Remove(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("FailedGroup");
            }
        }


    }
}