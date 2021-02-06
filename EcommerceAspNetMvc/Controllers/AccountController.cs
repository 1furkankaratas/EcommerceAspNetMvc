using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceAspNetMvc.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel user)
        {

            try
            {
                if (user.RePassword != user.Member.Password)
                {
                    throw new Exception("Şifreler uyuşmuyor");
                }

                //TODO:Email kontrol edilecek
                user.Member.MemberType = MemberTypes.Customer;
                user.Member.AddedDate = DateTime.Now;

                Context.Members.Add(user.Member);
                Context.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                ViewBag.info = "Bir hata meydana geldi" + " -> " + e.Message;
                return View(user); ;
            }



        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(RegisterViewModel model)
        {

            try
            {
                var user = Context.Members.FirstOrDefault(x =>
                    x.Password == model.Member.Password && x.Email == model.Member.Email);
                if (user != null)
                {
                    Session["logonuser"] = user;
                    return RedirectToAction("index", "Home");
                }
                else
                {
                    throw new Exception("Girilen bilgiler hatalı");
                }
            }
            catch (Exception e)
            {
                ViewBag.info = "Bir hata meydana geldi" + " -> " + e.Message;
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session["logonuser"] = null;
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Profil()
        {
            //TODO: id ile profile erişim kodlanacak
            var user = CurrentUser();

            if (user == null)
            {
                return RedirectToAction("index", "Home");
            }

            ProfilViewModel model = new ProfilViewModel()
            {
                Member = user,
                Addresses = Context.Addresses.Where(x => x.Member_Id == user.Id).ToList()
            };

            return View(model);
        }

        [Route("~/Profil/Edit")]
        public ActionResult ProfilEdit()
        {
            var user = CurrentUser();

            if (user == null)
            {
                return RedirectToAction("index", "Home");
            }

            ProfilViewModel model = new ProfilViewModel()
            {
                Member = user
            };

            return View(model);
        }

        [Route("~/Profil/Edit")]
        [HttpPost]
        public ActionResult ProfilEdit(ProfilViewModel model, HttpPostedFileBase img)
        {
            try
            {
                var user = CurrentUser();

                if (user == null)
                {
                    return RedirectToAction("index", "Home");
                }

                var cuser = Context.Members.FirstOrDefault(x => x.Id == user.Id);
                if (cuser != null)
                {
                    cuser.Bio = model.Member.Bio;
                    cuser.Name = model.Member.Name;
                    cuser.Surname = model.Member.Surname;
                    cuser.ModifiedDate = DateTime.Now;
                    if (img != null)
                    {
                        string path = Server.MapPath("~/wwwroot/img/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string fileName = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" +
                                          Path.GetFileName(img.FileName);
                        img.SaveAs(path + fileName);
                        cuser.ProfileImageName = fileName;
                    }
                }


                Context.SaveChanges();

                Session["logonuser"] = cuser;
            }
            catch (Exception e)
            {
                ViewBag.info = "Bir hata meydana geldi" + " -> " + e.Message;
                return View(model);
            }


            return RedirectToAction("Profil", "Account");
        }


        [HttpGet]
        public ActionResult AddAddress()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddAddress(AddAddressViewModel model)
        {

            try
            {
                model.Address.AddedDate = DateTime.Now;
                model.Address.Member_Id = CurrentUserId();
                model.Address.ModifiedDate = DateTime.Now;
                model.Address.Id = Guid.NewGuid();

                Context.Addresses.Add(model.Address);
                Context.SaveChanges();
            }
            catch
            {
                TempData["info"] = "Adres eklenirlen bir hata meydana geldi";
            }


            return RedirectToAction("Profil", "Account");
        }


        [HttpGet]
        public ActionResult RemoveAddress(int id, string addressId)
        {

            try
            {
                if (CurrentUserId() == id)
                {
                    var curAddress = Context.Addresses.FirstOrDefault(x => x.Id.ToString() == addressId);

                    Context.Entry(curAddress).State = EntityState.Deleted;
                    Context.SaveChanges();


                }
            }
            catch
            {
                TempData["info"] = "Bir hata meydana geldi";
            }


            return RedirectToAction("Profil", "Account");
        }


        [HttpGet]
        public ActionResult EditAddress(int id, string addressId)
        {
            AddAddressViewModel curAddress = new AddAddressViewModel();
            if (CurrentUserId() == id && addressId != null)
            {
                curAddress.Address = Context.Addresses.FirstOrDefault(x => x.Id.ToString() == addressId);

            }



            return View(curAddress);
        }


        [HttpPost]
        public ActionResult EditAddress(AddAddressViewModel model)
        {
            try
            {
                var curAddress = Context.Addresses.FirstOrDefault(x => x.Id == model.Address.Id);

                if (curAddress != null)
                {
                    curAddress.Name = model.Address.Name;
                    curAddress.AdresDescription = model.Address.AdresDescription;
                    curAddress.ModifiedDate = DateTime.Now;

                    Context.Entry(curAddress).State = EntityState.Modified;
                    Context.SaveChanges();

                }
            }
            catch
            {
                TempData["info"] = "Bir hata meydana geldi";
            }

            return RedirectToAction("Profil", "Account");
        }


        [HttpGet]
        public ActionResult ListOrder()
        {

            var user = CurrentUser();

            var orders = new List<Orders>();
            if (user!=null)
            {
                orders = Context.Orders.Where(x => x.Member_Id == user.Id).ToList();
            }

            return View(orders);
        }

    }
}