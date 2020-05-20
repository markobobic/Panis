using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panis.Interfaces
{
    public interface IEmployeeEnrollments
    {
        EmployeeEnrollment MapData(EmployeeViewModel emp,int employeeID);

    }
}
