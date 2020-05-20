using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panis.Interfaces
{
  public interface IAbsence
    {
        IEnumerable<Absence> GetAll();
        Task<Absence> GetByIdAsync(int? id);
        void Add(Absence absence);
        void Update(Absence absence);
        Task DeleteAsync(int id);
        dynamic GetCalendarData(int EmployeeID);
        Absence MapData(Absence absence, int EmployeeID);
        Task<Absence> UpdateMapDataAsync(Absence absence,int EmployeeID);
        Task SaveChangesAsync();
    }
}
