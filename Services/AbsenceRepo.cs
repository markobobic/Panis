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
    public class AbsenceRepo : IAbsence,IDisposable
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void Add(Absence absence)
        {
            db.Absences.Add(absence);
        }

        public async Task DeleteAsync(int id)
        {
            var absenceToRemove = await GetByIdAsync(id);
            if (absenceToRemove != null)
            {
                db.Absences.Attach(absenceToRemove);
                db.Absences.Remove(absenceToRemove);
            }
           
        }

        public IEnumerable<Absence> GetAll()
        {
            return db.Absences.AsNoTracking().ToList();
        }

        public async Task<Absence> GetByIdAsync(int? id)
        {
           return await db.Absences.AsNoTracking().SingleOrDefaultAsync(x=>x.AbsenceID==id);
        }

        public  dynamic GetCalendarData(int EmployeeID)
        {
            return db.Absences.AsNoTracking().Where(x => x.EmployeeID == EmployeeID).Select(
                x => new
                {
                    AbsenceID = x.AbsenceID,
                    Start = x.Start,
                    End = x.End,
                    ApplicationDate = x.ApplicationDate,
                    Description = x.Description,
                    AbsenceTypeID = x.AbsenceTypeID,
                    Approved = x.Approved
                }

                ).ToList();
        }

        public async Task<Absence> UpdateMapDataAsync(Absence absence,int employeeID)
        {
            var absenceToSave = await GetByIdAsync(absence.AbsenceID);
            absenceToSave.EmployeeID = employeeID;
            absenceToSave.Start = absence.Start;
            absenceToSave.Description = absence.Description;
            absenceToSave.End = absence.End;
            absenceToSave.ApplicationDate = DateTime.Now;
            return absenceToSave;
        }

        public void Update(Absence absence)
        {
            db.Absences.Attach(absence);
            db.Entry(absence).State = EntityState.Modified;
        }

        public Absence MapData(Absence absence,int EmployeeID)
        {
            Absence absenceToSave = new Absence();
            absenceToSave.Description = absence.Description;
            absenceToSave.EmployeeID = EmployeeID;
            absenceToSave.Start = absence.Start;
            absenceToSave.Employee = db.Employees.Find(EmployeeID);
            absenceToSave.End = absence.End;
            absenceToSave.AbsenceTypeID = absence.AbsenceTypeID;
            absenceToSave.ApplicationDate = DateTime.Now;
            absenceToSave.Approved = false;
            return absenceToSave;
        }

        public async Task SaveChangesAsync()
        {
           await db.SaveChangesAsync();
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


        public IQueryable<AbsenceAndTypeViewModel> GetRequestAbsencesAndTypes(int employeeID)
        {
           return db.Absences.Join(db.AbsenceTypes,
               absence => absence.AbsenceTypeID,
               absenceType => absenceType.AbsenceTypeID,
               (absence, absenceType) => new AbsenceAndTypeViewModel { AbsenceName = absence, AbsenceType = absenceType })
               .Where(x => x.AbsenceName.EmployeeID == employeeID && x.AbsenceName.Approved == false);
        }
    }
}