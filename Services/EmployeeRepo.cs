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

        public async Task<Employee> GetEmployeeByID(int? employeeID)
        {
            return await db.Employees.Where(x => x.EmployeeID == employeeID).AsNoTracking().SingleOrDefaultAsync();
        }
        public void Update(Employee emp)
        {
            db.Employees.Attach(emp);
            db.Entry(emp).State = EntityState.Modified;
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

        public async Task<Employee> MapDataUpdateProfile(int existingEmployeeID, Employee newEmployeeData, HttpPostedFileBase newImage)
        {
            var employee = await GetEmployeeByID(existingEmployeeID);

            if (newImage != null)
            {
                employee.PhotoType = newImage.ContentType;
                employee.Photo = new byte[newImage.ContentLength];
                newImage.InputStream.Read(employee.Photo, 0, newImage.ContentLength);
            }

            employee.Education = newEmployeeData.Education;
            employee.LivingCity = newEmployeeData.LivingCity;
            employee.LivingStreet = newEmployeeData.LivingStreet;
            employee.LivingStreetNumber = newEmployeeData.LivingStreetNumber;
            employee.ReportsTo = newEmployeeData.ReportsTo;
            employee.Mobile = newEmployeeData.Mobile;

            return employee;
        }

        public async Task<EmployeeViewModel> MapDataEmployeeViewModel(int? employeeID)
        {
           return await
           (from emp in db.Employees
            join teamLead in db.TeamLeads on emp.TeamLeadID equals teamLead.TeamLeadID
            join users in db.Users on emp.EmployeeID equals users.EmployeeID
            join enrollments in db.employeeEnrollments on emp.EmployeeID equals enrollments.EmployeeID
            where emp.EmployeeID == employeeID
            select new EmployeeViewModel
            {
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                CityFromID = emp.CityFromID,
                StreetFromID = emp.CityFromID,
                StreetNumberFromID = emp.StreetNumberFromID,
                AppartmentNumberFromID = emp.AppartmentNumberFromID,
                LivingCity = emp.LivingCity,
                LivingStreet = emp.LivingStreet,
                LivingStreetNumber = emp.LivingStreetNumber,
                Education = emp.Education,
                OfficialWorkStart = enrollments.OfficialWorkStart,
                Mobile = emp.Mobile,
                WorkStart = enrollments.WorkStart,
                IsActive = users.IsActive,
                UserName = users.UserName,
                Email = users.Email,
                ReportsTo = emp.ReportsTo,
                Password = users.PasswordHash,
                EmployeeID = employeeID,
                TeamLeadID = emp.TeamLeadID,
                SectorID = emp.SectorID,
                IsTeamLead = emp.IsTeamLead,
                Positions = (EmployeeViewModel.Level)enrollments.Seniority,
                EmployeePositions = (EmployeeViewModel.Position)users.EmpPosition
            }).FirstOrDefaultAsync();

           
        }


        public Employee MapDataEdit(Employee employeeSave, EmployeeViewModel employee, HttpPostedFileBase image)
        {
            if (image != null)
            {
                employeeSave.PhotoType = image.ContentType;
                employeeSave.Photo = new byte[image.ContentLength];
                image.InputStream.Read(employeeSave.Photo, 0, image.ContentLength);
            }
            employeeSave.FirstName = employee.FirstName;
            employeeSave.LastName = employee.LastName;
            employeeSave.Education = employee.Education;
            employeeSave.SectorID = employee.SectorID;
            employeeSave.ClientSectorID = employee.ClientSectorID;
            employeeSave.LivingCity = employee.LivingCity;
            employeeSave.LivingStreet = employee.LivingStreet;
            employeeSave.LivingStreetNumber = employee.LivingStreetNumber;
            employeeSave.CityFromID = employee.CityFromID;
            employeeSave.StreetFromID = employee.StreetFromID;
            employeeSave.StreetNumberFromID = employee.StreetNumberFromID;
            employeeSave.AppartmentNumberFromID = employee.AppartmentNumberFromID;
            employeeSave.Mobile = employee.Mobile;
            if (employee.TeamLeadID != employeeSave.TeamLeadID)
            {
                var previousTeamLeadID = employeeSave.TeamLeadID;
                employeeSave.TeamLeadID = employee.TeamLeadID;
            }

            return employeeSave;
            }

        public async Task<List<Employee>> GetAllTeamLeads()
        {
            return await db.Employees.Where(x => x.IsTeamLead == true).ToListAsync();
        }
       public async Task<List<MediorSeniorViewModel>> GetAllMediorSenior()
        {
            var mediorSenior = (from emp in db.Employees
                         join empLevel in db.employeeEnrollments on emp.EmployeeID equals empLevel.EmployeeID
                         where emp.IsTeamLead == false
                         select new MediorSeniorViewModel
                         {
                             EmployeeID = emp.EmployeeID,
                             Photo = emp.Photo,
                             FirstName = emp.FirstName,
                             LastName = emp.LastName,
                             City = emp.CityFromID
                         }).ToListAsync();
            return await mediorSenior;
        }
    }
}