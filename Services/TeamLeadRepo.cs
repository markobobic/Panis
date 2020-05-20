using Panis.Interfaces;
using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Panis.Services
{
    public class TeamLeadRepo : ITeamLead,IDisposable
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public TeamLead GetById(int? id)
        {
            return  db.TeamLeads.Find(id);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<SelectList> GetAllTeamLeadersForDropDown(string type,int? teamLeadID)
        {
            
            var teamLeads =
            (from employee in db.Employees
            join teamLead in db.TeamLeads on employee.EmployeeID equals teamLead.EmployeeID
            select new { FullName = employee.FirstName + " " + employee.LastName, TeamLeadID = teamLead.TeamLeadID }).AsNoTracking();
            if (type == "add")
            {
                return new SelectList(await teamLeads.ToListAsync(), "TeamLeadID", "FullName");
            } else if (type == "edit")
            {
               return new SelectList(teamLeads.ToList(), "TeamLeadID", "FullName", teamLeadID);
            }
            return null;
        }

        public dynamic GetSelectedTeamLeadForEmployee(int? teamLeaadID)
        {
           return (from employees in db.Employees
             join teamLead in db.TeamLeads on employees.EmployeeID equals teamLead.EmployeeID
             where teamLead.TeamLeadID == teamLeaadID
                   select new
             {
                 FullName = employees.FirstName + " " + employees.LastName,
                 EmployeeID = teamLead.EmployeeID
             }).AsNoTracking().FirstOrDefault();
        }

        public TeamLead MapData(Employee emp)
        {
            TeamLead teamLead = new TeamLead();
            teamLead.EmployeeID = emp.EmployeeID;
            teamLead.Employees.Add(emp);
            return teamLead;
        }
    }
}