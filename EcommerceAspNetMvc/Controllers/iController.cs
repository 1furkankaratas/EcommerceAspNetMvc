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

        [HttpGet]
        public ActionResult Product(int id)
        {
            var product = Context.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return RedirectToAction("Index", "i");
            }
            ProductViewModel model = new ProductViewModel()
            {
                Product = product,
                Comments = product.Comments.ToList()
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult Product(ProductViewModel model)
        {
            try
            {
                Comments com = new Comments
                {
                    Product_Id = model.Product.Id,
                    Text = model.Comment.Text,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Member_Id = CurrentUserId()
                };
                Context.Comments.Add(com);
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.info = "Yorum eklenirken bir hata meydana geldi" + " -> " + e.Message;

            }
            return RedirectToAction("Product", "i");
        }
    }
}