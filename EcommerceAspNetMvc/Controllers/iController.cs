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

        public ActionResult Index(int? id)
        {
            IQueryable<Products> product = _context.Products;
            Categories category = null;

            if (id.HasValue)
            {
                product = product.Where(x => x.Category_Id == id);
                category = _context.Categories?.FirstOrDefault(x => x.Id == id);
            }


            var model = new HomeViewModel
            {
                Products = product.ToList(),
                Category = category
            };

            List<Categories> categories = new List<Categories>();
            if (model.Category != null)
            {
                categories.Add(model.Category);
                var parentcat = model.Category.Categories2;
                while (parentcat != null)
                {
                    categories.Add(parentcat);
                    parentcat = parentcat.Categories2;
                }
            }

            TempData["cat"] = categories;
            return View(model);
        }
    }
}