using Panis.Extensions;
using Panis.Interfaces;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Panis.Services
{
    public class RealizationRepo : IDisposable, IRealization
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public async Task<Realization> GetLatestRealization(int employeeID)
        {
           return await db.Realizations.Where(x => x.EmployeeID == employeeID).OrderByDescending(x => x.RealizationID).FirstOrDefaultAsync();
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

        public void PopulateCalendarWhenEmployeeAbsent(DateTime start, DateTime end,int employeeID)
        {
            List<Realization> realizations = new List<Realization>();
            foreach (DateTime day in ExtensionClass.EachDay(start, end))
            {
                Realization realization = new Realization();
                realization.Hours = 8;
                realization.ProjectID = 1;
                realization.RealizationTypeID = 1;
                realization.Subject = "Absence";
                realization.Description = "Absence";
                realization.EmployeeID = employeeID;
                realization.Start = day;
                realization.DepartmentID = 1;
                realizations.Add(realization);
            }
            db.Realizations.AddRange(realizations);
            db.SaveChanges();
        }
    }
}