using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panis.Controllers
{
    public class RatingsController : BaseController
    {
        
        public ActionResult Index()
        {

            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Session["EmployeeID"] = currentUser.EmployeeID;
            var currentEmployee = db.Employees.Find(currentUser.EmployeeID);
            if (currentEmployee.IsTeamLead == true) {
                var employeesUnderTeamLead = db.Employees.Where(x => x.TeamLeadID == currentEmployee.TeamLeadID && x.IsTeamLead == false);
                return View(employeesUnderTeamLead);
            }
            return View();
        }


        public ActionResult RateProduct(int id, int rate)
        {
            
            bool success = false;
            string error = "";
            var avgRating = RegisterProductVote(id, rate);
            success = true;
            return Json(new { error = error, success = success, pid = id, avgRating=avgRating }, JsonRequestBehavior.AllowGet);
        }

        private double RegisterProductVote( int employeeID, int vote)
        {
            var employee = db.Employees.Where(x => x.EmployeeID == employeeID).SingleOrDefault();
            if (employee.Rating != 0) {  
            var rating= (employee.Rating + vote) / 2;
            employee.Rating = Math.Round(rating, 2);
            }
            else
            {
                employee.Rating = vote;
            }
            db.SaveChanges();
            return employee.Rating;
        }
    }
}