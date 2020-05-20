using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MoreLinq;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Panis.Controllers
{
    public class RealizationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.ProjectID = new SelectList(db.Projects.ToList(), "ProjectID", "Name");
            ViewBag.RealizationTypeID = new SelectList(db.RealizationTypes.ToList(), "RealizationTypeID", "Name");
            ViewBag.DepartmentID = new SelectList(db.Departments.ToList(), "DepartmentID", "Name");
            return View();
        }
       
        public async Task<JsonResult> GetEvents()
        {
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var employeeID = currentUser.EmployeeID;
            var events = db.Realizations.Where(x => x.EmployeeID == employeeID).Select(
                x=> new
                {
                                RealizationID = x.RealizationID,
                                Subject= x.Subject,
                                Description= x.Description,
                                Start =x.Start,
                                Hours= x.Hours,
                                RealizationTypeID= x.RealizationTypeID,
                                ProjectID= x.ProjectID,
                                DepartmentID = x.DepartmentID
                }

                
                ).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public async Task<JsonResult> SaveEvent(Realization e)
        {
            ApplicationUser currentUser = await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var status = "none";
            try { 
            if (e.RealizationID > 0)
            {
                var v = db.Realizations.Where(a => a.RealizationID == e.RealizationID).FirstOrDefault();
                if (v != null)
                {
                    v.EmployeeID = currentUser.EmployeeID;
                    v.Subject = e.Subject;
                    v.Start = e.Start;
                    v.Description = e.Description;
                    v.Hours = e.Hours;
                    v.ProjectID = e.ProjectID;
                    v.RealizationTypeID = e.RealizationTypeID;
                    v.DepartmentID = e.DepartmentID;
                    await db.SaveChangesAsync();

                    }
                status = "edit";
            }
            else
            {
                Realization ev = new Realization();
                ev.Description = e.Description;
                ev.EmployeeID = currentUser.EmployeeID;
                ev.Start = e.Start;
                ev.Subject = e.Subject;
                ev.ProjectID = e.ProjectID;
                ev.RealizationTypeID = e.RealizationTypeID;
                ev.DepartmentID = e.DepartmentID;
                ev.Hours = e.Hours;
                ev.Employee = currentUser.Employee;
                UpdateEmployee(ev.EmployeeID, ev.ProjectID);
                db.Realizations.Add(ev);
                await db.SaveChangesAsync();
                status = "save";
            }
            }
            catch
            {
                
            }

            return new JsonResult { Data = new { status } };
        }

        private void UpdateEmployee(int? employeeID,int? projectID)
        {
            var empProjects = new EmployeesProject();
            empProjects.EmployeeID = employeeID;
            empProjects.ProjectID = projectID;
            var nesto= db.EmployeesProjects.DistinctBy(m => new { m.EmployeeID, m.ProjectID }).ToList();
            if (!nesto.Any(x=>x.ProjectID == projectID && x.EmployeeID == employeeID))
            {
                db.EmployeesProjects.Add(empProjects);
            }

        }


        [HttpPost]
        public async Task<JsonResult> DeleteEvent(int RealizationID)
        {
            var status = false;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                 var v = db.Realizations.Where(a => a.RealizationID == RealizationID).FirstOrDefault();
                 if (v != null)
                 {
                        var projectId = v.ProjectID;
                        var employeeId = v.EmployeeID;
                        db.Realizations.Remove(v);
                        await db.SaveChangesAsync();
                        if (!db.Realizations.DistinctBy(x => new { x.EmployeeID, x.ProjectID })
                        .Any(x => x.ProjectID == projectId && x.EmployeeID == employeeId))
                        {
                         await DeleteEmployeeProjects(projectId, employeeId);    
                        }
                        dbContextTransaction.Commit();
                 }
                status = true;
            }

            return new JsonResult { Data = new { status } };
        }
        public async Task DeleteEmployeeProjects(int? projectId,int? employeeId)
        {
            var deleteEmpPro = db.EmployeesProjects.Where(x => x.ProjectID == projectId && x.EmployeeID == employeeId)
                       .FirstOrDefault();
            db.EmployeesProjects.Remove(deleteEmpPro);
            await db.SaveChangesAsync();
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
