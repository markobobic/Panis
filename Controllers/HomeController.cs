using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using Microsoft.AspNet.Identity.EntityFramework;
using static Panis.Models.ApplicationUser;

namespace Panis.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
            

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private UserManager<ApplicationUser> GetManager()
        {
            var store = new UserStore<ApplicationUser>(db);
            var manager = new UserManager<ApplicationUser>(store);
            return manager;
        }

        public async Task<ActionResult> UserDashBoard()
        {
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            ViewBag.Level = db.employeeEnrollments.Find(currentUser.EmployeeID).Seniority.ToString();
            ViewBag.NumberOfProjects = db.EmployeesProjects.Where(x => x.EmployeeID == currentUser.EmployeeID).Count();
            var currentEmployee = await db.Employees.FindAsync(currentUser.EmployeeID);
            return View(currentEmployee);
            
        }
        [HttpPost]
        public async Task<JsonResult> DeleteNotifyCount(int deleteCount = 0)
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            if (currentUser.ReadNotifications == false && deleteCount > 0)
            {
                currentUser.ReadNotifications = true;
                currentUser.CountNotifications = currentUser.CountNotifications - deleteCount;
                await System.Web.HttpContext.Current.GetOwinContext().
                GetUserManager<ApplicationUserManager>().UpdateAsync(currentUser);
               
            }
            return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult _DashBoardPartial()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var currentEmployee = db.Employees.Find(currentUser.EmployeeID);
            ViewBag.FullName = currentEmployee.FullName;
            ViewBag.Photo = currentEmployee.Photo;
            ViewBag.PhotoType = currentEmployee.PhotoType;
            ViewBag.Position = currentUser.EmpPosition.ToString();
            if (currentUser.ReadNotifications == true)
            {
                ViewBag.CountNotify = 0;
                ViewBag.Notify = db.Notifications.OrderByDescending(x => x.ID).Take(6).Where(x=>x.EmployeeID==currentEmployee.EmployeeID).Select(x => x.Message).ToList();
            }
            else
            {
                ViewBag.Notify = db.Notifications.Where(x=>x.EmployeeID==currentEmployee.EmployeeID).OrderByDescending(x => x.ID).
                Take((int?)currentUser.CountNotifications ==null  ? 0 : (int)currentUser.CountNotifications).Where(x=>x.EmployeeID==currentEmployee.EmployeeID).Select(x => x.Message).ToList();
                ViewBag.CountNotify = currentUser.CountNotifications;
            } 

            if(currentUser.EmpPosition== WorkPosition.HR)
            {
                ViewBag.Notify = db.Notifications.OrderByDescending(x => x.ID)
                .Select(x => x.Message).ToList();
                ViewBag.CountNotify = currentUser.CountNotifications;
            }

            return PartialView("~/Views/Shared/_DashBoard.cshtml");

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}