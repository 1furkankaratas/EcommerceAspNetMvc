using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Models
{
    public class HomeViewModel
    {
        public List<Products> Products { get; set; }
    }
}