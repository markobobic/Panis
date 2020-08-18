using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panis.Interfaces
{
  public  interface IProject
    {
        Task<Project> GetLatestProject(int? latestProjectID);
    }
}
