using Panis.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panis.Interfaces
{
  public  interface INotification
    {
        void Add(Notification notify);
        Task SaveChangesAsync();
        Notification MapData(string messege, int? employeeID);
    }
}
