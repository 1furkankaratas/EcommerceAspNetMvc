using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Controllers
{
    public class BaseController : Controller
    {
        protected EcommerceDbEntities Context { get; private set; }
        public BaseController()
        {
            Context = new EcommerceDbEntities();
            ViewBag.MenuCategories = Context.Categories.Where(x => x.Parent_Id == null).ToList();
        }
    }
}