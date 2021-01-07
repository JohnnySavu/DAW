using Microsoft.AspNet.Identity;
using SocialJohnny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace SocialJohnny.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int _perPage = 5;

        private void SendEmailNotification(string toEmail, string subject, string content)
        {
            const string senderEmail = "socialjohnnywithusu @gmail.com";
            const string senderPassword = "zy5nX5DkkwEi6cL";
            const string smtpServer = "smtp.gmail.com";
            const int smtpPort = 587;

            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

            MailMessage email = new MailMessage(senderEmail, toEmail, subject, content);

            email.IsBodyHtml = true;

            email.BodyEncoding = UTF8Encoding.UTF8;

            try
            {
                System.Diagnostics.Debug.WriteLine("Sendin email...");
                smtpClient.Send(email);
                System.Diagnostics.Debug.WriteLine("Email sent!");
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while trying to send email");
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                RedirectToAction("Index", "Home");
            }
        }


        public ActionResult Index()
        {

            if (TempData.ContainsKey("DeletePost"))
                ViewBag.deleteMessage = TempData["DeletePost"].ToString();

            if (TempData.ContainsKey("AddPost"))
                ViewBag.addMessage = TempData["AddPost"].ToString();

            if (TempData.ContainsKey("EditPost"))
                ViewBag.editMessage = TempData["EditPost"].ToString();


            if (TempData.ContainsKey("Allow"))
                ViewBag.editMessage = TempData["Allow"].ToString();


            var posts = from post in db.Posts
                        where post.GroupId == 1
                        orderby post.Date
                        select post;

            var totalItems = posts.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedPosts = posts.Skip(offset).Take(this._perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);

            ViewBag.Posts = paginatedPosts;
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                    ViewBag.IsAdmin = true;
            }
            else
                ViewBag.UserId = "";

            return View();
        }

        public ActionResult IndexReload(Post reloadPost)
        {
            if (TempData.ContainsKey("DeletePost"))
                ViewBag.deleteMessage = TempData["DeletePost"].ToString();

            if (TempData.ContainsKey("AddPost"))
                ViewBag.addMessage = TempData["AddPost"].ToString();

            if (TempData.ContainsKey("EditPost"))
                ViewBag.editMessage = TempData["EditPost"].ToString();

            if (TempData.ContainsKey("Allow"))
                ViewBag.editMessage = TempData["Allow"].ToString();

            var posts = from post in db.Posts
                        where post.GroupId == 1
                        select post;

            ViewBag.Posts = posts;
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                    ViewBag.IsAdmin = true;
            }
            else
                ViewBag.UserId = "";

            return View("Index", reloadPost);
        }



        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult New(Post post)
        {
            string currId = User.Identity.GetUserId();
            Profile currentProfile = db.Profiles.Where(p => p.UserId == currId).ToList().First();

            post.OwnerNickname = currentProfile.Nickname;

            post.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
            post.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["AddPost"] = "Postarea a fost creata cu succes";
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("IndexReload", post);
                }
            }
            catch (Exception e)
            {
                return View("FailedPost");
            }
        }

        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
                return RedirectToAction("Index");

            if (post.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                TempData["Allow"] = "Nu aveti suficiente drepturi";
                return RedirectToAction("Index");
            }
            //ViewBag.post = post;
            return View(post);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);
                if (post.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
                {
                    TempData["Allow"] = "Nu aveti suficiente drepturi";
                    return RedirectToAction("Index");
                }
                if(User.IsInRole("Admin"))
                {
                    string authorEmail = db.Profiles.Where(p => p.UserId == post.UserId).ToList().First().Email;
                    string notificationBody = "<p>O postare de a dumneavoastra a fost modificata de catre administrator</p>";

                    SendEmailNotification(authorEmail, "O postare a fost modificata", notificationBody);
                }

                if (TryUpdateModel(post))
                {
                    post.Text = requestPost.Text;
                    post.Title = requestPost.Title;
                    post.Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    db.SaveChanges();
                }
                TempData["EditPost"] = "Postarea " + post.Title + " modificata cu succes!";
                return RedirectToAction("Index");
       
            }
             catch(Exception e)
            {
                return View("FailedPost");
            }
        }


        public ActionResult DeletePosts(int id)
        {
            try
            {
                var posts = from post in db.Posts
                            where post.GroupId == id
                            select post;
                foreach (Post post in posts)
                {
                    var comments = from comment in db.Comments
                                   where comment.PostId == id
                                   select comment;
                    foreach (Comment com in comments)
                    {
                        db.Comments.Remove(com);
                        //db.SaveChanges();
                    }
                    db.Posts.Remove(post);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return View("FailedPost");
            }
        }


        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Delete(int id)
        {
            try
            {
                Post post = db.Posts.Find(id);

                if (post.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
                {
                    TempData["Allow"] = "Nu aveti suficiente drepturi";
                    return RedirectToAction("Index");
                }

                var comments = from comment in db.Comments
                               where comment.PostId == id
                               select comment;
                foreach (Comment com in comments)
                {
                    db.Comments.Remove(com);
                    //db.SaveChanges();
                }
                
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["DeletePost"] = "Postarea " + post.Title + " a fost stearsa cu succes";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("FailedPost");
            }
        }


    }
}