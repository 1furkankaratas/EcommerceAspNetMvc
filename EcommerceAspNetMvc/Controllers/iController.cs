using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;

namespace EcommerceAspNetMvc.Controllers
{
    public class iController : BaseController
    {
        

        public ActionResult Index(int? id)
        {
            IQueryable<Products> product = Context.Products;
            Categories category = null;

            if (id.HasValue)
            {
                product = product.Where(x => x.Category_Id == id);
                category = Context.Categories?.FirstOrDefault(x => x.Id == id);
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