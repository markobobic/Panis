using Panis.Interfaces;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Panis.Services
{
    public class AbsenceTypeRepo : IAbsenceTypeRepo,IDisposable
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public SelectList GetAllTypesDropDown()
        {
            return new SelectList(db.AbsenceTypes.ToList(), "AbsenceTypeID", "Name");
        }

        public async Task<AbsenceType> GetById(short? id)
        {
            return await db.AbsenceTypes.AsNoTracking().SingleOrDefaultAsync(x => x.AbsenceTypeID == id);
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

    }
}