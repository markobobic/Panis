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

        Task<Employee> GetEmployeeByID(int? id);
        void Update(Employee employee);

        Task<Employee> MapDataUpdateProfile(int existingEmployeeID, Employee newEmployeeData, HttpPostedFileBase newImage);

        Task<EmployeeViewModel> MapDataEmployeeViewModel(int? employeeID);

        Employee MapDataEdit(Employee employeeSave, EmployeeViewModel employee, HttpPostedFileBase image);

        Task<List<Employee>> GetAllTeamLeads();
        Task<List<MediorSeniorViewModel>> GetAllMediorSenior();

    }
}
