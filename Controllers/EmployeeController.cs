using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Panis.DataTableModel;
using Panis.Interfaces;
using Panis.Models;
using Panis.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Z.EntityFramework.Plus;
using static Panis.DataTableModel.JsonClasses;
using static Panis.Models.ApplicationUser;
using static Panis.Models.EmployeeEnrollment;

namespace Panis.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        IUserRepo _dbUser;
        INotification _dbNotify;
        ITeamLead _dbTeamLeads;
        IEmployee _dbEmployees;
        IEmployeeEnrollments _dbEmployeeEnrollments;
        public EmployeeController(IUserRepo dbUser, INotification dbNotify, 
            ITeamLead dbTeamLeads, IEmployee dbEmployees,IEmployeeEnrollments dbEEmployeeEnrollments)
        {
            _dbUser = dbUser;
            _dbNotify = dbNotify;
            _dbTeamLeads = dbTeamLeads;
            _dbEmployees=dbEmployees;
            _dbEmployeeEnrollments = dbEEmployeeEnrollments;

        }

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
            //get team leads for get create view

            ViewBag.TeamLeadID = await _dbTeamLeads.GetAllTeamLeadersForDropDown("add",0);

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
                {   //mapping data
                    var employeeToSave =_dbEmployees.MapData(employee,image);
                    db.Employees.Add(employeeToSave);
                    try
                    {
                        await db.SaveChangesAsync();                   }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                    if (employee.IsActive == true)
                    {
                        await AddUser(employee, employeeToSave.EmployeeID);
                        using (var dbContextTransaction = db.Database.BeginTransaction())
                        {
                            if (employee.IsTeamLead == true)
                            {
                                await AddTeamLead(employeeToSave);
                            }
                            if (employee.TeamLeadID > 0 || employee.TeamLeadID != null)
                            {
                                await AsignTeamLeaderNotify(employeeToSave);
                            }
                            if (employee.OfficialWorkStart != null)
                            {
                                await AddEnnrolemnt(employee, employeeToSave.EmployeeID);
                            }

                            dbContextTransaction.Commit();
                        }
                    }
                    return RedirectToAction("Index");

                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var errors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in errors.ValidationErrors)
                    {
                         // get the error message 
                        string errorMessage = validationError.ErrorMessage;
                    }
                }
            }
            ViewBag.TeamLeadID = await _dbTeamLeads.GetAllTeamLeadersForDropDown("add",0);
            return View(employee);
        }

        private async Task AsignTeamLeaderNotify(Employee emp)
        {
            //picking selected team lead from newly employee because we want to notify him 
            //that the new employee is in his team
            var pickedTeamLead = _dbTeamLeads.GetSelectedTeamLeadForEmployee(emp.TeamLeadID);
            var pickedTeamLeadEmployeeID = (int)pickedTeamLead.EmployeeID;         
            Notification notifyTeamLeader = new Notification();
            notifyTeamLeader.Message = $"You({pickedTeamLead.FullName}) became a team leader to your colleague {emp.FirstName} {emp.LastName}.";
            notifyTeamLeader.EmployeeID = pickedTeamLead.EmployeeID;
            var userTeamLead = await _dbUser.GetUserByEmployeeID(pickedTeamLeadEmployeeID);
            userTeamLead.CountNotifications = userTeamLead.CountNotifications + 1;
            userTeamLead.ReadNotifications = false;
            // then we are notifying employee which team lead is asigned to him 
            Notification notifyEmployee = new Notification();
            notifyEmployee.Message = $"You({emp.FirstName} {emp.LastName}) have been asigned to team leader {pickedTeamLead.FullName}.";
            notifyEmployee.EmployeeID = emp.EmployeeID;
            var user = await _dbUser.GetUserByEmployeeID(emp.EmployeeID);
            if (user.CountNotifications == null)
            {
                user.CountNotifications = 0;
            }
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            //here we are updating,adding and saving all changes and notifying hr department
            _dbUser.Update(userTeamLead);
            _dbUser.Update(user);
            db.Notifications.Add(notifyTeamLeader);
            db.Notifications.Add(notifyEmployee);
            await _dbUser.UpdateAllWithIncrementedNotification(WorkPosition.HR, 2);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
           
        }
        private async Task AddTeamLead(Employee emp)
        {
            //mapping data and creating notification for newly become team leader
            //notification goes to hr department aswell
            var teamLead =_dbTeamLeads.MapData(emp);
            var notify = _dbNotify.MapData($"You({emp.FirstName} {emp.LastName}) have became a team leader.", emp.EmployeeID);
            var user = await _dbUser.GetUserByEmployeeID(emp.EmployeeID);
            if (user.CountNotifications == null)
            {
                user.CountNotifications = 0;
            }
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            _dbUser.Update(user);
            await _dbUser.UpdateAllWithIncrementedNotification(WorkPosition.HR, 1);
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
            if (ModelState.IsValid)
            {
                //mapping data
                var employeeEnrollment =_dbEmployeeEnrollments.MapData(emp, id);
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

        }
        private async Task AddUser(EmployeeViewModel model, int empId)
        {
            if (ModelState.IsValid)
            {
                //adding user
                var user = new ApplicationUser
                { UserName = model.UserName, Email = model.Email, EmployeeID = empId,
                    EmpPosition= (WorkPosition)model.EmployeePositions,CountNotifications=0};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRole);
                }
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
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
                 SectorID = emp.SectorID,
                 IsTeamLead = emp.IsTeamLead,
                 Positions =(EmployeeViewModel.Level)enrollments.Seniority,
                 EmployeePositions = (EmployeeViewModel.Position)users.EmpPosition
             }).FirstOrDefaultAsync();
             ViewBag.TeamLeadID = _dbTeamLeads.GetAllTeamLeadersForDropDown("edit", viewModel.TeamLeadID);

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
            ViewBag.TeamLeadID = _dbTeamLeads.GetAllTeamLeadersForDropDown("edit", employee.TeamLeadID);
            return View(employee);
        }

        private async Task EditPreviousTeamLeadNotification(int? teamLeadID,string firstName,string lastName)
        {
            var notify = new Notification();
            var employeeID= db.TeamLeads.Find(teamLeadID).EmployeeID;
            var teamLeadName = db.Employees.Find(employeeID);
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            notify.Message = $"You({teamLeadName.FirstName} {teamLeadName.LastName}) are no longer team leader to {firstName} {lastName}";
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            db.Users.Where(x => x.EmpPosition == WorkPosition.HR)
                   .Update(x => new ApplicationUser { ReadNotifications = false, CountNotifications = x.CountNotifications + 1 });
            await db.SaveChangesAsync();

        }

        private async Task EditTeamLeadEmployeeNotification(string firstName, string lastName, int employeeID)
        {
            Notification notify = new Notification();
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            var employeeName = await db.Employees.Where(x => x.EmployeeID == employeeID).SingleAsync();
            notify.Message = ($"You({employeeName.FirstName} {employeeName.LastName}) became a team leader to your colleague {firstName} {lastName}.");
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            db.Users.Where(x => x.EmpPosition == WorkPosition.HR)
                   .Update(x => new ApplicationUser { ReadNotifications = false, CountNotifications = x.CountNotifications + 1 });
            await db.SaveChangesAsync();
        }

        private async Task EditEmployeeTeamLeadNotification(string fullName, int? employeeID)
        {
            Notification notify = new Notification();
            notify.EmployeeID = employeeID;
            var user = await db.Users.Where(x => x.EmployeeID == employeeID).SingleAsync();
            var employeeName = await db.Employees.Where(x => x.EmployeeID == employeeID).SingleAsync();
            notify.Message = ($"You({employeeName.FirstName} {employeeName.LastName}) have been assigned a new team leader {fullName}.");
            user.CountNotifications = user.CountNotifications + 1;
            user.ReadNotifications = false;
            db.Notifications.Add(notify);
            db.Users.Where(x => x.EmpPosition == WorkPosition.HR)
                   .Update(x => new ApplicationUser { ReadNotifications = false, CountNotifications = x.CountNotifications + 1 });
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
                data = myBag.OrderBy(x=>x.FirstName)
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
               .Where(z => db.Employees.OrderBy(x => x.EmployeeID).Select(x => x.EmployeeID).Skip(skip).Take(take).Contains(z.EmployeeID)).ToList();
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


       