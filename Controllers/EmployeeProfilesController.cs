using MoreLinq;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Z.EntityFramework.Plus;

namespace Panis.Controllers
{
    public class EmployeeProfilesController : Controller
    {
        // GET: EmployeeProfiles
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> EmployeeProfile(int id)
        {
            //by employee id we are getting all necesessery data to view
            Session["EmployeeID"] = id;
            var emp = db.Employees.Find(id);
            ViewBag.FullName = emp.FullName;
            ViewBag.PhotoType = emp.PhotoType;
            ViewBag.EmpPhoto = emp.Photo;
            ViewBag.City = emp.LivingCity;
            var teamLead = await db.TeamLeads.FindAsync(emp.TeamLeadID);
            var currentTeamLead =await db.Employees.FindAsync(teamLead.EmployeeID);
            ViewBag.TeamLeadPhoto = currentTeamLead.Photo;
            ViewBag.TeamLeadPhotoType = currentTeamLead.PhotoType;
            ViewBag.Seniority = db.employeeEnrollments.Where(x=>x.EmployeeID==emp.EmployeeID).FirstOrDefault().Seniority.ToString();
            var currentProject = await db.Realizations.Where(x => x.EmployeeID == emp.EmployeeID).OrderByDescending(x => x.RealizationID).FirstOrDefaultAsync();
            ViewBag.CurrentProject = db.Projects.Where(x => x.ProjectID == currentProject.ProjectID).FirstOrDefault().Name;    
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetAbsences()
        {
            //getting absences for particular employee to present in table 
               int employeeID = (int)Session["EmployeeID"];
               var dataJoin  =  db.Absences.Join(db.AbsenceTypes,
               absence => absence.AbsenceTypeID,
               absenceType => absenceType.AbsenceTypeID,
               (absence, absenceType) => new { Absence = absence, AbsenceType = absenceType })
               .Where(x => x.Absence.EmployeeID == employeeID && x.Absence.Approved==false);
                var data = await dataJoin
                .Select(x => new { AbsenceID = x.Absence.AbsenceID,TypeOfAbsence = x.AbsenceType.Name, Start = x.Absence.Start, End = x.Absence.End }).ToListAsync();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<JsonResult> ApproveAbsence(int AbsenceID)
        {
            //if absence is approved then we are sending notification to that user and also to all hr staff
            var absence = db.Absences.Find(AbsenceID);
            var absenceType = db.AbsenceTypes.Find(absence.AbsenceTypeID);
            absence.Approved = true;
            int employeeID = (int)Session["EmployeeID"];
            Notification notify = new Notification();
            notify.Message = $"Your request for {absenceType.Name.ToLower()} from {absence.Start.ToString("MMMM dd, yyyy")} to {absence.End.ToString("MMMM dd, yyyy")} has been approved";
            notify.EmployeeID = employeeID;
            db.Notifications.Add(notify);
            var user = db.Users.Where(x => x.EmployeeID == employeeID).FirstOrDefault();
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            await db.SaveChangesAsync();
            return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> DeclineAbsence(int AbsenceID)
        {
            //if absence is declined then we are sending notification to that user and also to all hr staff
            //absence is also being deleted 
            var absence = db.Absences.Find(AbsenceID);
            var absenceType = db.AbsenceTypes.Find(absence.AbsenceTypeID);
            int employeeID = (int)Session["EmployeeID"];
            Notification notify = new Notification();
            notify.Message = $"Your request for {absenceType.Name.ToLower()} from {absence.Start.ToString("MMMM dd, yyyy")} to {absence.End.ToString("MMMM dd, yyyy")} has been declined";
            notify.EmployeeID = employeeID;
            db.Notifications.Add(notify);
            var user = db.Users.Where(x => x.EmployeeID == employeeID).FirstOrDefault();
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Absences.Remove(absence);
            await db.SaveChangesAsync();
            return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
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