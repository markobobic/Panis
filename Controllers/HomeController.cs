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
using Panis.Extensions;
using System.Data.Entity;

namespace Panis.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public enum Departments
        {
            Bugfixing=1,
            Development,
            Training,
            Testing,
            TwentyFourSeven
        }
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

        public async Task<ActionResult> UserDashBoard()
        {
            //mapping data to userDashBoard 
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            ViewBag.Level = db.employeeEnrollments.Where(x=>x.EmployeeID==currentUser.EmployeeID).FirstOrDefault().Seniority.ToString();
            ViewBag.NumberOfProjects = db.EmployeesProjects.Where(x => x.EmployeeID == currentUser.EmployeeID).Count();
            var startDate = db.employeeEnrollments.Where(x => x.EmployeeID == currentUser.EmployeeID).FirstOrDefault().WorkStart;
            var endDate = DateTime.Now;
            ViewBag.Experience = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
            var groupByDepartments = db.Realizations.Where(x => x.EmployeeID == currentUser.EmployeeID).GroupBy(x => x.DepartmentID).ToList();
            #region Group departments
            foreach (var departments in groupByDepartments)
            {
                if (departments.Key == (int)Departments.Bugfixing)
                {
                    ViewBag.Bugfixing = departments.Sum(x => x.Hours);
                } 
                if (departments.Key == (int)Departments.Development)
                {
                    ViewBag.Development = departments.Sum(x => x.Hours);
                }
                if (departments.Key == (int)Departments.Training)
                {
                    ViewBag.Training = departments.Sum(x => x.Hours);
                }
                if (departments.Key == (int)Departments.Testing)
                {
                    ViewBag.Testing = departments.Sum(x => x.Hours);
                }
                if (departments.Key == (int)Departments.TwentyFourSeven)
                {
                    ViewBag.TwentyFourSeven = departments.Sum(x => x.Hours);
                }
                #endregion
            }
            ViewBag.Bugfixing = ViewBag.Bugfixing == null ? 0 : ViewBag.Bugfixing;
            ViewBag.Training = ViewBag.Training == null ? 1 : ViewBag.Training;
            ViewBag.Development = ViewBag.Development == null ? 0 : ViewBag.Development;
            ViewBag.Testing = ViewBag.Testing == null ? 0 : ViewBag.Testing;
            ViewBag.TwentyFourSeven = ViewBag.TwentyFourSeven == null ? 0 : ViewBag.TwentyFourSeven;
            var currentEmployee = await db.Employees.FindAsync(currentUser.EmployeeID);
            return View(currentEmployee);
            
        }
        [HttpPost]
        public async Task<JsonResult> DeleteNotifyCount(int deleteCount = 0)
        {
            //this function is triggerd when user clicks on notification bell
            //if there is more then zero unread notifications then we are updating
            //two columns in user table readnotificaiton and count notifications 
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
            Session["fullName"] = currentEmployee.FullName;
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

            if(currentUser.EmpPosition== WorkPosition.HR && currentUser.ReadNotifications==true)
            {
                ViewBag.Notify = db.Notifications.OrderByDescending(x => x.ID)
                .Select(x => x.Message).ToList();
                ViewBag.CountNotify = 0;
            }
            else if(currentUser.EmpPosition == WorkPosition.HR && currentUser.ReadNotifications == false)
            {
                ViewBag.Notify = db.Notifications.OrderByDescending(x => x.ID).
                Take((int?)currentUser.CountNotifications == null ? 0 : (int)currentUser.CountNotifications).Select(x => x.Message).ToList();
                ViewBag.CountNotify = currentUser.CountNotifications;
            }

            return PartialView("~/Views/Shared/_DashBoard.cshtml");

        }

        [HttpGet]
        public async Task<JsonResult> GetProjects()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var currentEmployee = db.Employees.Find(currentUser.EmployeeID);
            var data = await
            (from empProject in db.EmployeesProjects
             join project in db.Projects on empProject.ProjectID equals project.ProjectID
             join sector in db.Sectors on project.SectorID equals sector.SectorID
             where empProject.EmployeeID == currentEmployee.EmployeeID
             select new
             {
                 ProjectID = project.ProjectID,
                 ProjectOwner = sector.Name,
                 ProjectName = project.Name
             }).ToListAsync();

            return Json(data, JsonRequestBehavior.AllowGet);

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