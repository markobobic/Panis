using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Panis.Interfaces
{
   public interface IAbsenceTypeRepo
    {
        Task<AbsenceType> GetById(short? id);
        SelectList GetAllTypesDropDown();
    }
}
