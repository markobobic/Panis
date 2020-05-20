using Panis.Interfaces;
using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Panis.Services
{
    public class NotificationRepo : INotification,IDisposable
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Add(Notification notify)
        {
            db.Notifications.Add(notify);
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

        public Notification MapData(string messege, int? employeeID)
        {
            Notification notify = new Notification();
            notify.Message = messege;
            notify.EmployeeID = employeeID;
            return notify;
        }
    }
}