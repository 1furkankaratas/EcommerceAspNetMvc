using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Models
{
    public class BasketViewModel
    {
        public Products Product { get; set; }

        public int Count { get; set; }

    }
}