using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Models
{
    public class ProductViewModel
    {
        public Products Product { get; set; }

        public List<Comments> Comments { get; set; }

        public Comments Comment { get; set; }
    }
}