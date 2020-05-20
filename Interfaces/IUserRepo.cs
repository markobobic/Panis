using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Panis.Models.ApplicationUser;

namespace Panis.Interfaces
{
   public interface IUserRepo
    {
       Task<ApplicationUser>  GetCurrentUser();
       Task UpdateAllWithIncrementedNotification(WorkPosition workPosition, int numberOfNotification);
       Task<ApplicationUser> GetUserByEmployeeID(int employeeID);
       void Update(ApplicationUser user);

    }
}
