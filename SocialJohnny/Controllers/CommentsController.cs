﻿using Microsoft.AspNet.Identity;
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
    public class CommentsController : Controller
    {
        // GET: Comments
        private void SendEmailNotification(string toEmail, string subject, string content)
        {
            const string senderEmail = "ioan-daniel.savu@my.fmi.unibuc.ro";
            const string senderPassword = "parola";
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error occured while trying to send email");
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                RedirectToAction("Index", "Home");
            }
        }


        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int id)
        {
            if (TempData.ContainsKey("DeleteComment"))
                ViewBag.deleteMessage = TempData["DeleteComment"].ToString();

            if (TempData.ContainsKey("AddComment"))
                ViewBag.addMessage = TempData["AddComment"].ToString();

            if (TempData.ContainsKey("EditComment"))
                ViewBag.editMessage = TempData["EdditComment"].ToString();

            if (TempData.ContainsKey("Allow"))
                ViewBag.Allow = TempData["Allow"].ToString();


            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                    ViewBag.IsAdmin = true;
            }
            else
                ViewBag.UserId = "";


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

            if (TempData.ContainsKey("Allow"))
                ViewBag.Allow = TempData["Allow"].ToString();


            var comments = from comment in db.Comments
                           where comment.PostId == id
                           select comment;

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                if (User.IsInRole("Admin"))
                    ViewBag.IsAdmin = true;
            }
            else
                ViewBag.UserId = "";

            ViewBag.comments = comments;
            ViewBag.PostId = id;
            return View("Index",reloadComment);

        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult New(Comment comment)
        {

            string currId = User.Identity.GetUserId();
            Profile currentProfile = db.Profiles.Where(p => p.UserId == currId).ToList().First();

            comment.OwnerNickname = currentProfile.Nickname;

            comment.UserId = User.Identity.GetUserId();
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

        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id)
        {

            Comment comment = db.Comments.Find(id);
            if (comment == null)
                return View("FailedComment");
            if (comment.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                TempData["Allow"] = "Nu aveti suficiente drepturi";
                return RedirectToAction("Index/" + comment.PostId.ToString());
            }

            return View(comment);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                 
                Comment comment = db.Comments.Find(id);
                if (comment.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
                {
                    TempData["Allow"] = "Nu aveti suficiente drepturi";
                    return RedirectToAction("Index/" + comment.PostId.ToString());
                }

                if (User.IsInRole("Admin"))
                {
                    string authorEmail = db.Profiles.Where(p => p.UserId == comment.UserId).ToList().First().Email;
                    string notificationBody = "<p>Un comentariu de al dumneavoastra a fost modificat de catre administrator</p>";

                    SendEmailNotification(authorEmail, "Un comentariu a fost modificat", notificationBody);
                }

                if (TryUpdateModel(comment))
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
        [Authorize(Roles = "Admin,User")]
        public ActionResult Delete(int id)
        {
            try
            {
                Comment comment= db.Comments.Find(id);
                if (comment.UserId != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
                {
                    TempData["Allow"] = "Nu aveti suficiente drepturi";
                    return RedirectToAction("Index/" + comment.PostId.ToString());
                }
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["DeleteComment"] = "Commentariul a fost sters cu succes";

                return RedirectToAction("Index/" + comment.PostId.ToString());
            }
            catch (Exception e)
            {
                return View("FailedComment");
            }
        }

    }
}