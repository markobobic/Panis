using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Panis.Interfaces
{
    public interface ITeamLead
    {
        TeamLead GetById(int? id);
        Task<SelectList> GetAllTeamLeadersForDropDown(string type,int? teamLeadID);
        dynamic GetSelectedTeamLeadForEmployee(int? teamLeaadID);
        TeamLead MapData(Employee emp);
    }
}
