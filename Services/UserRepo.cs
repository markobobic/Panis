using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.Interfaces;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Z.EntityFramework.Plus;
using static Panis.Models.ApplicationUser;

namespace Panis.Services
{
    public class UserRepo : IUserRepo,IDisposable
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ApplicationUser> GetCurrentUser()
        {
            return  await System.Web.HttpContext.Current.GetOwinContext().
            GetUserManager<ApplicationUserManager>().FindByIdAsync(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        public async Task UpdateAllWithIncrementedNotification(WorkPosition workPosition,int numberOfNotification)
        {
              await db.Users.Where(x => x.EmpPosition == workPosition)
              .UpdateAsync(x => new ApplicationUser { ReadNotifications = false, CountNotifications = x.CountNotifications + numberOfNotification });
        }

        public void Update(ApplicationUser user)
        {
            db.Users.Attach(user);
            db.Entry(user).State = EntityState.Modified;
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

        public async Task<ApplicationUser> GetUserByEmployeeID(int employeeID)
        {
           return await db.Users.Where(x=>x.EmployeeID==employeeID).AsNoTracking().SingleOrDefaultAsync();
        }
    }
}