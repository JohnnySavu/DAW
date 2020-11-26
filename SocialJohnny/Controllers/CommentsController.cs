using SocialJohnny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialJohnny.Controllers
{
    public class CommentsController : Controller
    {
        // GET: Comments
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int id)
        {
            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;

            ViewBag.comments = comments;
            ViewBag.PostId = id;
            return View();
        }

        [HttpPost]
        public ActionResult New(Comment comment)
        {
            comment.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            try
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index/" + comment.PostId.ToString());
            } 
            catch (Exception e)
            {
                return View("FailedComment");
            }
        }

        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
                return RedirectToAction("FailedComment");

            return View(comment);
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if(TryUpdateModel(comment))
                {
                    comment.Text = requestComment.Text;
                    comment.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    db.SaveChanges();
                }
                return RedirectToAction("Index/" + comment.PostId.ToString());
            }
            catch (Exception e)
            {
                return View("FailedComment");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Comment comment= db.Comments.Find(id);
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Index/" + comment.PostId.ToString());
            }
            catch (Exception e)
            {
                return View("FailedComment");
            }
        }

    }
}