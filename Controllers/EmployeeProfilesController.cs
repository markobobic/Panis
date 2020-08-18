using MoreLinq;
using Panis.Extensions;
using Panis.Interfaces;
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
    public class EmployeeProfilesController : BaseController
    {
        IUserRepo _dbUser;
        INotification _dbNotify;
        ITeamLead _dbTeamLeads;
        IEmployee _dbEmployees;
        IProject _dbProjects;
        IRealization _dbRealizations;
        IAbsence _dbAbsences;
        IEmployeeEnrollments _dbEmployeeEnrollments;
        IAbsenceTypeRepo _dbAbsencesType;
        public EmployeeProfilesController(IUserRepo dbUser, INotification dbNotify,
            IProject dbProjects, IAbsenceTypeRepo dbAbsencesType, IRealization dbRealizations,IAbsence dbAbsences, ITeamLead dbTeamLeads, IEmployee dbEmployees, IEmployeeEnrollments dbEEmployeeEnrollments)
        {
            _dbUser = dbUser;
            _dbNotify = dbNotify;
            _dbTeamLeads = dbTeamLeads;
            _dbEmployees = dbEmployees;
            _dbEmployeeEnrollments = dbEEmployeeEnrollments;
            _dbRealizations = dbRealizations;
            _dbProjects = dbProjects;
            _dbAbsences = dbAbsences;
            _dbAbsencesType = dbAbsencesType;

        }

        // GET: EmployeeProfiles
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> EmployeeProfile(int id)
        {
            //by employee id we are getting all necesessery data to view
            Session["EmployeeID"] = id;
            var emp = await _dbEmployees.GetEmployeeByID(id);
            ViewBag.FullName = emp.FullName;
            ViewBag.PhotoType = emp.PhotoType;
            ViewBag.EmpPhoto = emp.Photo;
            ViewBag.City = emp.LivingCity;
            var teamLead =  _dbTeamLeads.GetById(emp.TeamLeadID);
            var currentTeamLead =await _dbEmployees.GetEmployeeByID(teamLead.EmployeeID);
            ViewBag.TeamLeadPhoto = currentTeamLead.Photo;
            ViewBag.TeamLeadPhotoType = currentTeamLead.PhotoType;
            ViewBag.Seniority = _dbEmployeeEnrollments.GetSeniority(id);
            var latestRealization = await _dbRealizations.GetLatestRealization(emp.EmployeeID);
            if(latestRealization == null)
            {
                ViewBag.CurrentProject = "No current project";
            }
            else
            {
                var selectCurrentProject = await _dbProjects.GetLatestProject(latestRealization.ProjectID);
                ViewBag.CurrentProject = selectCurrentProject == null ? "No current project" : selectCurrentProject.Name;
            }
           
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetAbsences()
        {
            //getting absences for particular employee to present in table 
               int employeeID = (int)Session["EmployeeID"];
               var absencesRequests = _dbAbsences.GetRequestAbsencesAndTypes(employeeID);
               var data = await absencesRequests
               .Select(x => new { AbsenceID = x.AbsenceName.AbsenceID,TypeOfAbsence = x.AbsenceType.Name, Start = x.AbsenceName.Start, End = x.AbsenceName.End }).ToListAsync();
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
            var user = await _dbUser.GetUserByEmployeeID(employeeID);
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            _dbRealizations.PopulateCalendarWhenEmployeeAbsent(absence.Start, absence.End, employeeID);
            await db.SaveChangesAsync();
            return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> DeclineAbsence(int AbsenceID,string Reason)
        {
            //if absence is declined then we are sending notification to that user and also to all hr staff
            //absence is also being deleted 
            var absence = db.Absences.Find(AbsenceID);
            var absenceType = db.AbsenceTypes.Find(absence.AbsenceTypeID);
            int employeeID = (int)Session["EmployeeID"];
            Notification notify = new Notification();
            if(Reason == null || Reason == "")
            {
                notify.Message = $"Your request for {absenceType.Name.ToLower()} from {absence.Start.ToString("MMMM dd, yyyy")} to {absence.End.ToString("MMMM dd, yyyy")} has been declined.";
            }
            else
            {
                notify.Message = $"Your request for {absenceType.Name.ToLower()} from {absence.Start.ToString("MMMM dd, yyyy")} to {absence.End.ToString("MMMM dd, yyyy")} has been declined.\nReason: {Reason}";
            }
            notify.EmployeeID = employeeID;
            db.Notifications.Add(notify);
            var user = await _dbUser.GetUserByEmployeeID(employeeID);
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