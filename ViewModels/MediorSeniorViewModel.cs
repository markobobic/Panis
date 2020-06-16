using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panis.ViewModels
{
    public class MediorSeniorViewModel
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Photo { get; set; }
        public string PhotoType { get; set; }
        public string City { get; set; }
    }
}