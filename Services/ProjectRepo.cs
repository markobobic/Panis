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
    public class ProjectRepo : IDisposable, IProject
    {
        ApplicationDbContext db = new ApplicationDbContext();
        

        public async Task<Project> GetLatestProject(int? latestProjectID)
        {
           return await  db.Projects.Where(x => x.ProjectID == latestProjectID).FirstOrDefaultAsync();
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