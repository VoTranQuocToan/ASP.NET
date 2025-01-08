using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoTranQuocToan_2122110425.Context;

namespace VoTranQuocToan_2122110425.Models
{
    public class CartModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}