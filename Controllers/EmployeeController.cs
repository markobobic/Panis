using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.DataTableModel;
using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using static Panis.DataTableModel.JsonClasses;
using static Panis.Models.ApplicationUser;
using static Panis.Models.EmployeeEnrollment;

namespace Panis.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        #region Create
        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> Create()
        {

            var teamLeads =
            from employee in db.Employees
            join teamLead in db.TeamLeads on employee.EmployeeID equals teamLead.EmployeeID
            select new { FullName = employee.FirstName + " " + employee.LastName, TeamLeadID = teamLead.TeamLeadID };
            ViewBag.TeamLeadID = new SelectList(await teamLeads.ToListAsync(), "TeamLeadID", "FullName");

            //ViewBag.SectorID = new SelectList(db.Sectors.ToList(), "SectorID", "Name");

            return View();
        }
        [Authorize(Roles = RoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeViewModel employee, HttpPostedFileBase image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Employee employeeSave = new Employee();
                    if (image != null)
                    {
                        employeeSave.PhotoType = image.ContentType;
                        employeeSave.Photo = new byte[image.ContentLength];
                        image.InputStream.Read(employeeSave.Photo, 0, image.ContentLength);
                    }
                    employeeSave.FirstName = employee.FirstName;
                    employeeSave.LastName = employee.LastName;
                    employeeSave.Education = employee.Education;
                    employeeSave.TeamLeadID = employee.TeamLeadID;
                    employeeSave.SectorID = employee.SectorID;
                    employeeSave.ClientSectorID = employee.ClientSectorID;
                    employeeSave.CityFromID = employee.CityFromID;
                    employeeSave.StreetFromID = employee.StreetFromID;
                    employeeSave.StreetNumberFromID = employee.StreetNumberFromID;
                    employeeSave.AppartmentNumberFromID = employee.AppartmentNumberFromID;
                    employeeSave.LivingCity = employee.LivingCity;
                    employeeSave.LivingStreet = employee.LivingStreet;
                    employeeSave.LivingStreetNumber = employee.LivingStreetNumber;
                    employeeSave.Mobile = employee.Mobile;
                    db.Employees.Add(employeeSave);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                    if (employee.IsActive == true)
                    {
                        {
                            await AddUser(employee, employeeSave.EmployeeID);
                        };
                    }
                    if (employee.IsTeamLead == true)
                    {
                        await AddTeamLead(employeeSave);
                    }

                    if (employee.OfficialWorkStart != null)
                    {
                        await AddEnnrolemnt(employee, employeeSave.EmployeeID);
                    }
                    return RedirectToAction("Index");

                }
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            var teamLeads =
              from emp in db.Employees
              join teamLead in db.TeamLeads on emp.EmployeeID equals teamLead.EmployeeID
              select new { FullName = emp.FirstName + " " + emp.LastName, TeamLeadID = teamLead.TeamLeadID };
            ViewBag.TeamLeadID = new SelectList(teamLeads.ToList(), "TeamLeadID", "FullName");
            return View(employee);
        }

        private async Task AddTeamLead(Employee emp)
        {
            TeamLead teamLead = new TeamLead();
            teamLead.EmployeeID = emp.EmployeeID;
            teamLead.Employees.Add(emp);
            db.TeamLeads.Add(teamLead);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

        }
        private async Task AddEnnrolemnt(EmployeeViewModel emp, int id)
        {
            var employeeEnrollment = new EmployeeEnrollment();
            employeeEnrollment.EmployeeID = id;
            employeeEnrollment.Seniority = (Level)emp.Positions;
            employeeEnrollment.OfficialWorkStart = emp.OfficialWorkStart;
            employeeEnrollment.WorkStart = emp.WorkStart;
            db.employeeEnrollments.Add(employeeEnrollment);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

        }
        private async Task AddUser(EmployeeViewModel model, int empId)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmployeeID = empId, EmpPosition= (WorkPosition)model.EmployeePositions };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRole);
                }
                await db.SaveChangesAsync();
            }
        }

        public JsonResult GetSectors()
        {
            var sectors = db.Sectors.Select(x => new { SectorID = x.SectorID, Name = x.Name });
             return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientSector(int SectorID)
        {
              var products = db.ClientSectors.Where(x => x.SectorID == SectorID).Select(x => new { ClientSectorID = x.ClientSectorID, Name = x.Name }).ToList();
               return Json(products, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpGet]
        public async Task<ActionResult> EditProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string query = "SELECT * FROM Employees WHERE EmployeeID = @p0";
            Employee employee = await db.Employees.SqlQuery(query, id).SingleOrDefaultAsync();
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        [HttpPost]
        public async Task<ActionResult> EditProfile(Employee emp, HttpPostedFileBase image)
        {
            var employee = db.Employees.Find(emp.EmployeeID);
            if (image != null)
            {
                employee.PhotoType = image.ContentType;
                employee.Photo = new byte[image.ContentLength];
                image.InputStream.Read(employee.Photo, 0, image.ContentLength);
            }

            employee.Education = emp.Education;
            employee.LivingCity = emp.LivingCity;
            employee.LivingStreet = emp.LivingStreet;
            employee.LivingStreetNumber = emp.LivingStreetNumber;
            employee.ReportsTo = emp.ReportsTo;
            employee.Mobile = emp.Mobile;
            db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction("UserDashBoard", "Home");

        }

        

        #region Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var teamLeads =
            from emp in db.Employees
            join teamLead in db.TeamLeads on emp.EmployeeID equals teamLead.EmployeeID
            select new { FullName = emp.FirstName + " " + emp.LastName, TeamLeadID = teamLead.TeamLeadID};

            var viewModel = await
            (from emp in db.Employees
             join teamLead in db.TeamLeads on emp.TeamLeadID equals teamLead.TeamLeadID
             join users in db.Users on emp.EmployeeID equals users.EmployeeID
             join enrollments in db.employeeEnrollments on emp.EmployeeID equals enrollments.EmployeeID
             where emp.EmployeeID == id
             select new EmployeeViewModel
             {
                 FirstName = emp.FirstName,
                 LastName = emp.LastName,
                 CityFromID = emp.CityFromID,
                 StreetFromID = emp.CityFromID,
                 StreetNumberFromID = emp.StreetNumberFromID,
                 AppartmentNumberFromID = emp.AppartmentNumberFromID,
                 LivingCity = emp.LivingCity,
                 LivingStreet = emp.LivingStreet,
                 LivingStreetNumber = emp.LivingStreetNumber,
                 Education = emp.Education,
                 OfficialWorkStart = enrollments.OfficialWorkStart,
                 Mobile = emp.Mobile,
                 WorkStart = enrollments.WorkStart,
                 IsActive = users.IsActive,
                 UserName = users.UserName,
                 Email = users.Email,
                 ReportsTo = emp.ReportsTo,
                 Password = users.PasswordHash,
                 EmployeeID = id,
                 TeamLeadID = emp.TeamLeadID,
                 SectorID = emp.SectorID

             }).FirstOrDefaultAsync();
            ViewBag.TeamLeadID = new SelectList(teamLeads.ToList(), "TeamLeadID", "FullName", viewModel.TeamLeadID);
            Session["CurrentSector"] = viewModel.SectorID;
            return View(viewModel);

        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? EmployeeID, EmployeeViewModel employee, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                if (EmployeeID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    string query = "SELECT * FROM Employees WHERE EmployeeID = @p0";
                    Employee employeeSave = await db.Employees.SqlQuery(query, EmployeeID).SingleOrDefaultAsync();
                    if (image != null)
                    {
                        employeeSave.PhotoType = image.ContentType;
                        employeeSave.Photo = new byte[image.ContentLength];
                        image.InputStream.Read(employeeSave.Photo, 0, image.ContentLength);
                    }
                    employeeSave.FirstName = employee.FirstName;
                    employeeSave.LastName = employee.LastName;
                    employeeSave.Education = employee.Education;
                    //employeeSave.OfficialWorkStart = employee.OfficialWorkStart;
                    //employeeSave.WorkStart = employee.WorkStart;
                    employeeSave.SectorID = employee.SectorID;
                    employeeSave.ClientSectorID = employee.ClientSectorID;
                    employeeSave.LivingCity = employee.LivingCity;
                    employeeSave.LivingStreet = employee.LivingStreet;
                    employeeSave.LivingStreetNumber = employee.LivingStreetNumber;
                    employeeSave.CityFromID = employee.CityFromID;
                    employeeSave.StreetFromID = employee.StreetFromID;
                    employeeSave.StreetNumberFromID = employee.StreetNumberFromID;
                    employeeSave.AppartmentNumberFromID = employee.AppartmentNumberFromID;
                    employeeSave.Mobile = employee.Mobile;
                    if (employee.TeamLeadID != employeeSave.TeamLeadID)
                    {
                        var previousTeamLeadID = employeeSave.TeamLeadID;
                        employeeSave.TeamLeadID = employee.TeamLeadID;
                        var pickedTeamLead =
                           (from employees in db.Employees
                            join teamLead in db.TeamLeads on employees.EmployeeID equals teamLead.EmployeeID
                            where teamLead.TeamLeadID == employeeSave.TeamLeadID
                            select new
                            {
                                FullName = employees.FirstName + " " + employees.LastName,
                                EmployeeID = employees.EmployeeID
                            }).FirstOrDefault();

                        await EditEmployeeTeamLeadNotification(pickedTeamLead.FullName, EmployeeID);
                        await EditTeamLeadEmployeeNotification(employee.FirstName, employee.LastName, pickedTeamLead.EmployeeID);
                        await EditPreviousTeamLeadNotification(previousTeamLeadID, employeeSave.FirstName,employeeSave.LastName);
                    }
                    db.Entry(employeeSave).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                return RedirectToAction("Index");

            }
            var teamLeads =
             from emp in db.Employees
             join teamLead in db.TeamLeads on emp.EmployeeID equals teamLead.EmployeeID
             select new { FullName = emp.FirstName + " " + emp.LastName, TeamLeadID = teamLead.TeamLeadID };
            ViewBag.TeamLeadID = new SelectList(teamLeads.ToList(), "TeamLeadID", "FullName");
            return View(employee);
        }

        private async Task EditPreviousTeamLeadNotification(int? teamLeadID,string firstName,string lastName)
        {
            var notify = new Notification();
            notify.Message = $"You are no longer team leader to {firstName} {lastName}";
            var employeeID= db.TeamLeads.Find(teamLeadID).EmployeeID;
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            await db.SaveChangesAsync();

        }

        private async Task EditTeamLeadEmployeeNotification(string firstName, string lastName, int employeeID)
        {
            Notification notify = new Notification();
            notify.Message = ($"You became a team leader to your colleague {firstName} {lastName}.");
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            await db.SaveChangesAsync();
        }

        private async Task EditEmployeeTeamLeadNotification(string fullName, int? employeeID)
        {
            Notification notify = new Notification();
            notify.Message = ($"{fullName}  is your new team leaad.");
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            await db.SaveChangesAsync();
        }

        

        #endregion

        #region DataTable Logic
        public JsonResult CustomServerSideSearchAction(DataTableAjaxPostModel model)
        {
            // action inside a standard controller
            int filteredResultsCount;
            int totalResultsCount;
            var res = YourCustomSearchFunc(model, out filteredResultsCount, out totalResultsCount);
            ConcurrentBag<EmployeeSearchClass> myBag = new ConcurrentBag<EmployeeSearchClass>();
            Parallel.ForEach(res, (s) =>
                   myBag.Add(new EmployeeSearchClass
                   {
                       Id = s.Id,
                       FirstName = s.FirstName,
                       LastName = s.LastName,
                       Mobile = s.Mobile,
                       ImagePath = s.ProductImage != null ? Convert.ToBase64String(s.ProductImage) : null,
                       PhotoType = s.PhotoType
                   })
                   );

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = myBag
            });
        }


        public IList<EmployeeSearchClass> YourCustomSearchFunc(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }


            var result = GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
            if (result == null)
            {
                return new List<EmployeeSearchClass>();
            }
            return result;
        }
        public List<EmployeeSearchClass> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {

            if (String.IsNullOrEmpty(sortBy))
            {
                sortBy = "Id";
                sortDir = true;
            }
            var list1 = db.Employees
               .Where(z => db.Employees.OrderBy(x => x.EmployeeID).Select(x => x.EmployeeID).Skip(skip).Take(take).Contains(z.EmployeeID));
            var result = list1.AsParallel().Select(m => new EmployeeSearchClass
            {
                Id = m.EmployeeID,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Mobile = m.Mobile,
                ProductImage = m.Photo
            }).ToList();

            totalResultsCount = db.Employees.Count();
            filteredResultsCount = totalResultsCount;
            if (searchBy != null)
            {
                List<EmployeeSearchClass> list = new List<EmployeeSearchClass>();
                if (searchBy.Trim() != "")
                {
                    var splitedSearch = searchBy.Trim().Split();
                    for (int i = 0; i < splitedSearch.Length; i++)
                    {
                        var forSearch = splitedSearch[i];
                        list = db.Employees.Where(x => x.FirstName.StartsWith(forSearch) || x.LastName.StartsWith(forSearch)).Select(m => new EmployeeSearchClass
                        {
                            Id = m.EmployeeID,
                            FirstName = m.FirstName,
                            LastName = m.LastName,
                            Mobile = m.Mobile,
                            ProductImage = m.Photo
                        }).ToList();
                    }
                    return list;
                }
                else { return result; }

            }
            else
            {
                return result;
            }

        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


       