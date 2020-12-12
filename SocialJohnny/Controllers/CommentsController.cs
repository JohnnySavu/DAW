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
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int id)
        {
            if (TempData.ContainsKey("DeleteComment"))
                ViewBag.deleteMessage = TempData["DeleteComment"].ToString();

            if (TempData.ContainsKey("AddComment"))
                ViewBag.addMessage = TempData["AddComment"].ToString();

            if (TempData.ContainsKey("EditComment"))
                ViewBag.editMessage = TempData["EdditComment"].ToString();


            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;

            ViewBag.comments = comments;
            ViewBag.PostId = id;
            return View();
        }

        public ActionResult IndexReload(int id,Comment reloadComment)
        {
            if (TempData.ContainsKey("DeleteComment"))
                ViewBag.deleteMessage = TempData["DeleteComment"].ToString();

            if (TempData.ContainsKey("AddComment"))
                ViewBag.addMessage = TempData["AddComment"].ToString();

            if (TempData.ContainsKey("EditComment"))
                ViewBag.editMessage = TempData["EdditComment"].ToString();


            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;

            ViewBag.comments = comments;
            ViewBag.PostId = id;
            return View("Index",reloadComment);

        }

        [HttpPost]
        public ActionResult New(Comment comment)
        {
            comment.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    TempData["AddComment"] = "Commentul a fost creata cu succes";
                    return RedirectToAction("Index/" + comment.PostId.ToString());
                }
                else
                    return RedirectToAction("IndexReload/" + comment.PostId.ToString(), comment);
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
                TempData["DeleteComment"] = "Commentariul a fost stears cu succes";

                return RedirectToAction("Index/" + comment.PostId.ToString());
            }
            catch (Exception e)
            {
                return View("FailedComment");
            }
        }

    }
}