using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceAspNetMvc.Models
{
    public class RegisterViewModel
    {
        public DB.Members Member { get; set; }

        public string RePassword { get; set; }
    }
}