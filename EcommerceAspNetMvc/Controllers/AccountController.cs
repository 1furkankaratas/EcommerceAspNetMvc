using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;

namespace EcommerceAspNetMvc.Controllers
{
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
                user.Member.AddedDate=DateTime.Now;
                
                Context.Members.Add(user.Member);
                Context.SaveChanges();
                return RedirectToAction("Login","Account");
            }
            catch (Exception e)
            {
                ViewBag.info = "Bir hata meydana geldi" + " -> " + e.Message;
                return View(user);;
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
                if (user!=null)
                {
                    Session["logonuser"] = user;
                    return RedirectToAction("index", "i");
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
            return RedirectToAction("Login","Account");
        }

        public ActionResult Profil()
        {
            return View();
        }
    }
}