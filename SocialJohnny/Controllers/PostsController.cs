using SocialJohnny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SocialJohnny.Controllers
{
    public class PostsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Posts
        public ActionResult Index()
        {
            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;

            return View();
        }

 
        [HttpPost]
        public ActionResult New(Post post)
        {
            post.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");

            try
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("FailedPost");
            }
        }

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
                return RedirectToAction("Index");
            ViewBag.post = post;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);
                if(TryUpdateModel(post))
                {
                    post.Text = requestPost.Text;
                    post.Title = requestPost.Title;
                    post.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
       
            }
             catch(Exception e)
            {
                return View("FailedPost");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var comments = from comment in db.Comments
                               where comment.PostId == id
                               select comment;
                foreach (Comment com in comments)
                {
                    db.Comments.Remove(com);
                    //db.SaveChanges();
                }
                Post post = db.Posts.Find(id);
                db.Posts.Remove(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("FailedPost");
            }
        }





    }
}