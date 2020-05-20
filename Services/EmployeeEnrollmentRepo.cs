using Panis.Interfaces;
using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Panis.Models.EmployeeEnrollment;

namespace Panis.Services
{
    public class EmployeeEnrollmentRepo : IDisposable,IEmployeeEnrollments
    {
        ApplicationDbContext db = new ApplicationDbContext();

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

        public EmployeeEnrollment MapData(EmployeeViewModel emp, int employeeID)
        {
            var employeeEnrollment = new EmployeeEnrollment();
            employeeEnrollment.EmployeeID = employeeID;
            employeeEnrollment.Seniority = (Level)emp.Positions;
            employeeEnrollment.OfficialWorkStart = emp.OfficialWorkStart;
            employeeEnrollment.WorkStart = emp.WorkStart;
            return employeeEnrollment;
        }
    }
}