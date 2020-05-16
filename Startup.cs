using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Panis.Models;

[assembly: OwinStartupAttribute(typeof(Panis.Startup))]
namespace Panis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //CreateRolesAndUsers();
        }

        //private void CreateRolesAndUsers()
        //{
        //    ApplicationDbContext context = new ApplicationDbContext();

        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        //    if (!roleManager.RoleExists(RoleName.Admin))
        //    {
        //        var role = new IdentityRole();
        //        role.Name = RoleName.Admin;
        //        roleManager.Create(role);

        //        var user = new ApplicationUser();
        //        user.UserName = "marko";
        //        user.Email = "bobic015@gmail.com";
        //        string userPWD = "Marko.123";
        //        var chkUser = userManager.Create(user, userPWD);
        //        if (chkUser.Succeeded)
        //        { var result1 = userManager.AddToRole(user.Id, RoleName.Admin); }
        //    }
        //    else
        //    {
        //        var regularUser = new IdentityRole();
        //        regularUser.Name = RoleName.User;
        //        roleManager.Create(regularUser);
        //        var advancedUser = new IdentityRole();
        //        regularUser.Name = RoleName.AdvancedUser;
        //        roleManager.Create(advancedUser);
        //    }
        //}

    }
}

