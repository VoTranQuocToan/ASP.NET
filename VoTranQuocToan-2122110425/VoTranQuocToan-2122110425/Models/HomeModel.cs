using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoTranQuocToan_2122110425.Context;

namespace VoTranQuocToan_2122110425.Models
{
    public class HomeModel
    {
        public List<Product> ListProduct { get; set; }
        public List<Category> ListCategory { get; set; }
    }
}