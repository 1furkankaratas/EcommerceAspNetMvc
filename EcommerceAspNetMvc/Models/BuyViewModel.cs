using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Models
{
    public class BuyViewModel
    {
        public List<Addresses> Addresseses { get; set; }

        public List<BasketViewModel> BasketView { get; set; }

    }
}