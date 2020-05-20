using Panis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panis.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            db = new ApplicationDbContext();
        }

        protected ApplicationDbContext db { get; set; }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}