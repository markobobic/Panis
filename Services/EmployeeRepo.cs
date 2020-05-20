using Panis.Interfaces;
using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Panis.Services
{
    public class EmployeeRepo : IDisposable, IEmployee
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Insert(Employee emp)
        {
            db.Employees.Add(emp);
        }

        public Employee MapData(EmployeeViewModel employee, HttpPostedFileBase image)
        {
            Employee employeeSave = new Employee();
            if (image != null)
            {
                employeeSave.PhotoType = image.ContentType;
                employeeSave.Photo = new byte[image.ContentLength];
                image.InputStream.Read(employeeSave.Photo, 0, image.ContentLength);
            }
            employeeSave.FirstName = employee.FirstName;
            employeeSave.LastName = employee.LastName;
            employeeSave.Education = employee.Education;
            employeeSave.TeamLeadID = employee.TeamLeadID;
            employeeSave.SectorID = employee.SectorID;
            employeeSave.ClientSectorID = employee.ClientSectorID;
            employeeSave.CityFromID = employee.CityFromID;
            employeeSave.StreetFromID = employee.StreetFromID;
            employeeSave.StreetNumberFromID = employee.StreetNumberFromID;
            employeeSave.AppartmentNumberFromID = employee.AppartmentNumberFromID;
            employeeSave.LivingCity = employee.LivingCity;
            employeeSave.LivingStreet = employee.LivingStreet;
            employeeSave.LivingStreetNumber = employee.LivingStreetNumber;
            employeeSave.Mobile = employee.Mobile;
            employeeSave.IsTeamLead = employee.IsTeamLead;
            return employeeSave;
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
        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
    }
}