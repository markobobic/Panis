using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.Migrations;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Z.EntityFramework.Plus;
using static Panis.Models.ApplicationUser;
using static Panis.Models.EmployeeEnrollment;

namespace Panis.Controllers
{
    public class AbsencesController : Controller
    {
        public enum Senior
        {
            Junior,
            Senior,
            Medior
        }
        // GET: Absences
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.AbsenceTypeID = new SelectList(db.AbsenceTypes.ToList(), "AbsenceTypeID", "Name");
            return View();
        }

        public async Task<JsonResult> GetAbsencesAsync()
        {
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var employeeID = currentUser.EmployeeID;
            var events = db.Absences.Where(x => x.EmployeeID == employeeID).Select(
                x => new
                {
                    AbsenceID = x.AbsenceID,
                    Start = x.Start,
                    End = x.End,
                    ApplicationDate = x.ApplicationDate,
                    Description = x.Description,
                    AbsenceTypeID=x.AbsenceTypeID,
                    Approved = x.Approved
                }


                ).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
        }

        [HttpPost]
        public async Task<JsonResult> SaveAbsenceAsync(Absence e)
        {
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
           GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var status = "none";
            try
            {
                if (e.AbsenceID > 0)
                {
                    var v = db.Absences.Where(a => a.AbsenceID == e.AbsenceID).FirstOrDefault();
                    if (v != null)
                    {
                        v.EmployeeID = currentUser.EmployeeID;
                        v.Start = e.Start;
                        v.Description = e.Description;
                        v.End = e.End;
                        v.ApplicationDate = DateTime.Now;
                    }
                    status = "edit";
                }
                else
                {
                    Absence ev = new Absence();
                    ev.Description = e.Description;
                    ev.EmployeeID = currentUser.EmployeeID;
                    ev.Start = e.Start;
                    ev.Employee = db.Employees.Find(ev.EmployeeID);
                    ev.End = e.End;
                    ev.AbsenceTypeID = e.AbsenceTypeID;
                    ev.ApplicationDate = DateTime.Now;
                    ev.Approved = false;
                    db.Absences.Add(ev);
                    Notification notify = new Notification();
                    var absenceType = db.AbsenceTypes.Find(ev.AbsenceTypeID); 
                    notify.Message = $"{ev.Employee.FullName} is requesting {absenceType.Name.ToLower()} for period from {ev.Start.ToString("MMMM dd, yyyy")} to {ev.End.ToString("MMMM dd, yyyy")} {ev.EmployeeID} ";
                    if (ev.Employee.TeamLeadID > 0)
                    {
                       notify.EmployeeID = db.TeamLeads.Where(x => x.TeamLeadID == ev.Employee.TeamLeadID).FirstOrDefault().EmployeeID;
                    }
                    db.Users.Where(x => x.EmpPosition == WorkPosition.HR)
                   .Update(x => new ApplicationUser { ReadNotifications = false, CountNotifications = x.CountNotifications + 1 });
                    status = "save";
                    db.Notifications.Add(notify);
                    await db.SaveChangesAsync();
                }
            }
            catch
            {

            }

            return new JsonResult { Data = new { status } };

           
        }

        [HttpPost]
        public JsonResult DeleteAbsence(int absenceID)
        {
            var status = false;
           
                var v = db.Absences.Where(a => a.AbsenceID == absenceID).FirstOrDefault();
                if (v != null)
                {
                    db.Absences.Remove(v);
                    db.SaveChanges();
                    status = true;
                }
            return new JsonResult { Data = new { status = status } };
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

    public class Nesto
    {
        public ApplicationUser ApplicationUser { get; set; }

        public EmployeeEnrollment EmployeeEnrollment { get; set; }
    }
}