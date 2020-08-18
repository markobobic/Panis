using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.Interfaces;
using Panis.Migrations;
using Panis.Models;
using Panis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Z.EntityFramework.Plus;
using static Panis.Models.ApplicationUser;
using static Panis.Models.EmployeeEnrollment;

namespace Panis.Controllers
{
    public class AbsencesController : BaseController
    {
        IUserRepo _dbUser;
        IAbsence _dbAbsence;
        IAbsenceTypeRepo _dbAbsenceType;
        INotification _dbNotify;
        ITeamLead _dbTeamLeads;
        public AbsencesController(IUserRepo dbUser,IAbsence dbAbsence,IAbsenceTypeRepo dbAbsenceType,INotification dbNotify, ITeamLead dbTeamLeads)
        {
            _dbUser = dbUser;
            _dbAbsence = dbAbsence;
            _dbAbsenceType = dbAbsenceType;
            _dbNotify = dbNotify;
            _dbTeamLeads = dbTeamLeads;

        }
        public ActionResult Index()
        {
            ViewBag.AbsenceTypeID = _dbAbsenceType.GetAllTypesDropDown();
            return View();
        }

        public async Task<JsonResult> GetAbsencesAsync()
        {
            //getting absences for current user and sending json data
            ApplicationUser currentUser = await _dbUser.GetCurrentUser();
            var calendarData = _dbAbsence.GetCalendarData(currentUser.EmployeeID);
            return new JsonResult { Data = calendarData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
        }

        [HttpPost]
        public async Task<JsonResult> SaveAbsenceAsync(Absence e)
        {
            //saving data for current user if ID is greater than 0 that means user wanted to update his absence
            // if not then new absence is being created
            ApplicationUser currentUser = await _dbUser.GetCurrentUser();
            var status = "none";
            try { 
                if (e.AbsenceID > 0) {
                    _dbAbsence.Update(await _dbAbsence.UpdateMapDataAsync(e,currentUser.EmployeeID));
                     await _dbAbsence.SaveChangesAsync();
                    status = "edit";
                 }
                else
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var absenceToSave = _dbAbsence.MapData(e, currentUser.EmployeeID);
                        _dbAbsence.Add(absenceToSave);
                        var absenceType = await _dbAbsenceType.GetById(absenceToSave.AbsenceTypeID);
                        Notification notify = new Notification();
                        notify.Message = $"{absenceToSave.Employee.FullName} is requesting absence {absenceType.Name.ToLower()} for period from {absenceToSave.Start.ToString("MMMM dd, yyyy")} to {absenceToSave.End.ToString("MMMM dd, yyyy")} {absenceToSave.EmployeeID} ";
                        if (absenceToSave.Employee.TeamLeadID > 0 && !absenceToSave.Employee.IsTeamLead)
                        {
                            notify.EmployeeID = _dbTeamLeads.GetById(absenceToSave.Employee.TeamLeadID).EmployeeID;
                        }
                        await _dbUser.UpdateAllWithIncrementedNotification(WorkPosition.HR,1);
                        status = "save";
                        _dbNotify.Add(notify);
                        await _dbAbsence.SaveChangesAsync();
                        await _dbNotify.SaveChangesAsync(); 
                        dbContextTransaction.Commit();
                    }
                   
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return new JsonResult { Data = new { status } };
           
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAbsence(int absenceID)
        {
            var status = false;
            await _dbAbsence.DeleteAsync(absenceID); 
            status = true;
            await _dbAbsence.SaveChangesAsync();    
            return new JsonResult { Data = new { status = status } };
        }
        
        public enum Senior
        {
            Junior,
            Senior,
            Medior
        }
    }

   
}