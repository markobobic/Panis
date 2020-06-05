using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Panis.Controllers
{
    public class SendMailController : Controller
    {
        // GET: SendMail
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SendEMail(string button)
        {
            if(button == "deliver") 
            { 
            ViewBag.Subject = (string)Session["fullName"] + "- Dostava dokumentacije za bolovanje";
            }
            else
            {
                ViewBag.Subject = (string)Session["fullName"] + "- Zahtev za HR-a 1 na 1";
            }

            return View();
        }
        [HttpPost]
        public ActionResult SendEMail(MailModel objModelMail, HttpPostedFileBase fileUploader)
        {
            MailMessage msg = new MailMessage("bobic015@gmail.com", objModelMail.To);
            msg.Subject = objModelMail.Subject;
            msg.Body = objModelMail.Body;
            if (fileUploader != null)
            {
                string fileName = Path.GetFileName(fileUploader.FileName);
                msg.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
            }
            msg.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential("bobic015@gmail.com", "madafaka123");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(msg);
            ViewBag.Message = "j doucc";
            return RedirectToAction("Index", objModelMail);
        }

        
    }
}