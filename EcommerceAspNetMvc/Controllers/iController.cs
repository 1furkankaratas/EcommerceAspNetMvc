using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;

namespace EcommerceAspNetMvc.Controllers
{
    public class iController : Controller
    {
        EcommerceDbEntities _context;

        public iController()
        {
            _context = new EcommerceDbEntities();
        }

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Products = _context.Products.ToList()
            };
            return View(model);
        }
    }
}