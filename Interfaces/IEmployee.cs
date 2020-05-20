using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Panis.Interfaces
{
 public interface IEmployee
    {
        void Insert(Employee emp);
        Employee MapData(EmployeeViewModel employee, HttpPostedFileBase image);
        Task SaveChangesAsync();


    }
}
