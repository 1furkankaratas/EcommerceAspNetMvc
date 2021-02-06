using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EcommerceAspNetMvc.Controllers
{
    public class HomeController : BaseController
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
                return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Product", "Home");
        }

        [HttpGet]
        public ActionResult AddBasket(int id, bool remove = false)
        {
            List<BasketViewModel> basket = null;
            if (Session["Basket"] == null)
            {
                basket = new List<BasketViewModel>();
            }
            else
            {
                basket = (List<BasketViewModel>)Session["Basket"];
            }

            if (basket.Any(x => x.Product.Id == id))
            {
                var pro = basket.FirstOrDefault(x => x.Product.Id == id);
                if (pro != null)
                {
                    if (remove && pro.Count > 0)
                    {
                        pro.Count -= 1;
                        
                    }
                    else
                    {
                        if (pro.Product.UnitsInStock > pro.Count)
                        {
                            pro.Count += 1;
                        }
                        else
                        {
                            TempData["info"] = "Daha fazla ürün eklenemez";
                        }
                    }
                }

            }
            else
            {
                var pro = Context.Products.FirstOrDefault(x => x.Id == id);
                if (pro != null && pro.IsContinued && pro.UnitsInStock>0)
                {
                    basket.Add(new BasketViewModel()
                    {
                        Count = 1,
                        Product = pro
                    });
                }
                else if (pro != null && !pro.IsContinued)
                {
                    TempData["info"] = "Bu ürünün satışı duruduruldu.";
                }

            }

            basket.RemoveAll(x => x.Count < 1);
            Session["Basket"] = basket;

            return RedirectToAction("Basket", "Home");
        }


        [HttpGet]
        public ActionResult Basket()
        {

            List<BasketViewModel> model = (List<BasketViewModel>)Session["Basket"];

            if (model == null)
            {
                model = new List<BasketViewModel>();
            }



            ViewBag.totalPrice = model.Select(x => x.Product.Price * x.Count).Sum();

            return View(model);
        }


        [HttpGet]
        public ActionResult RemoveBasket(int id=0)
        {
            List<BasketViewModel> basket = (List<BasketViewModel>)Session["Basket"];
            
            if (basket!=null)
            {
                if (id>0)
                {
                    basket.RemoveAll(x => x.Product.Id == id);
                    
                }
                else if(id==0)
                {
                    basket.Clear();
                }
                Session["Basket"] = basket;
            }

            return RedirectToAction("Basket", "Home");

        }


        [HttpGet]
        public ActionResult Buy()
        {

            if (IsLogon())
            {
                BuyViewModel model = new BuyViewModel();
                var user = CurrentUserId();
                model.Addresseses = Context.Addresses.Where(x => x.Member_Id == user).ToList();
                model.BasketView = ((List<BasketViewModel>)Session["Basket"]).ToList();
                return View(model);
            }

            return RedirectToAction("Login","Account");
        }

        [HttpPost]
        public ActionResult Buy(string address)
        {
            if (IsLogon())
            {
                try
                {
                    var basket= ((List<BasketViewModel>)Session["Basket"]).ToList();

                    var _address = Context.Addresses.FirstOrDefault(x => x.Id.ToString() == address);

                    var order = new Orders()
                    {
                        AddedDate = DateTime.Now,
                        Address = _address.AdresDescription,
                        Member_Id = CurrentUserId(),
                        Id = Guid.NewGuid(),
                        Status = "0"
                    };
                    foreach (var item in basket)
                    {
                        var oDetail = new OrderDetails();
                        oDetail.AddedDate=DateTime.Now;
                        oDetail.Price = item.Product.Price * item.Count;
                        oDetail.Product_Id = item.Product.Id;
                        oDetail.Quantity = item.Count;
                        oDetail.Id = Guid.NewGuid();
                        order.OrderDetails.Add(oDetail);
                        Context.Orders.Add(order);

                        var product = Context.Products.FirstOrDefault(x => x.Id == item.Product.Id);
                        if (product!=null&&product.UnitsInStock>=item.Count)
                        {
                            product.UnitsInStock = product.UnitsInStock - item.Count;
                        }
                        else
                        {
                            throw new Exception(string.Format("'{0}' ürününe ait yeterli stok bulunamadı",item.Product.Name));
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception e)
                {
                    TempData["info"] = "Bir hata meydana geldi tekrar deneyiniz" + e;
                }
            }



            return RedirectToAction("Index", "Home");
        }

    }
}