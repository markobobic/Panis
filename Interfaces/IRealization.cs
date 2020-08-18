using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panis.Interfaces
{
   public interface IRealization
    {
        Task<Realization> GetLatestRealization(int employeeID);
        void PopulateCalendarWhenEmployeeAbsent(DateTime start, DateTime end,int employeeID);
    }
}
